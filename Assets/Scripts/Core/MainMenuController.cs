using UnityEngine;

public class MainMenuController : MonoBehaviour
{
    void Awake()
    {
        if (GetComponent<MainMenuUI>() == null)
            gameObject.AddComponent<MainMenuUI>();
    }
}
