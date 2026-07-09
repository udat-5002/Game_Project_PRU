using UnityEngine;

public class RainEvent : MonoBehaviour
{
    public string triggerDuringStepId = "stealth_cross";
    public bool rainFromChapterStart = true;
    public bool escalateToStorm = true;
    public string stormNotification = "Mưa bão ập đến! Tiếp tục lẻn qua rừng!";
    public float stormNotificationDuration = 4f;

    bool triggered;

    void Start()
    {
        if (!rainFromChapterStart) return;
        WeatherController.Instance?.EnableChapterRain(instant: true);
    }

    void Update()
    {
        if (triggered || !escalateToStorm || QuestManager.Instance == null) return;
        if (string.IsNullOrEmpty(triggerDuringStepId)) return;
        if (!QuestManager.Instance.IsStepActive(triggerDuringStepId)) return;

        triggered = true;
        WeatherController.Instance?.StartStorm(instant: false);

        if (!string.IsNullOrEmpty(stormNotification))
            GameUI.Instance?.ShowNotification(stormNotification, stormNotificationDuration);
    }
}
