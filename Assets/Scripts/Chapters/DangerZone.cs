using UnityEngine;

/// <summary>
/// Vùng pháo kích / bom — đứng lâu sẽ bị đẩy về checkpoint.
/// </summary>
public class DangerZone : MonoBehaviour
{
    public float exposureLimit = 2.2f;
    public Vector3 resetPoint;
    public string activeQuestId = "cross_danger";

    float exposure;
    bool warned;

    void OnTriggerStay(Collider other)
    {
        if (!IsPlayer(other)) return;
        if (QuestManager.Instance == null || !QuestManager.Instance.IsStepActive(activeQuestId))
            return;

        exposure += Time.deltaTime;

        if (!warned && exposure > exposureLimit * 0.45f)
        {
            warned = true;
            GameUI.Instance?.ShowNotification("⚠ Khu vực nguy hiểm! Rút lui ngay!", 2.5f);
        }

        if (exposure >= exposureLimit)
        {
            exposure = 0f;
            warned = false;
            GameUI.Instance?.ShowNotification("Pháo kích gần! Nam bị cuốn về chỗ an toàn.", 3f);
            GameManager.Instance?.TeleportPlayer(resetPoint);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (!IsPlayer(other)) return;
        exposure = 0f;
        warned = false;
    }

    static bool IsPlayer(Collider other) =>
        other.GetComponent<ThirdPersonController>() != null ||
        other.GetComponentInParent<ThirdPersonController>() != null;
}
