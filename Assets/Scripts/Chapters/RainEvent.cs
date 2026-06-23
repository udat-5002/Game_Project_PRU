using UnityEngine;

public class RainEvent : MonoBehaviour
{
    public string triggerDuringStepId = "stealth_cross";
    bool triggered;

    void Update()
    {
        if (triggered || QuestManager.Instance == null) return;
        if (!QuestManager.Instance.IsStepActive(triggerDuringStepId)) return;

        triggered = true;
        WeatherController.Instance?.StartStorm(instant: false);
        GameUI.Instance?.ShowNotification("Mưa bão ập đến! Nhanh tìm chỗ trú!", 4f);
    }
}
