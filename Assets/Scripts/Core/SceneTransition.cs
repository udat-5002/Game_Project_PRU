using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneTransition : MonoBehaviour
{
    public static SceneTransition Instance { get; private set; }

    public float fadeDuration = 1f;
    public float titleDuration = 3f;

    Canvas canvas;
    Image fadeImage;
    GameObject titlePanel;
    Text titleText;
    Text subtitleText;
    GameObject endingPanel;
    Text endingText;

    bool isTransitioning;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        BuildUI();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDestroy()
    {
        if (Instance == this)
            SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        UpdateMenuBlocker();
        if (scene.name == GameManager.SceneMainMenu)
            ResetForMainMenu();
    }

    public void ResetForMainMenu()
    {
        if (fadeImage != null)
            fadeImage.color = new Color(0, 0, 0, 0);
        if (titlePanel != null)
            titlePanel.SetActive(false);
        if (endingPanel != null)
            endingPanel.SetActive(false);
        isTransitioning = false;
        UpdateMenuBlocker();
    }

    void UpdateMenuBlocker()
    {
        var gr = GetComponent<GraphicRaycaster>();
        if (gr == null) return;
        // Không chặn click menu khi ở MainMenu
        gr.enabled = SceneManager.GetActiveScene().name != GameManager.SceneMainMenu;
    }

    void BuildUI()
    {
        canvas = gameObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 999;
        gameObject.AddComponent<CanvasScaler>();
        gameObject.AddComponent<GraphicRaycaster>();
        UpdateMenuBlocker();

        fadeImage = CreateFullScreenImage("Fade", new Color(0, 0, 0, 0));
        fadeImage.raycastTarget = false;

        titlePanel = CreatePanel("TitlePanel", new Color(0, 0, 0, 0.85f));
        titleText = CreateText(titlePanel.transform, "Title", 44, TextAnchor.MiddleCenter, new Vector2(0, 40));
        subtitleText = CreateText(titlePanel.transform, "Subtitle", 28, TextAnchor.MiddleCenter, new Vector2(0, -40));
        titlePanel.SetActive(false);

        endingPanel = CreatePanel("EndingPanel", new Color(0, 0, 0, 0.92f));
        endingText = CreateText(endingPanel.transform, "Ending", 30, TextAnchor.MiddleCenter, Vector2.zero);
        var endingHint = CreateText(endingPanel.transform, "Hint", 22, TextAnchor.LowerCenter, new Vector2(0, 40));
        endingHint.text = "Nhấn Space để quay về menu";
        endingPanel.SetActive(false);
    }

    public void LoadScene(string sceneName)
    {
        if (!isTransitioning)
            StartCoroutine(LoadSceneRoutine(sceneName, null, null, 0, false));
    }

    public void TransitionToChapter(string sceneName, string title, string subtitle, int chapterIndex)
    {
        if (isTransitioning) return;
        StartCoroutine(TransitionRoutine(sceneName, title, subtitle, chapterIndex));
    }

    public void ShowEnding(string message, Action onComplete)
    {
        if (isTransitioning) return;
        StartCoroutine(EndingRoutine(message, onComplete));
    }

    IEnumerator TransitionRoutine(string sceneName, string title, string subtitle, int chapterIndex)
    {
        isTransitioning = true;
        GameManager.Instance?.LockInput(true);

        yield return Fade(0f, 1f);

        AsyncOperation load = SceneManager.LoadSceneAsync(sceneName);
        while (!load.isDone) yield return null;

        GameManager.Instance?.SetCurrentChapter(chapterIndex);
        GameManager.Instance?.FindPlayer();

        titleText.text = title;
        subtitleText.text = subtitle;
        titlePanel.SetActive(true);

        yield return Fade(1f, 0f);
        yield return new WaitForSeconds(titleDuration);

        titlePanel.SetActive(false);
        GameManager.Instance?.LockInput(false);
        isTransitioning = false;
    }

    IEnumerator LoadSceneRoutine(string sceneName, string title, string subtitle, int chapterIndex, bool showTitle)
    {
        isTransitioning = true;
        yield return Fade(0f, 1f);

        AsyncOperation load = SceneManager.LoadSceneAsync(sceneName);
        while (!load.isDone) yield return null;

        if (showTitle && titlePanel != null)
        {
            titleText.text = title ?? "";
            subtitleText.text = subtitle ?? "";
            titlePanel.SetActive(true);
            yield return Fade(1f, 0f);
            yield return new WaitForSeconds(titleDuration);
            titlePanel.SetActive(false);
        }
        else
        {
            yield return Fade(1f, 0f);
        }

        isTransitioning = false;
    }

    IEnumerator EndingRoutine(string message, Action onComplete)
    {
        isTransitioning = true;
        GameManager.Instance?.LockInput(true);

        yield return Fade(0f, 1f);

        endingText.text = message;
        endingPanel.SetActive(true);
        yield return Fade(1f, 0f);

        yield return new WaitUntil(() => GameInput.SpacePressedThisFrame);

        endingPanel.SetActive(false);
        yield return Fade(0f, 1f);

        onComplete?.Invoke();
        isTransitioning = false;
    }

    IEnumerator Fade(float from, float to)
    {
        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.unscaledDeltaTime;
            float a = Mathf.Lerp(from, to, t / fadeDuration);
            fadeImage.color = new Color(0, 0, 0, a);
            yield return null;
        }
        fadeImage.color = new Color(0, 0, 0, to);
    }

    Image CreateFullScreenImage(string name, Color color)
    {
        var go = new GameObject(name, typeof(RectTransform), typeof(Image));
        go.transform.SetParent(canvas.transform, false);
        var rt = go.GetComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
        var img = go.GetComponent<Image>();
        img.color = color;
        return img;
    }

    GameObject CreatePanel(string name, Color color)
    {
        var go = new GameObject(name, typeof(RectTransform), typeof(Image));
        go.transform.SetParent(canvas.transform, false);
        var rt = go.GetComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
        go.GetComponent<Image>().color = color;
        go.GetComponent<Image>().raycastTarget = false;
        return go;
    }

    Text CreateText(Transform parent, string name, int fontSize, TextAnchor anchor, Vector2 anchoredPos)
    {
        var go = new GameObject(name, typeof(RectTransform), typeof(Text));
        go.transform.SetParent(parent, false);
        var rt = go.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0.5f, 0.5f);
        rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.sizeDelta = new Vector2(900, 200);
        rt.anchoredPosition = anchoredPos;

        var text = go.GetComponent<Text>();
        text.font = CrispUiText.GetFont() ?? Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        if (text.font == null)
        {
            UnityEngine.Object.Destroy(go);
            return null;
        }
        text.fontSize = fontSize;
        text.fontStyle = FontStyle.Bold;
        text.alignment = anchor;
        text.color = Color.white;
        text.horizontalOverflow = HorizontalWrapMode.Wrap;
        text.verticalOverflow = VerticalWrapMode.Overflow;
        text.supportRichText = false;
        text.resizeTextForBestFit = false;
        text.raycastTarget = false;
        CrispUiText.WarmAtlas(fontSize);
        return text;
    }
}
