#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class ChapterSceneSetup
{
    const string MainScenePath = "Assets/Scenes/main.unity";

    [MenuItem("Người Đưa Thư/Khôi Phục Scene MainMenu")]
    public static void RestoreMainMenuScene()
    {
        CreateMainMenuScene();
        EditorUtility.DisplayDialog("Người Đưa Thư",
            "Đã khôi phục scene MainMenu (Camera + Light + MainMenu + hệ thống).\n\nBấm Play để kiểm tra.",
            "OK");
    }

    [MenuItem("Người Đưa Thư/Setup 3 Chapters + Build Settings")]
    public static void SetupAllChapters()
    {
        CreateMainMenuScene();
        CreateChapterScene(1, new Vector3(14.38f, 3.09f, -41.3f));
        CreateChapterScene(2, new Vector3(20f, 3.09f, -35f));
        CreateChapterScene(3, new Vector3(15f, 3.09f, -20f));
        SetupBuildSettings();
        PlayFromMainMenu.Apply();
        AssetDatabase.SaveAssets();
        EditorUtility.DisplayDialog("Người Đưa Thư",
            "Đã tạo MainMenu + Chapter1/2/3.\n\n" +
            "Chạy game từ scene MainMenu.\n" +
            "Các marker màu (cube) là NPC/vùng nhiệm vụ — chỉnh vị trí trong Inspector nếu cần.",
            "OK");
    }

    static void CreateMainMenuScene()
    {
        var scene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);

        var menuGo = new GameObject("MainMenu");
        menuGo.AddComponent<MainMenuController>();

        EnsurePersistentSystems();

        EditorSceneManager.SaveScene(scene, "Assets/Scenes/MainMenu.unity");
    }

    static void CreateChapterScene(int chapter, Vector3 spawn)
    {
        if (!File.Exists(MainScenePath))
        {
            Debug.LogError("Không tìm thấy main.unity");
            return;
        }

        string dest = $"Assets/Scenes/Chapter{chapter}.unity";
        if (!File.Exists(dest))
            AssetDatabase.CopyAsset(MainScenePath, dest);

        var scene = EditorSceneManager.OpenScene(dest, OpenSceneMode.Single);

        // Xóa hệ thống menu cũ nếu lỡ gắn vào scene chapter
        RemoveIfExists<GameManager>();
        RemoveIfExists<SceneTransition>();
        RemoveIfExists<GameUI>();
        RemoveIfExists<DialogueManager>();
        RemoveIfExists<QuestManager>();
        RemoveIfExists<ChapterFlowController>();

        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
    }

    static void RemoveIfExists<T>() where T : Object
    {
        var obj = Object.FindFirstObjectByType<T>();
        if (obj != null)
            Object.DestroyImmediate(obj is Component c ? c.gameObject : obj);
    }

    static void EnsurePersistentSystems()
    {
        if (Object.FindFirstObjectByType<GameManager>() == null)
            new GameObject("GameManager").AddComponent<GameManager>();
        if (Object.FindFirstObjectByType<SceneTransition>() == null)
            new GameObject("SceneTransition").AddComponent<SceneTransition>();
        if (Object.FindFirstObjectByType<GameUI>() == null)
            new GameObject("GameUI").AddComponent<GameUI>();
        if (Object.FindFirstObjectByType<DialogueManager>() == null)
            new GameObject("DialogueManager").AddComponent<DialogueManager>();
        if (Object.FindFirstObjectByType<QuestManager>() == null)
            new GameObject("QuestManager").AddComponent<QuestManager>();
    }

    static void SetupBuildSettings()
    {
        EditorBuildSettings.scenes = new[]
        {
            new EditorBuildSettingsScene("Assets/Scenes/MainMenu.unity", true),
            new EditorBuildSettingsScene("Assets/Scenes/Chapter1.unity", true),
            new EditorBuildSettingsScene("Assets/Scenes/Chapter2.unity", true),
            new EditorBuildSettingsScene("Assets/Scenes/Chapter3.unity", true),
        };
    }
}
#endif
