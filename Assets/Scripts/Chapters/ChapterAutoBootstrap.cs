using UnityEngine;
using UnityEngine.SceneManagement;

public static class ChapterAutoBootstrap
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void OnAfterSceneLoad()
    {
        string sceneName = SceneManager.GetActiveScene().name;
        if (sceneName == GameManager.SceneMainMenu) return;
        if (Object.FindFirstObjectByType<ChapterFlowController>() != null) return;

        int chapter = sceneName switch
        {
            "Chapter1" => 1,
            "Chapter2" => 2,
            "Chapter3" => 3,
            "main" => 1,
            _ => 0
        };

        if (chapter == 0) return;

        var go = new GameObject("ChapterFlow");
        var flow = go.AddComponent<ChapterFlowController>();
        flow.chapterIndex = chapter;
        flow.playerSpawnPosition = ForestZoneLayout.SnapPoint(chapter switch
        {
            1 => ForestZoneLayout.Ch1Spawn,
            2 => ForestZoneLayout.Ch2Spawn,
            3 => ForestZoneLayout.Ch3Spawn,
            _ => flow.playerSpawnPosition
        });
    }
}
