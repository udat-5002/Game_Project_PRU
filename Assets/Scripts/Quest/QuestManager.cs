using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class QuestStep
{
    public string id;
    [TextArea] public string description;
    public bool completed;
}

public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance { get; private set; }

    public List<QuestStep> steps = new List<QuestStep>();
    public int currentStepIndex;

    public event Action OnQuestUpdated;
    public event Action OnAllQuestsCompleted;

    void Awake()
    {
        Instance = this;
    }

    void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    public void SetupQuests(params (string id, string desc)[] questData)
    {
        steps.Clear();
        foreach (var q in questData)
            steps.Add(new QuestStep { id = q.id, description = q.desc });

        currentStepIndex = 0;
        NotifyUpdate();
    }

    public QuestStep CurrentStep =>
        currentStepIndex >= 0 && currentStepIndex < steps.Count ? steps[currentStepIndex] : null;

    public bool IsStepActive(string id) =>
        CurrentStep != null && CurrentStep.id == id && !CurrentStep.completed;

    public void CompleteStep(string id)
    {
        if (CurrentStep == null || CurrentStep.id != id || CurrentStep.completed)
            return;

        CurrentStep.completed = true;
        currentStepIndex++;

        NotifyUpdate();

        if (currentStepIndex >= steps.Count)
            OnAllQuestsCompleted?.Invoke();
    }

    public bool AllCompleted => currentStepIndex >= steps.Count;

    void NotifyUpdate()
    {
        OnQuestUpdated?.Invoke();
        GameUI.Instance?.RefreshQuestUI();
    }
}
