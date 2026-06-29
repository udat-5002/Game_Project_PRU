using UnityEngine;

/// <summary>
/// Chỉ hiện nhãn Canvas khi nhiệm vụ tương ứng đang active.
/// </summary>
public class QuestStepLabel : MonoBehaviour
{
    public string questStepId;

    Canvas canvas;
    bool lastVisible;
    bool forcedHidden;

    void Awake()
    {
        canvas = GetComponent<Canvas>();
        lastVisible = !ShouldShow();
        Apply();
    }

    void OnEnable()
    {
        if (QuestManager.Instance != null)
            QuestManager.Instance.OnQuestUpdated += OnQuestChanged;
        Apply();
    }

    void OnDisable()
    {
        if (QuestManager.Instance != null)
            QuestManager.Instance.OnQuestUpdated -= OnQuestChanged;
    }

    void LateUpdate() => Apply();

    void OnQuestChanged() => Apply();

    public void SetForcedHidden(bool hidden)
    {
        forcedHidden = hidden;
        lastVisible = !ShouldShow();
        Apply();
    }

    void Apply()
    {
        if (canvas == null) canvas = GetComponent<Canvas>();
        if (canvas == null) return;

        bool visible = ShouldShow();
        if (visible == lastVisible) return;
        lastVisible = visible;
        canvas.enabled = visible;
    }

    bool ShouldShow()
    {
        if (forcedHidden) return false;
        return string.IsNullOrEmpty(questStepId)
            || (QuestManager.Instance != null && QuestManager.Instance.IsStepActive(questStepId));
    }
}
