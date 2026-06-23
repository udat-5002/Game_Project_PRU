#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

/// <summary>
/// Luôn bắt đầu Play từ MainMenu — dù đang mở scene Chapter nào.
/// </summary>
[InitializeOnLoad]
public static class PlayFromMainMenu
{
    const string MainMenuPath = "Assets/Scenes/MainMenu.unity";

    static PlayFromMainMenu()
    {
        Apply();
    }

    [MenuItem("Người Đưa Thư/Play From MainMenu (Bật)")]
    public static void Apply()
    {
        var scene = AssetDatabase.LoadAssetAtPath<SceneAsset>(MainMenuPath);
        if (scene != null)
        {
            EditorSceneManager.playModeStartScene = scene;
            Debug.Log("[Người Đưa Thư] Play Mode sẽ luôn bắt đầu từ MainMenu.");
        }
    }
}
#endif
