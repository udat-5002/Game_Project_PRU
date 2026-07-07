using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
    public static GameUI Instance { get; private set; }

    Canvas canvas;
    Text questText;
    Text chapterText;
    Text zoneText;
    Text waypointText;
    GameObject interactPrompt;
    Text interactText;
    GameObject dialoguePanel;
    Text dialogueSpeaker;
    Text dialogueBody;
    Text dialogueHint;
    Text notificationText;
    float notificationTimer;
    bool waypointVisibleBeforeDialogue;
    GameObject hudBackdrop;
    bool hudVisible = true;

    static readonly Color TextBright = Color.white;
    static readonly Color WarningYellow = new Color(1f, 0.88f, 0.12f, 1f);
    static readonly Color HudBg = new Color(0.01f, 0.01f, 0.02f, 0.96f);
    static readonly Color DialogueBg = new Color(0.02f, 0.02f, 0.03f, 0.93f);

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
    }

    void Update()
    {
        if (notificationTimer > 0f)
        {
            notificationTimer -= Time.deltaTime;
            if (notificationTimer <= 0f)
                notificationText.gameObject.SetActive(false);
        }
    }

    void BuildUI()
    {
        canvas = gameObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 100;
        var scaler = gameObject.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.matchWidthOrHeight = 1f;
        gameObject.AddComponent<GraphicRaycaster>();

        CrispUiText.WarmGameplayAtlas();

        hudBackdrop = CreatePanel("HudBackdrop", HudBg, new Vector2(0, 1), new Vector2(0, 1), new Vector2(12, -12), new Vector2(840, 260));

        chapterText = CreateHudLine("Chapter", 38, new Vector2(24, -20), 52);
        zoneText = CreateHudLine("Zone", 30, new Vector2(24, -78), 44);
        zoneText.color = CrispUiText.Gold;
        questText = CreateHudLine("Quest", 34, new Vector2(24, -130), 80);
        questText.lineSpacing = 1.25f;
        waypointText = CreateHudLine("Waypoint", 30, new Vector2(24, -218), 44);
        waypointText.color = CrispUiText.White;

        interactPrompt = CreatePanel("InteractPrompt", new Color(0.02f, 0.02f, 0.03f, 1f), new Vector2(0.5f, 0), new Vector2(0.5f, 0), new Vector2(0, 96), new Vector2(620, 80));
        interactText = CreateAnchoredText("InteractText", 36, Vector2.zero, TextAnchor.MiddleCenter, new Vector2(0.5f, 0), FontStyle.Bold);
        interactText.transform.SetParent(interactPrompt.transform, false);
        interactText.rectTransform.anchorMin = Vector2.zero;
        interactText.rectTransform.anchorMax = Vector2.one;
        interactText.rectTransform.offsetMin = Vector2.zero;
        interactText.rectTransform.offsetMax = Vector2.zero;
        interactPrompt.SetActive(false);

        BuildDialoguePanel();

        notificationText = CreateAnchoredText("Notification", 38, new Vector2(0, -130), TextAnchor.UpperCenter, new Vector2(0.5f, 1), FontStyle.Bold);
        notificationText.rectTransform.sizeDelta = new Vector2(980, 60);
        notificationText.gameObject.SetActive(false);
    }

    void BuildDialoguePanel()
    {
        dialoguePanel = CreatePanel("Dialogue", DialogueBg, new Vector2(0f, 0f), new Vector2(0f, 0f), new Vector2(12f, 28f), new Vector2(840f, 340f));

        dialogueSpeaker = CreatePanelText(dialoguePanel.transform, "Speaker", 36, CrispUiText.Gold, FontStyle.Bold);
        var speakerRt = dialogueSpeaker.rectTransform;
        speakerRt.anchorMin = new Vector2(0f, 1f);
        speakerRt.anchorMax = new Vector2(1f, 1f);
        speakerRt.pivot = new Vector2(0f, 1f);
        speakerRt.offsetMin = new Vector2(12f, -64f);
        speakerRt.offsetMax = new Vector2(-12f, -12f);
        dialogueSpeaker.alignment = TextAnchor.UpperLeft;

        dialogueBody = CreatePanelText(dialoguePanel.transform, "Body", 34, TextBright, FontStyle.Bold);
        var bodyRt = dialogueBody.rectTransform;
        bodyRt.anchorMin = Vector2.zero;
        bodyRt.anchorMax = Vector2.one;
        bodyRt.offsetMin = new Vector2(12f, 48f);
        bodyRt.offsetMax = new Vector2(-12f, -60f);
        dialogueBody.alignment = TextAnchor.UpperLeft;
        dialogueBody.lineSpacing = 1.18f;

        var hint = CreatePanelText(dialoguePanel.transform, "Hint", 28, TextBright, FontStyle.Bold);
        hint.text = "Space / E để tiếp tục";
        dialogueHint = hint;
        var hintRt = hint.rectTransform;
        hintRt.anchorMin = new Vector2(1f, 0f);
        hintRt.anchorMax = new Vector2(1f, 0f);
        hintRt.pivot = new Vector2(1f, 0f);
        hintRt.anchoredPosition = new Vector2(-12f, 12f);
        hintRt.sizeDelta = new Vector2(460f, 36f);
        hint.alignment = TextAnchor.LowerRight;

        dialoguePanel.SetActive(false);
    }

    Text CreateHudLine(string name, int size, Vector2 anchoredPos, float height)
    {
        var t = CreateAnchoredText(name, size, anchoredPos, TextAnchor.UpperLeft, new Vector2(0, 1), FontStyle.Bold);
        t.rectTransform.sizeDelta = new Vector2(780, height);
        return t;
    }

    Text CreatePanelText(Transform parent, string name, int size, Color color, FontStyle style)
    {
        var go = new GameObject(name, typeof(RectTransform), typeof(Text));
        go.transform.SetParent(parent, false);
        var t = go.GetComponent<Text>();
        t.font = CrispUiText.GetFont();
        t.fontSize = size;
        t.fontStyle = style;
        t.color = color;
        t.text = "";
        CrispUiText.ApplyReadableDefaults(t);
        CrispUiText.WarmAtlas(size);
        return t;
    }

    public void SetChapterLabel(string text)
    {
        if (chapterText != null) chapterText.text = text;
    }

    public void SetZoneLabel(string text)
    {
        if (zoneText != null) zoneText.text = text;
    }

    public void SetWaypointHint(string text)
    {
        if (waypointText == null) return;
        waypointText.text = text;
        waypointText.gameObject.SetActive(!string.IsNullOrEmpty(text));
    }

    public void RefreshQuestUI()
    {
        if (QuestManager.Instance == null)
        {
            questText.text = "";
            return;
        }

        var step = QuestManager.Instance.CurrentStep;
        if (step == null)
        {
            questText.text = "✓ Hoàn thành tất cả nhiệm vụ";
            return;
        }

        int total = QuestManager.Instance.steps.Count;
        int current = QuestManager.Instance.currentStepIndex + 1;
        questText.text = $"Nhiệm vụ {current}/{total}: {step.description}";
    }

    public void SetInteractPrompt(bool visible, string text = "")
    {
        interactPrompt.SetActive(visible);
        interactText.text = string.IsNullOrEmpty(text) ? "Nhấn E" : text;
    }

    public void SetHudVisible(bool visible)
    {
        hudVisible = visible;
        if (hudBackdrop != null) hudBackdrop.SetActive(visible);
        if (chapterText != null) chapterText.gameObject.SetActive(visible);
        if (zoneText != null) zoneText.gameObject.SetActive(visible);
        if (questText != null) questText.gameObject.SetActive(visible);
        if (waypointText != null)
            waypointText.gameObject.SetActive(visible && !string.IsNullOrEmpty(waypointText.text));
        if (!visible)
        {
            if (interactPrompt != null) interactPrompt.SetActive(false);
            if (dialoguePanel != null) dialoguePanel.SetActive(false);
            SetWorldLabelsVisible(false);
        }
        else
        {
            SetWorldLabelsVisible(true);
        }
    }

    Coroutine typewriterRoutine;
    bool typewriterComplete = true;

    public bool IsTypewriterComplete => typewriterComplete;

    public void PrepareDialogue(string speaker, string message)
    {
        waypointVisibleBeforeDialogue = waypointText != null && waypointText.gameObject.activeSelf;
        if (waypointText != null) waypointText.gameObject.SetActive(false);
        if (interactPrompt != null) interactPrompt.SetActive(false);

        bool hasSpeaker = !string.IsNullOrWhiteSpace(speaker);
        if (dialogueSpeaker != null)
        {
            dialogueSpeaker.text = hasSpeaker ? speaker : "";
            dialogueSpeaker.gameObject.SetActive(hasSpeaker);
        }

        if (dialogueBody != null)
        {
            dialogueBody.text = "";
            var bodyRt = dialogueBody.rectTransform;
            bodyRt.offsetMax = new Vector2(-12f, hasSpeaker ? -60f : -52f);
        }

        dialoguePanel.SetActive(true);
        SetWorldLabelsVisible(false);
    }

    public void SetDialogueBody(string message)
    {
        if (dialogueBody != null)
            dialogueBody.text = message ?? "";
    }

    public void SetDialogueHint(string hint)
    {
        if (dialogueHint != null)
            dialogueHint.text = hint ?? "";
    }

    public Coroutine StartTypewriter(string message, float charsPerSecond)
    {
        StopTypewriter();
        typewriterComplete = false;
        typewriterRoutine = StartCoroutine(TypewriterRoutine(message, charsPerSecond));
        return typewriterRoutine;
    }

    public void CompleteTypewriter(string message)
    {
        StopTypewriter();
        SetDialogueBody(message);
        typewriterComplete = true;
    }

    void StopTypewriter()
    {
        if (typewriterRoutine != null)
        {
            StopCoroutine(typewriterRoutine);
            typewriterRoutine = null;
        }
    }

    IEnumerator TypewriterRoutine(string message, float charsPerSecond)
    {
        message ??= "";
        SetDialogueBody("");

        if (message.Length == 0)
        {
            typewriterComplete = true;
            yield break;
        }

        float shown = 0f;
        while (shown < message.Length)
        {
            shown += charsPerSecond * Time.deltaTime;
            int count = Mathf.Clamp(Mathf.FloorToInt(shown), 1, message.Length);
            SetDialogueBody(message.Substring(0, count));
            yield return null;
        }

        SetDialogueBody(message);
        typewriterComplete = true;
        typewriterRoutine = null;
    }

    public void ShowDialogue(string speaker, string message)
    {
        PrepareDialogue(speaker, message);
        SetDialogueBody(message);
    }

    public void HideDialogue()
    {
        StopTypewriter();
        typewriterComplete = true;
        dialoguePanel.SetActive(false);
        SetWorldLabelsVisible(true);
        if (waypointText != null && waypointVisibleBeforeDialogue && !string.IsNullOrEmpty(waypointText.text))
            waypointText.gameObject.SetActive(true);
    }

    static void SetWorldLabelsVisible(bool visible)
    {
        foreach (var label in Object.FindObjectsByType<QuestStepLabel>(FindObjectsSortMode.None))
            label.SetForcedHidden(!visible);

        foreach (var npcLabel in Object.FindObjectsByType<NpcHeadLabel>(FindObjectsSortMode.None))
            npcLabel.SetForcedHidden(!visible);

        foreach (var canvas in Object.FindObjectsByType<Canvas>(FindObjectsSortMode.None))
        {
            if (canvas.renderMode != RenderMode.WorldSpace) continue;
            var n = canvas.gameObject.name;
            if (n == "ObjectiveLabel" || n == "Label")
                canvas.gameObject.SetActive(visible);
        }
    }

    public void ShowNotification(string msg, float duration = 3f) =>
        ShowNotification(msg, duration, TextBright);

    public void ShowNotification(string msg, float duration, Color color)
    {
        notificationText.text = msg;
        notificationText.color = color;
        notificationText.gameObject.SetActive(true);
        notificationTimer = duration;
    }

    public static Color PatrolWarningColor => WarningYellow;

    public void ShowLetter(string title, string body, System.Action onClose) =>
        ShowLetter(title, body, null, onClose);

    public void ShowLetter(string title, string body, string voiceKey, System.Action onClose)
    {
        StartCoroutine(LetterRoutine(title, body, voiceKey, onClose));
    }

    IEnumerator LetterRoutine(string title, string body, string voiceKey, System.Action onClose)
    {
        GameManager.Instance?.LockInput(true);
        GameMusicController.Instance?.SetMusicDuck(0.22f);

        var panel = CreatePanel("Letter", new Color(0.02f, 0.02f, 0.03f, 1f), Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
        var titleT = CreateAnchoredText("LetterTitle", 44, new Vector2(0, 220), TextAnchor.MiddleCenter, new Vector2(0.5f, 0.5f), FontStyle.Bold);
        titleT.transform.SetParent(panel.transform, false);
        titleT.text = title;
        titleT.rectTransform.sizeDelta = new Vector2(980, 60);

        var bodyT = CreateAnchoredText("LetterBody", 36, Vector2.zero, TextAnchor.MiddleCenter, new Vector2(0.5f, 0.5f), FontStyle.Bold);
        bodyT.transform.SetParent(panel.transform, false);
        bodyT.rectTransform.sizeDelta = new Vector2(980, 440);
        bodyT.lineSpacing = 1.25f;
        bodyT.text = body;

        var hintT = CreateAnchoredText("LetterHint", 30, new Vector2(0, -280), TextAnchor.MiddleCenter, new Vector2(0.5f, 0.5f), FontStyle.Bold);
        hintT.transform.SetParent(panel.transform, false);
        hintT.text = "Space / E để đóng";

        bool voicePlaying = !string.IsNullOrWhiteSpace(voiceKey)
            && DialogueVoicePlayer.Instance != null
            && DialogueVoicePlayer.Instance.Play(voiceKey);

        while (voicePlaying && DialogueVoicePlayer.Instance != null && DialogueVoicePlayer.Instance.IsPlaying)
        {
            if (GameInput.ContinuePressedThisFrame)
            {
                DialogueVoicePlayer.Instance.Stop();
                break;
            }
            yield return null;
        }

        yield return new WaitUntil(() => GameInput.ContinuePressedThisFrame);

        DialogueVoicePlayer.Instance?.Stop();
        GameMusicController.Instance?.SetMusicDuck(1f);
        Destroy(panel);
        GameManager.Instance?.LockInput(false);
        onClose?.Invoke();
    }

    Text CreateAnchoredText(string name, int size, Vector2 anchoredPos, TextAnchor anchor, Vector2 anchorMin,
        FontStyle style = FontStyle.Normal)
    {
        var t = CrispUiText.Create(canvas.transform, name, "", size, anchor, anchorMin, new Vector2(500, 40), TextBright, style);
        t.rectTransform.anchoredPosition = anchoredPos;
        return t;
    }

    GameObject CreatePanel(string name, Color color, Vector2 anchorMin, Vector2 anchorMax, Vector2 offsetMin, Vector2 sizeDelta)
    {
        var go = new GameObject(name, typeof(RectTransform), typeof(Image));
        go.transform.SetParent(canvas.transform, false);
        var rt = go.GetComponent<RectTransform>();
        rt.anchorMin = anchorMin;
        rt.anchorMax = anchorMax;
        if (anchorMin == anchorMax)
        {
            rt.pivot = anchorMin;
            rt.anchoredPosition = offsetMin;
            rt.sizeDelta = sizeDelta;
        }
        else
        {
            rt.offsetMin = offsetMin;
            rt.offsetMax = sizeDelta;
        }
        go.GetComponent<Image>().color = color;
        return go;
    }
}
