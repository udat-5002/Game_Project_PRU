using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Bật phát hiện lính tuần tra chỉ ở nhiệm vụ lẻn — không bắt khi tìm manh mối / giao thư.
/// </summary>
public class PatrolRelocator : MonoBehaviour
{
    readonly List<StealthEnemy> patrols = new List<StealthEnemy>();

    public void Register(StealthEnemy patrol) => patrols.Add(patrol);

    void OnEnable()
    {
        if (QuestManager.Instance != null)
            QuestManager.Instance.OnQuestUpdated += OnQuestChanged;
    }

    void OnDisable()
    {
        if (QuestManager.Instance != null)
            QuestManager.Instance.OnQuestUpdated -= OnQuestChanged;
    }

    void Start() => OnQuestChanged();

    void OnQuestChanged()
    {
        if (QuestManager.Instance == null) return;

        var step = QuestManager.Instance.CurrentStep;
        string activeId = step != null ? step.id : "";
        bool detectPlayer = IsStealthDetectQuest(activeId);

        foreach (var patrol in patrols)
        {
            if (patrol == null) continue;
            bool shouldDetect = detectPlayer && patrol.activeQuestId == activeId;
            patrol.SetPatrolActive(shouldDetect);
        }
    }

    static bool IsStealthDetectQuest(string questId) =>
        questId is "sneak_patrol" or "stealth_cross";
}
