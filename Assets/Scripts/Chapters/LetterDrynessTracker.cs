using UnityEngine;

/// <summary>
/// Chương 2 — đứng mưa quá lâu làm thư ẩm; phải trú rồi đi tiếp.
/// </summary>
public class LetterDrynessTracker : MonoBehaviour
{
    public string activeQuestId = "keep_letter_dry";
    public float wetSecondsToFail = 45f;
    public float dryRecoverPerSecond = 1.4f;
    public Vector3 fallbackResetPosition;

    float wetTimer;
    bool warnHalf;
    bool warnNear;
    bool failing;

    public static LetterDrynessTracker Instance { get; private set; }

    void OnEnable() => Instance = this;

    void OnDisable()
    {
        if (Instance == this) Instance = null;
    }

    public static void NotifySheltered()
    {
        if (Instance == null) return;
        Instance.warnHalf = false;
        Instance.warnNear = false;
    }

    void Update()
    {
        if (failing || QuestManager.Instance == null) return;
        if (!QuestManager.Instance.IsStepActive(activeQuestId))
        {
            wetTimer = 0f;
            return;
        }

        if (RainShelter.PlayerIsSheltered)
        {
            wetTimer = Mathf.Max(0f, wetTimer - dryRecoverPerSecond * Time.deltaTime);
            return;
        }

        wetTimer += Time.deltaTime;
        float ratio = wetTimer / Mathf.Max(0.1f, wetSecondsToFail);

        if (!warnHalf && ratio >= 0.45f)
        {
            warnHalf = true;
            GameUI.Instance?.ShowNotification("Thư bắt đầu ẩm... mau tìm chỗ trú!", 3f, CrispUiText.Gold);
        }

        if (!warnNear && ratio >= 0.75f)
        {
            warnNear = true;
            GameUI.Instance?.ShowNotification("⚠ Thư sắp ướt! Trú mưa ngay!", 2.5f, GameUI.PatrolWarningColor);
        }

        if (wetTimer < wetSecondsToFail) return;

        failing = true;
        wetTimer = 0f;
        warnHalf = false;
        warnNear = false;

        Vector3 reset = RainShelter.LastShelterPosition;
        if (reset.sqrMagnitude < 0.01f)
            reset = fallbackResetPosition;

        GameUI.Instance?.ShowNotification("Thư ướt hết! Rút về chỗ trú và thử lại.", 3.5f, GameUI.PatrolWarningColor);
        GameManager.Instance?.TeleportPlayer(GroundSnap.SnapCharacter(reset));
        failing = false;
    }
}
