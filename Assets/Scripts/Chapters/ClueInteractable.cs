using UnityEngine;

public class ClueInteractable : Interactable
{
    public string clueId;
    public string clueTitle;
    [TextArea] public string clueHint;
    public string dialogueSpeaker = "Nam";
    public string voiceKey;
    public bool hideAfterCollect = true;

    public override bool CanInteract()
    {
        var flow = ResolveFlow();
        if (flow != null && flow.HasCollectedClue(clueId))
            return false;
        if (!GameSession.MapRouteUnlocked)
            return false;
        return QuestManager.Instance != null && QuestManager.Instance.IsStepActive("find_clues");
    }

    public override void Interact()
    {
        var flow = ResolveFlow();
        if (flow == null || flow.HasCollectedClue(clueId))
            return;
        if (QuestManager.Instance == null || !QuestManager.Instance.IsStepActive("find_clues"))
            return;

        var dm = DialogueManager.Instance;
        if (dm != null && dm.IsShowing)
            return;

        void FinishPresentation()
        {
            if (hideAfterCollect)
                gameObject.SetActive(false);
        }

        void OnCluePresented()
        {
            if (flow.TryRegisterClue(clueId))
                FinishPresentation();
        }

        if (dm == null)
        {
            OnCluePresented();
            return;
        }

        if (!string.IsNullOrWhiteSpace(voiceKey))
        {
            DialogueAudio.Load(voiceKey, refreshFromDisk: true);
            DialogueVoicePlayer.Instance?.ClearCache();
        }

        string speaker = string.IsNullOrWhiteSpace(dialogueSpeaker) ? clueTitle : dialogueSpeaker;
        dm.ShowDialogue(speaker, clueHint, voiceKey, OnCluePresented);
    }

    static ChapterFlowController ResolveFlow() =>
        ChapterFlowController.Active != null
            ? ChapterFlowController.Active
            : Object.FindFirstObjectByType<ChapterFlowController>();
}
