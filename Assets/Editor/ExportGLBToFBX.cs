using UnityEditor;
using UnityEngine;
using UnityEditor.Formats.Fbx.Exporter;
using System.IO;

[InitializeOnLoad]
public class ExportGLBToFBXAuto
{
    static ExportGLBToFBXAuto()
    {
        EditorApplication.delayCall += () => {
            if (SessionState.GetBool("HasExportedLinhVietFBX2", false)) return;
            SessionState.SetBool("HasExportedLinhVietFBX2", true);
            
            var path = "Assets/LinhViet.glb";
            var obj = AssetDatabase.LoadMainAssetAtPath(path);
            
            string log = "Log start\n";
            if (obj != null) {
                log += "Loaded object of type: " + obj.GetType().Name + "\n";
                if (obj is GameObject go) {
                    var outPath = "Assets/LinhViet_ForMixamo.fbx";
                    ModelExporter.ExportObject(outPath, go);
                    log += "Exported to FBX\n";
                } else {
                    log += "Object is not a GameObject, cannot export directly.\n";
                }
            } else {
                log += "Failed to load Main Asset at " + path + "\n";
            }
            File.WriteAllText("Assets/export_log.txt", log);
            AssetDatabase.Refresh();
        };
    }
}
