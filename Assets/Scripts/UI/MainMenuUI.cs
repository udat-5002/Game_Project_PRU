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

        CreateLabel(mainPanel.transform, "Tagline", "Hy vọng tìm đường qua từng lá thư",
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

        CreateLabel(introPanel.transform, "IntroChapter", "Chương 1: Con Đường Hy Vọng",
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
        if (introBody != null)
            introBody.text = IntroPages[introPageIndex];
        if (introPageIndicator != null)
            introPageIndicator.text = $"{introPageIndex + 1} / {IntroPages.Length}";

        bool lastPage = introPageIndex >= IntroPages.Length - 1;
        if (introActionLabel != null)
            introActionLabel.text = lastPage ? "BẮT ĐẦU HÀNH TRÌNH" : "TIẾP THEO";
    }

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
        if (mainPanel != null) mainPanel.SetActive(true);
        if (settingsPanel != null) settingsPanel.SetActive(false);
        if (introPanel != null) introPanel.SetActive(false);
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
