using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Nếu mở thẳng Chapter/main thì đẩy về MainMenu.
/// </summary>
public static class GameEntryRedirect
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void RedirectIfNeeded()
    {
        string scene = SceneManager.GetActiveScene().name;

        if (scene == GameManager.SceneMainMenu)
        {
            GameSession.StartedFromMenu = false;
            return;
        }

        if (!IsGameplayScene(scene)) return;

        if (!GameSession.StartedFromMenu)
            SceneManager.LoadScene(GameManager.SceneMainMenu);
    }

    static bool IsGameplayScene(string scene) =>
        scene == "Chapter1" || scene == "Chapter2" || scene == "Chapter3" || scene == "main";
}
