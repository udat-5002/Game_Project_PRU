using UnityEngine;

public class HideSpot : MonoBehaviour
{
    public static bool PlayerIsHidden { get; private set; }

    void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<ThirdPersonController>() || other.GetComponentInParent<ThirdPersonController>())
        {
            PlayerIsHidden = true;
            GameUI.Instance?.ShowNotification("Đang núp... (an toàn)");
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<ThirdPersonController>() || other.GetComponentInParent<ThirdPersonController>())
            PlayerIsHidden = false;
    }
}
