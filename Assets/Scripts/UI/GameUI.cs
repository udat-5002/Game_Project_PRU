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
    Text notificationText;
    float notificationTimer;

    static readonly Color TextBright = new Color(1f, 0.98f, 0.94f, 1f);
    static readonly Color HudBg = new Color(0.02f, 0.02f, 0.03f, 0.72f);
    static readonly Color DialogueBg = new Color(0.02f, 0.02f, 0.03f, 0.88f);

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

        CreatePanel("HudBackdrop", HudBg, new Vector2(0, 1), new Vector2(0, 1), new Vector2(12, -12), new Vector2(760, 185));

        chapterText = CreateAnchoredText("Chapter", 32, new Vector2(28, -28), TextAnchor.UpperLeft, new Vector2(0, 1), FontStyle.Bold);
        chapterText.rectTransform.sizeDelta = new Vector2(720, 48);
        zoneText = CreateAnchoredText("Zone", 24, new Vector2(28, -68), TextAnchor.UpperLeft, new Vector2(0, 1), FontStyle.Bold);
        zoneText.rectTransform.sizeDelta = new Vector2(720, 36);
        zoneText.color = new Color(0.85f, 0.75f, 0.35f, 1f);
        questText = CreateAnchoredText("Quest", 28, new Vector2(28, -108), TextAnchor.UpperLeft, new Vector2(0, 1), FontStyle.Bold);
        questText.rectTransform.sizeDelta = new Vector2(720, 76);
        questText.lineSpacing = 1.15f;
        waypointText = CreateAnchoredText("Waypoint", 26, new Vector2(28, -168), TextAnchor.UpperLeft, new Vector2(0, 1), FontStyle.Bold);
        waypointText.rectTransform.sizeDelta = new Vector2(720, 40);
        waypointText.color = new Color(0.55f, 0.9f, 1f, 1f);

        interactPrompt = CreatePanel("InteractPrompt", new Color(0.04f, 0.03f, 0.02f, 0.96f), new Vector2(0.5f, 0), new Vector2(0.5f, 0), new Vector2(0, 90), new Vector2(540, 68));
        interactText = CreateAnchoredText("InteractText", 28, Vector2.zero, TextAnchor.MiddleCenter, new Vector2(0.5f, 0), FontStyle.Bold);
        interactText.transform.SetParent(interactPrompt.transform, false);
        interactText.rectTransform.anchorMin = Vector2.zero;
        interactText.rectTransform.anchorMax = Vector2.one;
        interactText.rectTransform.offsetMin = Vector2.zero;
        interactText.rectTransform.offsetMax = Vector2.zero;
        interactPrompt.SetActive(false);

        dialoguePanel = CreatePanel("Dialogue", DialogueBg, new Vector2(0, 0), new Vector2(1, 0), new Vector2(0, 0), new Vector2(0, 260));
        dialogueSpeaker = CreateAnchoredText("Speaker", 34, new Vector2(36, 200), TextAnchor.UpperLeft, new Vector2(0, 0), FontStyle.Bold);
        dialogueSpeaker.transform.SetParent(dialoguePanel.transform, false);
        dialogueSpeaker.rectTransform.sizeDelta = new Vector2(1680, 48);
        dialogueBody = CreateAnchoredText("Body", 30, new Vector2(36, 138), TextAnchor.UpperLeft, new Vector2(0, 0), FontStyle.Bold);
        dialogueBody.transform.SetParent(dialoguePanel.transform, false);
        dialogueBody.rectTransform.sizeDelta = new Vector2(1680, 130);
        dialogueBody.lineSpacing = 1.2f;
        var hint = CreateAnchoredText("Hint", 24, new Vector2(-36, 30), TextAnchor.LowerRight, new Vector2(1, 0), FontStyle.Bold);
        hint.transform.SetParent(dialoguePanel.transform, false);
        hint.text = "Space / E để tiếp tục";
        hint.rectTransform.sizeDelta = new Vector2(400, 36);
        dialoguePanel.SetActive(false);

        notificationText = CreateAnchoredText("Notification", 30, new Vector2(0, -130), TextAnchor.UpperCenter, new Vector2(0.5f, 1), FontStyle.Bold);
        notificationText.rectTransform.sizeDelta = new Vector2(900, 50);
        notificationText.gameObject.SetActive(false);
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

    public void ShowDialogue(string speaker, string message)
    {
        dialogueSpeaker.text = speaker;
        dialogueBody.text = message;
        dialoguePanel.SetActive(true);
    }

    public void HideDialogue()
    {
        dialoguePanel.SetActive(false);
    }

    public void ShowNotification(string msg, float duration = 3f)
    {
        notificationText.text = msg;
        notificationText.gameObject.SetActive(true);
        notificationTimer = duration;
    }

    public void ShowLetter(string title, string body, System.Action onClose)
    {
        StartCoroutine(LetterRoutine(title, body, onClose));
    }

    IEnumerator LetterRoutine(string title, string body, System.Action onClose)
    {
        GameManager.Instance?.LockInput(true);

        var panel = CreatePanel("Letter", new Color(0.06f, 0.05f, 0.04f, 0.96f), Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
        var titleT = CreateAnchoredText("LetterTitle", 36, new Vector2(0, 220), TextAnchor.MiddleCenter, new Vector2(0.5f, 0.5f), FontStyle.Bold);
        titleT.transform.SetParent(panel.transform, false);
        titleT.text = title;
        titleT.rectTransform.sizeDelta = new Vector2(900, 50);

        var bodyT = CreateAnchoredText("LetterBody", 28, Vector2.zero, TextAnchor.MiddleCenter, new Vector2(0.5f, 0.5f), FontStyle.Bold);
        bodyT.transform.SetParent(panel.transform, false);
        bodyT.rectTransform.sizeDelta = new Vector2(900, 420);
        bodyT.lineSpacing = 1.2f;
        bodyT.text = body;

        var hintT = CreateAnchoredText("LetterHint", 22, new Vector2(0, -280), TextAnchor.MiddleCenter, new Vector2(0.5f, 0.5f), FontStyle.Bold);
        hintT.transform.SetParent(panel.transform, false);
        hintT.text = "Space để đóng";

        yield return new WaitUntil(() => GameInput.SpacePressedThisFrame);

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
