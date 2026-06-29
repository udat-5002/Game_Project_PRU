using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Nhãn tên NPC trên màn hình — giống Trạm Liên Lạc, luôn hiện được.
/// </summary>
public class NpcHeadLabel : MonoBehaviour
{
    public string labelText = "NPC";
    public float headOffset = 3.2f;

    RectTransform panel;
    RectTransform canvasRect;
    bool configured;
    bool forcedHidden;
    int createRetries;

    void Start()
    {
        if (!configured)
            StartCoroutine(BuildWhenReady());
    }

    public void Configure(string text, float height = 3.2f)
    {
        labelText = text;
        headOffset = height;
        configured = true;
        createRetries = 0;
        StopAllCoroutines();
        StartCoroutine(BuildWhenReady());
    }

    public void SetForcedHidden(bool hidden) => forcedHidden = hidden;

    IEnumerator BuildWhenReady()
    {
        yield return null;
        yield return null;

        for (int i = 0; i < 60 && GameUI.Instance == null; i++)
            yield return null;

        foreach (var old in GetComponentsInChildren<Transform>(true))
        {
            if (old.name == "ObjectiveLabel")
                Destroy(old.gameObject);
        }

        if (panel != null)
            Destroy(panel.gameObject);

        ComputeHeadOffset();
        CreateLabel();
    }

    void ComputeHeadOffset()
    {
        var model = transform.Find("NpcModel");
        if (model == null) return;

        float maxLocalY = 0f;
        foreach (var r in model.GetComponentsInChildren<Renderer>())
        {
            var localTop = transform.InverseTransformPoint(r.bounds.max).y;
            maxLocalY = Mathf.Max(maxLocalY, localTop);
        }

        if (maxLocalY > 0.5f)
            headOffset = maxLocalY + 0.35f;
    }

    void CreateLabel()
    {
        if (GameUI.Instance == null) return;

        var canvas = GameUI.Instance.GetComponent<Canvas>();
        if (canvas == null) return;

        canvasRect = canvas.transform as RectTransform;

        var go = new GameObject("NpcHeadLabel_" + labelText);
        go.transform.SetParent(canvas.transform, false);

        panel = go.AddComponent<RectTransform>();
        panel.anchorMin = new Vector2(0.5f, 0.5f);
        panel.anchorMax = new Vector2(0.5f, 0.5f);
        panel.pivot = new Vector2(0.5f, 0.5f);
        panel.sizeDelta = CrispUiText.NameTagSize;
        panel.localScale = Vector3.one;

        var bgGo = new GameObject("Bg", typeof(RectTransform), typeof(Image));
        bgGo.transform.SetParent(panel, false);
        var bgRt = bgGo.GetComponent<RectTransform>();
        bgRt.anchorMin = Vector2.zero;
        bgRt.anchorMax = Vector2.one;
        bgRt.offsetMin = Vector2.zero;
        bgRt.offsetMax = Vector2.zero;
        bgGo.GetComponent<Image>().color = CrispUiText.NameTagBg;

        var textGo = new GameObject("Text", typeof(RectTransform), typeof(Text));
        textGo.transform.SetParent(panel, false);
        var textRt = textGo.GetComponent<RectTransform>();
        textRt.anchorMin = Vector2.zero;
        textRt.anchorMax = Vector2.one;
        textRt.offsetMin = CrispUiText.NameTagTextInsetMin;
        textRt.offsetMax = CrispUiText.NameTagTextInsetMax;

        CrispUiText.ConfigureNameTagText(textGo.GetComponent<Text>(), labelText);

        panel.gameObject.SetActive(true);
    }

    void LateUpdate()
    {
        if (panel == null)
        {
            if (createRetries < 120 && GameUI.Instance != null)
            {
                createRetries++;
                if (createRetries % 30 == 0)
                    CreateLabel();
            }
            return;
        }

        if (!ShouldShow())
        {
            panel.gameObject.SetActive(false);
            return;
        }

        var cam = Camera.main;
        if (cam == null || canvasRect == null)
        {
            panel.gameObject.SetActive(false);
            return;
        }

        var worldPos = transform.position + Vector3.up * headOffset;
        var screen = cam.WorldToScreenPoint(worldPos);
        if (screen.z <= 0f)
        {
            panel.gameObject.SetActive(false);
            return;
        }

        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvasRect, screen, null, out var localPoint))
        {
            panel.gameObject.SetActive(true);
            panel.anchoredPosition = new Vector2(
                Mathf.Round(localPoint.x),
                Mathf.Round(localPoint.y));
        }
        else
        {
            panel.gameObject.SetActive(false);
        }
    }

    bool ShouldShow()
    {
        if (forcedHidden) return false;
        if (DialogueManager.Instance != null && DialogueManager.Instance.IsShowing) return false;
        if (!gameObject.activeInHierarchy) return false;
        return true;
    }

    void OnDestroy()
    {
        if (panel != null)
            Destroy(panel.gameObject);
    }
}
