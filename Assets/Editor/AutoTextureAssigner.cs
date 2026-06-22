using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using System.Linq;

public class AutoTextureAssigner : EditorWindow
{
    private static string sourceDir = @"C:\0Main\HOC TAP CUK MANH\current\PRU213\1_PRU213";
    private static string targetTexDir = "Assets/Textures/AutoImported";
    private static string targetMatDir = "Assets/Materials/AutoImported";
    
    [MenuItem("Tools/Auto Assign Textures")]
    public static void RunAutoAssign()
    {
        Debug.Log("Starting Auto Texture Assignment...");
        
        if (!Directory.Exists(sourceDir))
        {
            Debug.LogError($"Source directory not found: {sourceDir}");
            return;
        }

        // Ensure directories exist
        EnsureDirectory(targetTexDir);
        EnsureDirectory(targetMatDir);

        // 1. Scan & Copy Textures
        string[] validExtensions = { ".png", ".jpg", ".jpeg", ".webp" };
        string[] files = Directory.GetFiles(sourceDir);
        List<string> importedTextures = new List<string>();

        foreach (string file in files)
        {
            if (validExtensions.Contains(Path.GetExtension(file).ToLower()))
            {
                string fileName = Path.GetFileName(file);
                string targetPath = Path.Combine(targetTexDir, fileName).Replace("\\", "/");
                if (!File.Exists(targetPath))
                {
                    File.Copy(file, targetPath);
                }
                importedTextures.Add(targetPath);
            }
        }
        
        AssetDatabase.Refresh();

        // 2. Create Materials
        Dictionary<string, Material> createdMaterials = new Dictionary<string, Material>();
        foreach (string texPath in importedTextures)
        {
            Texture2D tex = AssetDatabase.LoadAssetAtPath<Texture2D>(texPath);
            if (tex == null) continue;

            string fileName = Path.GetFileNameWithoutExtension(texPath).ToLower();
            string matPath = $"{targetMatDir}/{fileName}.mat";

            Material mat = AssetDatabase.LoadAssetAtPath<Material>(matPath);
            if (mat == null)
            {
                mat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
                AssetDatabase.CreateAsset(mat, matPath);
            }

            mat.SetTexture("_BaseMap", tex);
            
            // Alpha clipping logic
            if (fileName.Contains("leaf") || fileName.Contains("grass") || fileName.Contains("transparent") || UnityEngine.Experimental.Rendering.GraphicsFormatUtility.HasAlphaChannel(tex.graphicsFormat))
            {
                mat.SetFloat("_AlphaClip", 1f); // Enable alpha clipping in URP
                mat.EnableKeyword("_ALPHATEST_ON");
            }
            
            EditorUtility.SetDirty(mat);
            createdMaterials[fileName] = mat;
        }
        AssetDatabase.SaveAssets();

        // 3. Assign to models
        Renderer[] renderers = Object.FindObjectsByType<Renderer>(FindObjectsSortMode.None);
        List<string> successObjects = new List<string>();
        List<string> missingObjects = new List<string>();
        HashSet<string> usedTextures = new HashSet<string>();

        foreach (Renderer r in renderers)
        {
            if (!(r is MeshRenderer || r is SkinnedMeshRenderer)) continue;

            string objName = r.gameObject.name.ToLower();
            List<string> searchKeywords = new List<string>();

            // Categories based on user requirement
            if (objName.Contains("tree")) searchKeywords.AddRange(new string[] { "tree", "leaf", "grass" });
            else if (objName.Contains("ground") || objName.Contains("terrain")) searchKeywords.AddRange(new string[] { "ground", "forest", "terrain", "rocky" });
            else if (objName.Contains("rock") || objName.Contains("stone") || objName.Contains("cliff")) searchKeywords.AddRange(new string[] { "rock", "stone", "cliff", "marble" });
            else if (objName.Contains("house") || objName.Contains("building") || objName.Contains("hangbag")) searchKeywords.AddRange(new string[] { "wood", "wall", "roof", "house", "image" });
            else searchKeywords.Add(objName); // fallback

            Material matchedMat = null;
            string matchedKey = null;

            // Try to match partial keyword with material names
            foreach (var kvp in createdMaterials)
            {
                if (searchKeywords.Any(k => kvp.Key.Contains(k)))
                {
                    matchedMat = kvp.Value;
                    matchedKey = kvp.Key;
                    break;
                }
            }

            if (matchedMat != null)
            {
                // Remap existing editable material or assign matched material
                Material currentMat = r.sharedMaterial;
                bool isReadOnly = currentMat != null && !string.IsNullOrEmpty(AssetDatabase.GetAssetPath(currentMat)) && (AssetDatabase.GetAssetPath(currentMat).EndsWith(".fbx") || AssetDatabase.GetAssetPath(currentMat).EndsWith(".blend"));

                if (currentMat != null && !isReadOnly && AssetDatabase.GetAssetPath(currentMat).StartsWith("Assets/"))
                {
                    currentMat.SetTexture("_BaseMap", matchedMat.GetTexture("_BaseMap"));
                    if (matchedMat.GetFloat("_AlphaClip") == 1f) 
                    {
                        currentMat.SetFloat("_AlphaClip", 1f);
                        currentMat.EnableKeyword("_ALPHATEST_ON");
                    }
                    EditorUtility.SetDirty(currentMat);
                }
                else
                {
                    Material[] sharedMats = r.sharedMaterials;
                    for (int i = 0; i < sharedMats.Length; i++) sharedMats[i] = matchedMat;
                    r.sharedMaterials = sharedMats;
                }
                
                successObjects.Add(r.gameObject.name + " -> " + matchedKey);
                usedTextures.Add(matchedKey);
            }
            else
            {
                // Create temporary color material
                Material tempMat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
                tempMat.SetColor("_BaseColor", Color.magenta);
                string tempMatPath = $"{targetMatDir}/Temp_{objName}.mat";
                if (AssetDatabase.LoadAssetAtPath<Material>(tempMatPath) == null)
                {
                    AssetDatabase.CreateAsset(tempMat, tempMatPath);
                }
                else
                {
                    tempMat = AssetDatabase.LoadAssetAtPath<Material>(tempMatPath);
                }
                
                Material[] sharedMats = r.sharedMaterials;
                for (int i = 0; i < sharedMats.Length; i++) sharedMats[i] = tempMat;
                r.sharedMaterials = sharedMats;
                
                missingObjects.Add(r.gameObject.name);
            }
        }

        // 4. Report
        string report = "==== AUTO TEXTURE ASSIGNMENT REPORT ====\n\n";
        report += "OBJECTS SUCCESSFULLY TEXTURED:\n" + (successObjects.Count > 0 ? string.Join("\n", successObjects) : "None") + "\n\n";
        report += "OBJECTS MISSING TEXTURES (Assigned Magenta color):\n" + (missingObjects.Count > 0 ? string.Join("\n", missingObjects) : "None") + "\n\n";
        
        List<string> unusedTextures = createdMaterials.Keys.Where(k => !usedTextures.Contains(k)).ToList();
        report += "UNUSED TEXTURES:\n" + (unusedTextures.Count > 0 ? string.Join("\n", unusedTextures) : "None") + "\n";

        Debug.Log(report);
        string reportPath = "Assets/AutoAssignReport.txt";
        File.WriteAllText(reportPath, report);
        AssetDatabase.ImportAsset(reportPath);
        Debug.Log($"Report saved to {reportPath}");
    }

    private static void EnsureDirectory(string path)
    {
        if (!AssetDatabase.IsValidFolder(path))
        {
            string parent = Path.GetDirectoryName(path).Replace("\\", "/");
            string folder = Path.GetFileName(path);
            if (!AssetDatabase.IsValidFolder(parent))
            {
                EnsureDirectory(parent);
            }
            AssetDatabase.CreateFolder(parent, folder);
        }
    }
}
