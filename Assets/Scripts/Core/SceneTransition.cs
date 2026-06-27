using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneTransition : MonoBehaviour
{
    public static SceneTransition Instance { get; private set; }

    public float fadeDuration = 1f;
    public float titleDuration = 3.5f;
    public bool IsShowingChapterTitle { get; private set; }

    Canvas canvas;
    Image fadeImage;
    GameObject titlePanel;
    Text titleBadge;
    Text titleText;
    Text subtitleText;
    GameObject endingPanel;
    Text endingText;
    Text endingHint;

    bool isTransitioning;
    AudioSource titleVoiceSource;

    static readonly Color Gold = new Color(1f, 0.82f, 0.28f, 1f);
    static readonly Color CardBg = new Color(0.05f, 0.04f, 0.03f, 0.98f);

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
        StopTitleVoice();
        if (fadeImage != null)
            fadeImage.color = new Color(0, 0, 0, 0);
        if (titlePanel != null)
            titlePanel.SetActive(false);
        if (endingPanel != null)
            endingPanel.SetActive(false);
        IsShowingChapterTitle = false;
        isTransitioning = false;
        UpdateMenuBlocker();
    }

    void UpdateMenuBlocker()
    {
        var gr = GetComponent<GraphicRaycaster>();
        if (gr == null) return;
        gr.enabled = SceneManager.GetActiveScene().name != GameManager.SceneMainMenu;
    }

    void BuildUI()
    {
        canvas = gameObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 999;
        var scaler = gameObject.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.matchWidthOrHeight = 0.5f;
        gameObject.AddComponent<GraphicRaycaster>();
        UpdateMenuBlocker();

        titleVoiceSource = gameObject.AddComponent<AudioSource>();
        titleVoiceSource.playOnAwake = false;
        titleVoiceSource.spatialBlend = 0f;
        titleVoiceSource.loop = false;

        fadeImage = CreateFullScreenImage("Fade", new Color(0, 0, 0, 0));
        fadeImage.raycastTarget = false;

        BuildChapterTitlePanel();

        endingPanel = CreatePanel("EndingPanel", new Color(0, 0, 0, 0.96f));

        var endingCard = CreateBox(endingPanel.transform, "EndingCard", new Vector2(980f, 460f), CardBg);

        endingText = CreateText(endingCard.transform, "Ending", 38, TextAnchor.MiddleCenter, new Vector2(0f, 36f), Color.white);
        endingText.rectTransform.sizeDelta = new Vector2(900f, 260f);
        endingText.lineSpacing = 1.28f;

        endingHint = CreateText(endingCard.transform, "Hint", 30, TextAnchor.MiddleCenter, new Vector2(0f, -168f), Gold);
        endingHint.rectTransform.sizeDelta = new Vector2(900f, 48f);
        endingHint.text = "Nhấn Space để quay về menu";

        endingPanel.SetActive(false);
    }

    void BuildChapterTitlePanel()
    {
        titlePanel = CreatePanel("TitlePanel", new Color(0, 0, 0, 0.88f));

        var card = CreateBox(titlePanel.transform, "TitleCard", new Vector2(920f, 360f), CardBg);

        CreateBox(card.transform, "AccentLine", new Vector2(760f, 6f), Gold)
            .GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, 148f);

        titleBadge = CreateText(card.transform, "Badge", 30, TextAnchor.MiddleCenter, new Vector2(0f, 118f), Gold);
        titleBadge.rectTransform.sizeDelta = new Vector2(800f, 44f);

        titleText = CreateText(card.transform, "Title", 52, TextAnchor.MiddleCenter, new Vector2(0f, 48f), Color.white);
        titleText.rectTransform.sizeDelta = new Vector2(820f, 72f);

        subtitleText = CreateText(card.transform, "Subtitle", 34, TextAnchor.MiddleCenter, new Vector2(0f, -42f), Color.white);
        subtitleText.rectTransform.sizeDelta = new Vector2(820f, 100f);
        subtitleText.lineSpacing = 1.2f;

        titlePanel.SetActive(false);
    }

    public void LoadScene(string sceneName)
    {
        StartCoroutine(LoadSceneRoutine(sceneName, null, null, 0, false));
    }

    public void TransitionToChapter(string sceneName, string title, string subtitle, int chapterIndex, string voiceKey = null)
    {
        if (isTransitioning) return;
        StartCoroutine(TransitionRoutine(sceneName, title, subtitle, chapterIndex, voiceKey));
    }

    public void ShowEnding(string message, Action onComplete, string voiceKey = null)
    {
        if (isTransitioning) return;
        StartCoroutine(EndingRoutine(message, onComplete, voiceKey));
    }

    IEnumerator TransitionRoutine(string sceneName, string title, string subtitle, int chapterIndex, string voiceKey)
    {
        isTransitioning = true;
        GameManager.Instance?.LockInput(true);
        GameUI.Instance?.SetHudVisible(false);

        yield return Fade(0f, 1f);

        AsyncOperation load = SceneManager.LoadSceneAsync(sceneName);
        while (!load.isDone) yield return null;

        GameManager.Instance?.SetCurrentChapter(chapterIndex);
        GameManager.Instance?.FindPlayer();

        titleBadge.text = $"CHƯƠNG {chapterIndex}";
        titleText.text = ExtractChapterName(title, chapterIndex);
        subtitleText.text = subtitle;

        IsShowingChapterTitle = true;
        titlePanel.SetActive(true);

        yield return Fade(1f, 0f);

        float waitDuration = PlayTitleVoice(voiceKey);
        float endTime = Time.unscaledTime + waitDuration;
        while (Time.unscaledTime < endTime)
            yield return null;

        StopTitleVoice();
        titlePanel.SetActive(false);
        IsShowingChapterTitle = false;
        GameUI.Instance?.SetHudVisible(true);
        GameManager.Instance?.LockInput(false);
        isTransitioning = false;
    }

    float PlayTitleVoice(string voiceKey)
    {
        StopTitleVoice();
        if (string.IsNullOrWhiteSpace(voiceKey) || titleVoiceSource == null)
            return titleDuration;

        var clip = Resources.Load<AudioClip>($"Audio/Dialogue/{voiceKey.Trim()}");
        if (clip == null)
        {
            Debug.LogWarning($"[SceneTransition] Không tìm thấy giọng: Audio/Dialogue/{voiceKey}");
            return titleDuration;
        }

        titleVoiceSource.clip = clip;
        titleVoiceSource.volume = AudioSettings.DialogueVoiceScaled;
        GameMusicController.Instance?.SetMusicDuck(0.22f);
        titleVoiceSource.Play();
        return Mathf.Max(titleDuration, clip.length + 0.25f);
    }

    void StopTitleVoice()
    {
        if (titleVoiceSource != null && titleVoiceSource.isPlaying)
            titleVoiceSource.Stop();
        GameMusicController.Instance?.SetMusicDuck(1f);
    }

    IEnumerator LoadSceneRoutine(string sceneName, string title, string subtitle, int chapterIndex, bool showTitle)
    {
        isTransitioning = true;
        GameManager.Instance?.LockInput(true);
        yield return Fade(0f, 1f);

        AsyncOperation load = SceneManager.LoadSceneAsync(sceneName);
        while (!load.isDone) yield return null;

        if (sceneName == GameManager.SceneMainMenu)
            PrepareMainMenuReturn();

        if (showTitle && titlePanel != null)
        {
            titleBadge.text = chapterIndex > 0 ? $"CHƯƠNG {chapterIndex}" : "";
            titleText.text = title ?? "";
            subtitleText.text = subtitle ?? "";
            titlePanel.SetActive(true);
            yield return Fade(1f, 0f);
            yield return new WaitForSecondsRealtime(titleDuration);
            titlePanel.SetActive(false);
        }
        else
        {
            yield return Fade(1f, 0f);
        }

        isTransitioning = false;
        GameManager.Instance?.LockInput(false);
    }

    static void PrepareMainMenuReturn()
    {
        GameUI.Instance?.SetHudVisible(false);
        GameUI.Instance?.SetInteractPrompt(false);
        GameManager.Instance?.LockInput(false);
        GameMusicController.Instance?.SetMusicDuck(1f);

        var menuUi = UnityEngine.Object.FindFirstObjectByType<MainMenuUI>();
        if (menuUi != null)
            menuUi.ReturnToMainScreen();
    }

    IEnumerator EndingRoutine(string message, Action onComplete, string voiceKey)
    {
        isTransitioning = true;
        GameManager.Instance?.LockInput(true);
        GameUI.Instance?.SetHudVisible(false);

        yield return Fade(0f, 1f);

        GameMusicController.Instance?.PlayEndingMusic();
        GameMusicController.Instance?.SetMusicDuck(0.22f);
        endingText.text = message;
        endingPanel.SetActive(true);
        yield return Fade(1f, 0f);

        PlayTitleVoice(voiceKey);

        while (titleVoiceSource != null && titleVoiceSource.isPlaying)
        {
            if (GameInput.SpacePressedThisFrame)
            {
                StopTitleVoice();
                break;
            }
            yield return null;
        }

        yield return new WaitUntil(() => GameInput.SpacePressedThisFrame);

        StopTitleVoice();
        endingPanel.SetActive(false);
        yield return Fade(0f, 1f);

        isTransitioning = false;
        onComplete?.Invoke();
    }

    static string ExtractChapterName(string fullTitle, int chapter)
    {
        if (string.IsNullOrEmpty(fullTitle)) return "";
        string prefix = $"Chương {chapter}: ";
        return fullTitle.StartsWith(prefix, StringComparison.OrdinalIgnoreCase)
            ? fullTitle.Substring(prefix.Length)
            : fullTitle;
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

    GameObject CreateBox(Transform parent, string name, Vector2 size, Color color)
    {
        var go = new GameObject(name, typeof(RectTransform), typeof(Image));
        go.transform.SetParent(parent, false);
        var rt = go.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0.5f, 0.5f);
        rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.sizeDelta = size;
        rt.anchoredPosition = Vector2.zero;
        go.GetComponent<Image>().color = color;
        go.GetComponent<Image>().raycastTarget = false;
        return go;
    }

    Text CreateText(Transform parent, string name, int fontSize, TextAnchor anchor, Vector2 anchoredPos, Color color)
    {
        var go = new GameObject(name, typeof(RectTransform), typeof(Text));
        go.transform.SetParent(parent, false);
        var rt = go.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0.5f, 0.5f);
        rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.sizeDelta = new Vector2(900, 200);
        rt.anchoredPosition = anchoredPos;

        var text = go.GetComponent<Text>();
        text.font = CrispUiText.GetFont() ?? Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        if (text.font == null)
        {
            Destroy(go);
            return null;
        }
        text.fontSize = fontSize;
        text.fontStyle = FontStyle.Bold;
        text.alignment = anchor;
        text.color = color;
        text.horizontalOverflow = HorizontalWrapMode.Wrap;
        text.verticalOverflow = VerticalWrapMode.Overflow;
        CrispUiText.ApplyReadableDefaults(text);
        CrispUiText.WarmAtlas(fontSize);
        return text;
    }
}
