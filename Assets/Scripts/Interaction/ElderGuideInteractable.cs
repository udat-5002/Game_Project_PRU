using UnityEngine;

public class ElderGuideInteractable : Interactable
{
    public override bool CanInteract() =>
        QuestManager.Instance != null && QuestManager.Instance.IsStepActive("ask_elder");

    public override void Interact()
    {
        DialogueManager.Instance?.ShowDialogue("Cụ già", Chapter1Dialogue.Elder, Chapter1Voice.Elder, () =>
        {
            DialogueManager.Instance?.ShowDialogue("Nam", Chapter1Dialogue.ElderNam, Chapter1Voice.ElderNam, () =>
                QuestManager.Instance?.CompleteStep("ask_elder"));
        });
    }
}
