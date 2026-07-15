using UnityEngine;

/// <summary>
/// Hiển thị hướng + khoảng cách tới mục tiêu nhiệm vụ hiện tại trên HUD.
/// Chưa mở bản đồ (check_map) thì không hiện mũi tên — áp dụng cả 3 chương.
/// </summary>
public class QuestNavigator : MonoBehaviour
{
    Transform player;

    void Start()
    {
        var controller = FindFirstObjectByType<ThirdPersonController>();
        if (controller != null) player = controller.transform;
    }

    void LateUpdate()
    {
        if (player == null || QuestManager.Instance == null || GameUI.Instance == null)
            return;

        var step = QuestManager.Instance.CurrentStep;
        if (step == null)
        {
            GameUI.Instance.SetWaypointHint("");
            return;
        }

        if (NeedsMapBeforeArrow(step.id))
        {
            GameUI.Instance.SetWaypointHint(GetLockedHint(step.id));
            return;
        }

        if (!QuestWaypointRegistry.TryGet(step.id, out var target))
        {
            if (step.id == "check_map")
            {
                GameUI.Instance.SetWaypointHint("Tab — mở bản đồ xem đường đi");
                return;
            }

            GameUI.Instance.SetWaypointHint("");
            return;
        }

        var to = target - player.position;
        to.y = 0f;
        float dist = to.magnitude;
        string arrow = GetDirectionArrow(to, player.forward);
        string label = GetShortLabel(step.id);
        string hint = PlayerInteraction.HasActiveTarget ? "  •  Nhấn E" : "";
        GameUI.Instance.SetWaypointHint($"{arrow}  {Mathf.RoundToInt(dist)}m  —  {label}{hint}");
    }

    static bool NeedsMapBeforeArrow(string stepId)
    {
        if (GameManager.Instance == null) return false;
        if (GameSession.MapRouteUnlocked) return false;

        int chapter = GameManager.Instance.CurrentChapter;
        return chapter switch
        {
            1 => stepId is "pickup_mail" or "check_map",
            2 => stepId is "receive_letter" or "check_map",
            3 => stepId is "check_map",
            _ => false
        };
    }

    static string GetLockedHint(string stepId) => stepId switch
    {
        "pickup_mail" => "Tìm Trạm Liên Lạc (theo biển hiệu trên đường)",
        "receive_letter" => "Tìm người lính trẻ (theo biển hiệu trên đường)",
        "check_map" => "Tab — mở bản đồ xem đường đi",
        _ => "Tab — mở bản đồ để xem đường đi"
    };

    static string GetShortLabel(string stepId) => stepId switch
    {
        "pickup_mail" => "Trạm Liên Lạc",
        "check_map" => "Mở bản đồ (Tab)",
        "ask_elder" => "Cụ già",
        "cross_obstacle" => "Khu gỗ đổ",
        "sneak_patrol" => "Vượt tuần tra",
        "deliver_mail" => "Bà Lan",
        "receive_letter" => "Người lính trẻ",
        "stealth_cross" => "Vượt tuần tra",
        "deliver_mother" => "Mẹ người lính",
        "find_clues" => "Manh mối thư",
        "read_brother_letter" => "Đọc thư anh trai",
        "final_delivery" => "Trạm thư cuối",
        _ => "Mục tiêu"
    };

    static string GetDirectionArrow(Vector3 flatOffset, Vector3 forward)
    {
        if (flatOffset.sqrMagnitude < 2f) return "●";

        float angle = Vector3.SignedAngle(forward, flatOffset.normalized, Vector3.up);
        if (angle >= -22.5f && angle < 22.5f) return "▲";
        if (angle >= 22.5f && angle < 67.5f) return "↗";
        if (angle >= 67.5f && angle < 112.5f) return "▶";
        if (angle >= 112.5f && angle < 157.5f) return "↘";
        if (angle >= 157.5f || angle < -157.5f) return "▼";
        if (angle >= -157.5f && angle < -112.5f) return "↙";
        if (angle >= -112.5f && angle < -67.5f) return "◀";
        return "↖";
    }
}
