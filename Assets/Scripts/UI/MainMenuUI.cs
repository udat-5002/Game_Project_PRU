using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    Canvas canvas;
    GameObject mainPanel;
    GameObject settingsPanel;
    GameObject introPanel;

    Font uiFont;
    Text introBody;
    Text introPageIndicator;
    Text introActionLabel;
    int introPageIndex;
    AudioSource introVoiceSource;
    Coroutine introVoiceCoroutine;
    int introVoiceCoroutinePage = -1;

    static readonly string[] IntroVoiceKeys =
    {
        MenuVoice.Intro,
        null
    };

    static readonly string[] IntroPages =
    {
        "Thời chiến tranh, một ngôi làng nhỏ ở miền Trung Việt Nam.\n" +
        "Chiến tranh đang diễn ra ác liệt.\n\n" +
        "Người dân sống trong cảnh:\n" +
        "• thiếu lương thực\n" +
        "• thường xuyên sơ tán\n" +
        "• lo sợ bom đạn",

        "Nam là một thanh niên 19 tuổi sống cùng mẹ.\n\n" +
        "Anh trai Nam đã ra chiến trường và mất liên lạc hơn 6 tháng."
    };

    static readonly Color YearRed = new Color(1f, 0.32f, 0.14f, 1f);
    static readonly Color TitleWhite = Color.white;
    static readonly Color Accent = new Color(1f, 0.82f, 0.28f, 1f);
    static readonly Color AccentHover = new Color(1f, 0.92f, 0.45f, 1f);
    static readonly Color PanelBg = new Color(0.03f, 0.02f, 0.02f, 0.98f);
    static readonly Color IntroBoxBg = new Color(0.02f, 0.02f, 0.03f, 1f);
    static readonly Color TextBright = Color.white;
    static readonly Color TextCream = new Color(1f, 0.97f, 0.9f, 1f);

    void Awake()
    {
        try
        {
            AudioSettings.Apply();
            CrispUiText.WarmMenuAtlas();
            uiFont = CrispUiText.GetFont();
            EnsureEventSystem();
            BuildCanvas();
            BuildMainPanel();
            BuildSettingsPanel();
            BuildIntroPanel();
            introVoiceSource = gameObject.AddComponent<AudioSource>();
            introVoiceSource.playOnAwake = false;
            introVoiceSource.spatialBlend = 0f;
            introVoiceSource.loop = false;
            ShowMain();
        }
        catch (System.Exception e)
        {
            Debug.LogError("[MainMenuUI] Không tạo được menu: " + e.Message);
            BuildFallbackMenu();
        }
    }

    void BuildFallbackMenu()
    {
        if (canvas == null)
        {
            canvas = gameObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 200;
            gameObject.AddComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            gameObject.AddComponent<GraphicRaycaster>();
        }

        mainPanel = CreatePanel("MainPanel", canvas.transform);
        CreateLabel(mainPanel.transform, "FallbackTitle", "NGƯỜI ĐƯA THƯ",
            56, TextAnchor.MiddleCenter, new Vector2(0.5f, 0.55f), new Vector2(900, 96), TextBright, FontStyle.Bold);
        CreatePosterButton(mainPanel.transform, "BtnStart", "BẮT ĐẦU CHƠI", new Vector2(0.5f, 0.35f), StartGame);
    }

    static void EnsureEventSystem()
    {
        if (Object.FindFirstObjectByType<EventSystem>() != null) return;
        var go = new GameObject("EventSystem");
        go.AddComponent<EventSystem>();
        go.AddComponent<InputSystemUIInputModule>();
    }

    void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    void Update()
    {
        if (introPanel == null || !introPanel.activeInHierarchy) return;
        if (GameInput.EnterPressedThisFrame)
            IntroAdvanceOrStart();
    }

    void BuildCanvas()
    {
        canvas = gameObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 200;

        var scaler = gameObject.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.matchWidthOrHeight = 1f;
        gameObject.AddComponent<GraphicRaycaster>();

        CreatePosterBackground();
        CreateBottomVignette();
    }

    void CreatePosterBackground()
    {
        var go = CreateImage("PosterBg", canvas.transform, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero, Color.white);
        var img = go.GetComponent<Image>();
        img.raycastTarget = false;
        img.preserveAspect = false;

        var tex = Resources.Load<Texture2D>("UI/MenuBackground_1975");
        if (tex != null)
        {
            img.sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f), 100f);
        }
        else
        {
            img.color = new Color(0.12f, 0.1f, 0.08f, 1f);
        }
    }

    void CreateBottomVignette()
    {
        var go = CreateImage("BottomVignette", canvas.transform,
            new Vector2(0, 0), new Vector2(1, 0.42f), Vector2.zero, Vector2.zero,
            new Color(0.01f, 0.01f, 0.02f, 0.72f));
        go.GetComponent<Image>().raycastTarget = false;
    }

    void BuildMainPanel()
    {
        mainPanel = CreatePanel("MainPanel", canvas.transform);

        var textBackdrop = CreateImage("TextBackdrop", mainPanel.transform,
            new Vector2(0, 0), new Vector2(1, 0.46f), Vector2.zero, Vector2.zero,
            new Color(0.01f, 0.01f, 0.02f, 0.96f));
        textBackdrop.GetComponent<Image>().raycastTarget = false;

        CreateStyledTitle(mainPanel.transform, "Year1975", "1975",
            200, YearRed, new Vector2(0.5f, 0.78f), new Vector2(500, 120));

        CreateStyledTitle(mainPanel.transform, "GameTitle", "NGƯỜI ĐƯA THƯ",
            92, TitleWhite, new Vector2(0.5f, 0.235f), new Vector2(1100, 120), FontStyle.BoldAndItalic);

        CreateLabel(mainPanel.transform, "Tagline", "Mang thư về làng, mang hy vọng về nhà",
            46, TextAnchor.MiddleCenter, new Vector2(0.5f, 0.168f), new Vector2(980, 68), TextBright, FontStyle.Bold);

        CreatePosterButton(mainPanel.transform, "BtnStart", "BẮT ĐẦU CHƠI", new Vector2(0.5f, 0.068f), () => ShowIntro());
        CreatePosterButton(mainPanel.transform, "BtnSettings", "CÀI ĐẶT ÂM LƯỢNG", new Vector2(0.5f, 0.012f), ShowSettings, small: true);
    }

    void BuildSettingsPanel()
    {
        settingsPanel = CreatePanel("SettingsPanel", canvas.transform);
        settingsPanel.SetActive(false);

        var overlay = CreateImage("SettingsOverlay", settingsPanel.transform,
            Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero, new Color(0, 0, 0, 0.82f));
        overlay.transform.SetAsFirstSibling();
        overlay.GetComponent<Image>().raycastTarget = false;

        CreateLabel(settingsPanel.transform, "SettingsTitle", "CÀI ĐẶT ÂM LƯỢNG",
            52, TextAnchor.MiddleCenter, new Vector2(0.5f, 0.78f), new Vector2(760, 76), Accent, FontStyle.Bold);

        var box = CreateBox(settingsPanel.transform, "SettingsBox", new Vector2(0.5f, 0.48f), new Vector2(620, 340));
        CreateSliderRow(box.transform, "Tổng âm lượng", 0.72f, AudioSettings.Master, v => AudioSettings.Master = v);
        CreateSliderRow(box.transform, "Nhạc nền", 0.52f, AudioSettings.Music, v => AudioSettings.Music = v);
        CreateSliderRow(box.transform, "Hiệu ứng (mưa, gió...)", 0.32f, AudioSettings.Sfx, v => AudioSettings.Sfx = v);

        CreatePosterButton(settingsPanel.transform, "BtnBack", "QUAY LẠI", new Vector2(0.5f, 0.14f), ShowMain, small: true);
    }

    void BuildIntroPanel()
    {
        introPanel = CreatePanel("IntroPanel", canvas.transform);
        introPanel.SetActive(false);

        // Lớp phủ nhẹ — giữ nền poster rõ, không làm mờ chữ
        var overlay = CreateImage("IntroOverlay", introPanel.transform,
            Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero, new Color(0, 0, 0, 0.55f));
        overlay.GetComponent<Image>().raycastTarget = true;

        CreateLabel(introPanel.transform, "IntroChapter", "Người đưa thư",
            52, TextAnchor.MiddleCenter, new Vector2(0.5f, 0.88f), new Vector2(980, 76), Accent, FontStyle.Bold);

        var box = CreateIntroBox(introPanel.transform, "IntroBox", new Vector2(0.5f, 0.52f), new Vector2(980, 520));

        var bodyGo = new GameObject("IntroBody", typeof(RectTransform), typeof(Text));
        bodyGo.transform.SetParent(box.transform, false);
        var bodyRt = bodyGo.GetComponent<RectTransform>();
        bodyRt.anchorMin = Vector2.zero;
        bodyRt.anchorMax = Vector2.one;
        bodyRt.offsetMin = new Vector2(36, 48);
        bodyRt.offsetMax = new Vector2(-36, -36);

        introBody = bodyGo.GetComponent<Text>();
        introBody.font = uiFont;
        introBody.fontSize = 44;
        introBody.fontStyle = FontStyle.Bold;
        introBody.lineSpacing = 1.35f;
        introBody.alignment = TextAnchor.UpperLeft;
        introBody.color = TextBright;
        introBody.horizontalOverflow = HorizontalWrapMode.Wrap;
        introBody.verticalOverflow = VerticalWrapMode.Truncate;
        CrispUiText.ApplyReadableDefaults(introBody);
        CrispUiText.WarmAtlas(44);

        introPageIndicator = CreateLabel(box.transform, "IntroPage", "1 / 2",
            30, TextAnchor.LowerRight, new Vector2(1f, 0f), new Vector2(140, 40), TextBright, FontStyle.Bold);
        introPageIndicator.GetComponent<RectTransform>().anchoredPosition = new Vector2(-24, 16);

        CreatePosterButton(introPanel.transform, "BtnIntroAction", "TIẾP THEO",
            new Vector2(0.5f, 0.06f), IntroAdvanceOrStart);
        var actionGo = introPanel.transform.Find("BtnIntroAction");
        if (actionGo != null)
            introActionLabel = actionGo.Find("Label")?.GetComponent<Text>();

        CreatePosterButton(introPanel.transform, "BtnIntroBack", "QUAY LẠI", new Vector2(0.12f, 0.06f), ShowMain, small: true);
    }

    void IntroAdvanceOrStart()
    {
        if (introVoiceSource != null && introVoiceSource.isPlaying)
        {
            StopIntroVoice();
            RefreshIntroActionLabel();
            return;
        }

        if (introPageIndex < IntroPages.Length - 1)
        {
            introPageIndex++;
            RefreshIntroPage();
            return;
        }
        StartGame();
    }

    void RefreshIntroPage()
    {
        RefreshIntroPageVisuals();
        PlayIntroVoice(introPageIndex);
    }

    void RefreshIntroPageVisuals()
    {
        if (introBody != null)
            introBody.text = IntroPages[introPageIndex];
        if (introPageIndicator != null)
            introPageIndicator.text = $"{introPageIndex + 1} / {IntroPages.Length}";
        RefreshIntroActionLabel();
    }

    void RefreshIntroActionLabel()
    {
        if (introActionLabel == null) return;

        if (introVoiceSource != null && introVoiceSource.isPlaying)
        {
            introActionLabel.text = "ENTER / BẤM ĐỂ BỎ QUA GIỌNG";
            return;
        }

        bool lastPage = introPageIndex >= IntroPages.Length - 1;
        introActionLabel.text = lastPage ? "BẮT ĐẦU HÀNH TRÌNH" : "TIẾP THEO";
    }

    void PlayIntroVoice(int pageIndex)
    {
        StopIntroVoice();
        if (introVoiceSource == null) return;
        if (pageIndex < 0 || pageIndex >= IntroVoiceKeys.Length) return;

        var key = IntroVoiceKeys[pageIndex];
        if (string.IsNullOrWhiteSpace(key))
            return;

        var clip = DialogueAudio.Load(key, refreshFromDisk: true);
        if (clip == null)
        {
            Debug.LogWarning($"[MainMenuUI] Không tìm thấy giọng intro: Audio/Dialogue/{key}");
            return;
        }

        Debug.Log($"[MainMenuUI] Phát giọng intro: {key} ({clip.length:0.0}s)");

        introVoiceCoroutinePage = pageIndex;
        introVoiceCoroutine = StartCoroutine(IntroVoiceRoutine(pageIndex, clip));
    }

    IEnumerator IntroVoiceRoutine(int startPage, AudioClip clip)
    {
        introVoiceSource.clip = clip;
        introVoiceSource.volume = AudioSettings.DialogueVoiceScaled;
        introVoiceSource.Play();

        float waitStart = 1f;
        while (waitStart > 0f && introVoiceSource != null && !introVoiceSource.isPlaying)
        {
            waitStart -= Time.unscaledDeltaTime;
            yield return null;
        }

        RefreshIntroActionLabel();
        if (introVoiceSource == null || !introVoiceSource.isPlaying)
            yield break;

        int pagesCovered = CountIntroPagesCoveredByVoice(startPage);
        int visualPage = startPage;

        while (introVoiceSource != null && introVoiceSource.isPlaying)
        {
            if (pagesCovered > 1 && visualPage < startPage + pagesCovered - 1)
            {
                float switchAt = GetIntroClipSwitchTime(clip, startPage, pagesCovered, visualPage - startPage);
                if (introVoiceSource.time >= switchAt)
                {
                    visualPage++;
                    introPageIndex = visualPage;
                    RefreshIntroPageVisuals();
                }
            }

            yield return null;
        }

        if (introVoiceCoroutinePage != startPage)
            yield break;

        introVoiceCoroutine = null;
        introVoiceCoroutinePage = -1;

        int lastCoveredPage = startPage + pagesCovered - 1;
        if (introPageIndex < lastCoveredPage)
        {
            introPageIndex = lastCoveredPage;
            RefreshIntroPageVisuals();
        }

        if (lastCoveredPage < IntroPages.Length - 1)
        {
            introPageIndex = lastCoveredPage + 1;
            RefreshIntroPage();
            yield break;
        }

        RefreshIntroPageVisuals();
    }

    static int CountIntroPagesCoveredByVoice(int startPage)
    {
        int count = 1;
        for (int page = startPage + 1; page < IntroPages.Length; page++)
        {
            if (!string.IsNullOrWhiteSpace(IntroVoiceKeys[page]))
                break;
            count++;
        }

        return count;
    }

    static float GetIntroClipSwitchTime(AudioClip clip, int startPage, int pagesCovered, int pageOffset)
    {
        int totalChars = 0;
        for (int i = 0; i < pagesCovered; i++)
            totalChars += IntroPages[startPage + i].Length;

        int charsBeforeSwitch = 0;
        for (int i = 0; i <= pageOffset; i++)
            charsBeforeSwitch += IntroPages[startPage + i].Length;

        return clip.length * ((float)charsBeforeSwitch / totalChars);
    }

    void StopIntroVoice()
    {
        if (introVoiceCoroutine != null)
        {
            StopCoroutine(introVoiceCoroutine);
            introVoiceCoroutine = null;
        }

        introVoiceCoroutinePage = -1;
        if (introVoiceSource != null)
        {
            introVoiceSource.Stop();
            introVoiceSource.clip = null;
        }
    }

    public void StopIntroVoiceNow() => StopIntroVoice();

    GameObject CreateIntroBox(Transform parent, string name, Vector2 anchor, Vector2 size)
    {
        var go = CreateImage(name, parent, anchor, anchor, Vector2.zero, size, IntroBoxBg);
        go.GetComponent<Image>().raycastTarget = false;
        return go;
    }

    Text CreateStyledTitle(Transform parent, string name, string text, int size, Color color, Vector2 anchor, Vector2 sizeDelta,
        FontStyle style = FontStyle.Bold)
    {
        var t = CreateLabel(parent, name, text, size, TextAnchor.MiddleCenter, anchor, sizeDelta, color, style);
        t.horizontalOverflow = HorizontalWrapMode.Overflow;
        return t;
    }

    void CreatePosterButton(Transform parent, string name, string label, Vector2 anchor,
        UnityEngine.Events.UnityAction onClick, bool small = false)
    {
        var size = small ? new Vector2(440, 72) : new Vector2(560, 84);
        var go = new GameObject(name, typeof(RectTransform), typeof(Image), typeof(Button));
        go.transform.SetParent(parent, false);
        var rt = go.GetComponent<RectTransform>();
        rt.anchorMin = anchor;
        rt.anchorMax = anchor;
        rt.pivot = anchor;
        rt.sizeDelta = size;

        var img = go.GetComponent<Image>();
        img.color = new Color(0.04f, 0.02f, 0.01f, 1f);

        var btn = go.GetComponent<Button>();
        var colors = btn.colors;
        colors.normalColor = img.color;
        colors.highlightedColor = new Color(0.22f, 0.15f, 0.08f, 0.98f);
        colors.pressedColor = new Color(0.4f, 0.26f, 0.08f, 1f);
        colors.selectedColor = colors.highlightedColor;
        btn.colors = colors;
        btn.targetGraphic = img;
        btn.onClick.AddListener(onClick);

        var fontSize = small ? 34 : 40;
        var labelText = CreateLabel(go.transform, "Label", label,
            fontSize, TextAnchor.MiddleCenter, new Vector2(0.5f, 0.5f), size, TextBright, FontStyle.Bold);
        labelText.raycastTarget = false;
    }

    void ShowMain()
    {
        StopIntroVoice();
        if (mainPanel != null) mainPanel.SetActive(true);
        if (settingsPanel != null) settingsPanel.SetActive(false);
        if (introPanel != null) introPanel.SetActive(false);
    }

    public void ReturnToMainScreen()
    {
        introPageIndex = 0;
        ShowMain();
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    void ShowSettings()
    {
        if (mainPanel != null) mainPanel.SetActive(false);
        if (settingsPanel != null) settingsPanel.SetActive(true);
        if (introPanel != null) introPanel.SetActive(false);
    }

    void ShowIntro()
    {
        if (mainPanel != null) mainPanel.SetActive(false);
        if (settingsPanel != null) settingsPanel.SetActive(false);
        if (introPanel != null) introPanel.SetActive(true);
        introPageIndex = 0;
        RefreshIntroPage();
    }

    void StartGame()
    {
        StopIntroVoice();
        VoicePlayback.StopAll();
        DialogueAudio.Preload(refreshFromDisk: true, Chapter1Voice.Transition, Chapter1Voice.IntroHud);
        GameSession.StartedFromMenu = true;
        EnsureSystems();
        GameManager.Instance.StartNewGame();
    }

    void EnsureSystems()
    {
        if (GameManager.Instance == null) new GameObject("GameManager").AddComponent<GameManager>();
        if (SceneTransition.Instance == null) new GameObject("SceneTransition").AddComponent<SceneTransition>();
        if (GameUI.Instance == null) new GameObject("GameUI").AddComponent<GameUI>();
        if (DialogueManager.Instance == null) new GameObject("DialogueManager").AddComponent<DialogueManager>();
    }

    GameObject CreatePanel(string name, Transform parent)
    {
        var go = new GameObject(name, typeof(RectTransform));
        go.transform.SetParent(parent, false);
        var rt = go.GetComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
        return go;
    }

    GameObject CreateBox(Transform parent, string name, Vector2 anchor, Vector2 size)
    {
        return CreateImage(name, parent, anchor, anchor, Vector2.zero, size, PanelBg);
    }

    Text CreateLabel(Transform parent, string name, string text, int size, TextAnchor align,
        Vector2 anchor, Vector2 sizeDelta, Color color, FontStyle style = FontStyle.Normal)
    {
        return CrispUiText.Create(parent, name, text, size, align, anchor, sizeDelta, color, style);
    }

    void CreateSliderRow(Transform parent, string label, float yAnchor, float value, System.Action<float> onChanged)
    {
        float y = (yAnchor - 0.5f) * 280f;
        var lbl = CreateLabel(parent, label + "_lbl", label, 32, TextAnchor.MiddleLeft,
            new Vector2(0, 0.5f), new Vector2(520, 44), TextBright, FontStyle.Bold);
        lbl.GetComponent<RectTransform>().anchoredPosition = new Vector2(30, y);
        if (lbl != null) lbl.raycastTarget = false;

        var sliderGo = new GameObject("Slider_" + label, typeof(RectTransform), typeof(Slider));
        sliderGo.transform.SetParent(parent, false);
        var srt = sliderGo.GetComponent<RectTransform>();
        srt.anchorMin = new Vector2(0, 0.5f);
        srt.anchorMax = new Vector2(0, 0.5f);
        srt.pivot = new Vector2(0, 0.5f);
        srt.sizeDelta = new Vector2(480, 36);
        srt.anchoredPosition = new Vector2(30, y - 32);

        var bg = CreateImage("Bg", sliderGo.transform, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero,
            new Color(0.08f, 0.06f, 0.05f, 1f));
        bg.GetComponent<Image>().raycastTarget = true;

        var fillArea = new GameObject("Fill Area", typeof(RectTransform));
        fillArea.transform.SetParent(sliderGo.transform, false);
        var fillAreaRt = fillArea.GetComponent<RectTransform>();
        fillAreaRt.anchorMin = Vector2.zero;
        fillAreaRt.anchorMax = Vector2.one;
        fillAreaRt.offsetMin = new Vector2(10, 10);
        fillAreaRt.offsetMax = new Vector2(-10, -10);

        var fill = CreateImage("Fill", fillArea.transform, Vector2.zero, Vector2.zero, Vector2.zero, Vector2.zero, Accent);
        fill.GetComponent<Image>().raycastTarget = false;
        var fillRt = fill.GetComponent<RectTransform>();
        fillRt.anchorMin = new Vector2(0f, 0f);
        fillRt.anchorMax = new Vector2(0f, 1f);
        fillRt.offsetMin = Vector2.zero;
        fillRt.offsetMax = Vector2.zero;

        var handleArea = new GameObject("Handle Slide Area", typeof(RectTransform));
        handleArea.transform.SetParent(sliderGo.transform, false);
        var handleAreaRt = handleArea.GetComponent<RectTransform>();
        handleAreaRt.anchorMin = Vector2.zero;
        handleAreaRt.anchorMax = Vector2.one;
        handleAreaRt.offsetMin = new Vector2(10, 0);
        handleAreaRt.offsetMax = new Vector2(-10, 0);

        var handle = CreateImage("Handle", handleArea.transform,
            new Vector2(0, 0.5f), new Vector2(0, 0.5f), Vector2.zero, new Vector2(24, 36), TextCream);
        var handleImg = handle.GetComponent<Image>();
        handleImg.raycastTarget = true;

        var slider = sliderGo.GetComponent<Slider>();
        slider.direction = Slider.Direction.LeftToRight;
        slider.fillRect = fillRt;
        slider.handleRect = handle.GetComponent<RectTransform>();
        slider.targetGraphic = handleImg;
        slider.interactable = true;
        slider.minValue = 0f;
        slider.maxValue = 1f;
        slider.wholeNumbers = false;
        slider.value = value;
        slider.onValueChanged.AddListener(v => onChanged(v));
    }

    GameObject CreateImage(string name, Transform parent, Vector2 anchorMin, Vector2 anchorMax,
        Vector2 offsetMin, Vector2 sizeDelta, Color color)
    {
        var go = new GameObject(name, typeof(RectTransform), typeof(Image));
        go.transform.SetParent(parent, false);
        var rt = go.GetComponent<RectTransform>();

        if (anchorMin == anchorMax && sizeDelta != Vector2.zero)
        {
            rt.anchorMin = anchorMin;
            rt.anchorMax = anchorMax;
            rt.pivot = anchorMin;
            rt.sizeDelta = sizeDelta;
            rt.anchoredPosition = offsetMin;
        }
        else
        {
            rt.anchorMin = anchorMin;
            rt.anchorMax = anchorMax;
            rt.offsetMin = offsetMin;
            rt.offsetMax = sizeDelta;
        }

        var img = go.GetComponent<Image>();
        img.color = color;
        img.raycastTarget = false;
        return go;
    }
}
