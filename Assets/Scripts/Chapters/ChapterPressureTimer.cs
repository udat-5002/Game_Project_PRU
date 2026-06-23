using UnityEngine;

/// <summary>
/// Áp lực thời gian nhẹ — cảnh báo khi chương kéo dài quá lâu.
/// </summary>
public class ChapterPressureTimer : MonoBehaviour
{
    public int chapter = 1;

    float limitSeconds;
    float elapsed;
    bool warnedHalf;
    bool warnedUrgent;

    void Start()
    {
        float minutes = ChapterDifficulty.TimeLimitMinutes(chapter);
        limitSeconds = minutes > 0f ? minutes * 60f : 0f;
    }

    void Update()
    {
        if (limitSeconds <= 0f || QuestManager.Instance == null || QuestManager.Instance.AllCompleted)
            return;

        elapsed += Time.deltaTime;

        if (!warnedHalf && elapsed >= limitSeconds * 0.55f)
        {
            warnedHalf = true;
            GameUI.Instance?.ShowNotification("⏳ Thời gian trôi nhanh — hoàn thành nhiệm vụ sớm!", 4f);
        }

        if (!warnedUrgent && elapsed >= limitSeconds * 0.85f)
        {
            warnedUrgent = true;
            GameUI.Instance?.ShowNotification("⚠ Gấp rồi! Chiến sự không chờ đợi!", 4f);
        }
    }
}
