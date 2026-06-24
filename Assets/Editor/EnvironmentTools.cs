using UnityEngine;
using UnityEditor;

public class EnvironmentTools : EditorWindow
{
    [MenuItem("Tools/Scale Random NewTrees")]
    public static void ScaleRandomNewTrees()
    {
        GameObject newTreesParent = GameObject.Find("NewTrees");
        if (newTreesParent == null) {
            Debug.Log("Could not find NewTrees parent.");
            return;
        }

        int count = 0;
        foreach (Transform tree in newTreesParent.transform) {
            if (Random.value < 0.4f) {
                float scale = Random.Range(1.3f, 2.0f);
                tree.localScale = tree.localScale * scale;
                Debug.Log("Scaled " + tree.name + " by " + scale);
                count++;
            }
        }

        if (count == 0 && newTreesParent.transform.childCount > 0) {
            Transform tree = newTreesParent.transform.GetChild(0);
            float scale = Random.Range(1.3f, 2.0f);
            tree.localScale = tree.localScale * scale;
            Debug.Log("Scaled " + tree.name + " by " + scale);
            count++;
        }
        
        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(UnityEngine.SceneManagement.SceneManager.GetActiveScene());
        Debug.Log($"Total trees scaled: {count}");
    }

    [MenuItem("Tools/Apply MatRock to Chapter 1 and 3")]
    public static void ApplyMatRock()
    {
        string[] scenes = { "Assets/Scenes/Chapter1.unity", "Assets/Scenes/Chapter3.unity" };
        Material matRock = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/RockMat/MatRock.mat");
        if (matRock == null) {
            Debug.LogError("Could not find MatRock at Assets/Materials/RockMat/MatRock.mat");
            return;
        }

        foreach (string scenePath in scenes) {
            UnityEditor.SceneManagement.EditorSceneManager.OpenScene(scenePath, UnityEditor.SceneManagement.OpenSceneMode.Single);
            int count = 0;
            GameObject[] allObjects = FindObjectsOfType<GameObject>(true); // include inactive
            foreach (GameObject go in allObjects) {
                if (go.name.ToLower().Contains("cliff_rock_model") || go.name.ToLower().Contains("đá") || go.name.ToLower().Contains("rock")) {
                    Renderer[] renderers = go.GetComponentsInChildren<Renderer>(true);
                    foreach (Renderer r in renderers) {
                        Undo.RecordObject(r, "Apply MatRock");
                        r.sharedMaterial = matRock;
                        EditorUtility.SetDirty(r);
                        PrefabUtility.RecordPrefabInstancePropertyModifications(r);
                    }
                    count++;
                }
            }
            UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());
            UnityEditor.SceneManagement.EditorSceneManager.SaveOpenScenes();
            Debug.Log($"Applied MatRock to {count} rocks in {scenePath}");
        }
        
        Debug.Log("Finished applying MatRock.");
    }
}
