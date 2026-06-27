using UnityEngine;
using UnityEngine.SceneManagement;

public static class MainMenuBootstrap
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void Init()
    {
        if (SceneManager.GetActiveScene().name != GameManager.SceneMainMenu) return;

        EnsureCameraAndLight();
        EnsureMainMenuUi();
        SceneTransition.Instance?.ResetForMainMenu();

        var menuUi = Object.FindFirstObjectByType<MainMenuUI>();
        menuUi?.ReturnToMainScreen();
        GameUI.Instance?.SetHudVisible(false);
        GameManager.Instance?.LockInput(false);
    }

    static void EnsureCameraAndLight()
    {
        if (Object.FindFirstObjectByType<Camera>() == null)
        {
            var camGo = new GameObject("Main Camera");
            camGo.tag = "MainCamera";
            var cam = camGo.AddComponent<Camera>();
            cam.clearFlags = CameraClearFlags.Skybox;
            camGo.AddComponent<AudioListener>();
        }

        if (Object.FindFirstObjectByType<Light>() == null)
        {
            var lightGo = new GameObject("Directional Light");
            var light = lightGo.AddComponent<Light>();
            light.type = LightType.Directional;
            lightGo.transform.rotation = Quaternion.Euler(50f, -30f, 0f);
        }
    }

    static void EnsureMainMenuUi()
    {
        if (Object.FindFirstObjectByType<MainMenuUI>() != null) return;

        var controller = Object.FindFirstObjectByType<MainMenuController>();
        var menuGo = controller != null ? controller.gameObject : new GameObject("MainMenu");

        if (controller == null)
            menuGo.AddComponent<MainMenuController>();

        if (menuGo.GetComponent<MainMenuUI>() == null)
            menuGo.AddComponent<MainMenuUI>();
    }
}
