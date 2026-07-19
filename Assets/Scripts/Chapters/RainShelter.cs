using UnityEngine;

/// <summary>
/// Chỗ trú mưa — giữ thư khô khi đứng trong vùng trigger.
/// </summary>
public class RainShelter : MonoBehaviour
{
    public static bool PlayerIsSheltered { get; private set; }
    public static Vector3 LastShelterPosition { get; set; }

    [TextArea] public string enterMessage = "Đang trú mưa — thư vẫn khô.";

    void OnTriggerEnter(Collider other)
    {
        if (!IsPlayer(other)) return;

        PlayerIsSheltered = true;
        LastShelterPosition = transform.position;
        LetterDrynessTracker.NotifySheltered();

        if (!string.IsNullOrEmpty(enterMessage))
            GameUI.Instance?.ShowNotification(enterMessage, 2.5f);
    }

    void OnTriggerExit(Collider other)
    {
        if (!IsPlayer(other)) return;
        PlayerIsSheltered = false;
    }

    void OnDisable()
    {
        if (PlayerIsSheltered)
            PlayerIsSheltered = false;
    }

    static bool IsPlayer(Collider other) =>
        other.GetComponent<ThirdPersonController>() != null ||
        other.GetComponentInParent<ThirdPersonController>() != null;
}
