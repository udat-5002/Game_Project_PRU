using UnityEngine;

public class FinalDeliveryInteractable : Interactable
{
    bool delivered;

    public override bool CanInteract() =>
        QuestManager.Instance != null &&
        QuestManager.Instance.IsStepActive("final_delivery") &&
        !delivered;

    public override void Interact()
    {
        if (delivered) return;
        delivered = true;

        DialogueManager.Instance?.ShowDialogue("Nam", Chapter3Dialogue.DeliverNam, Chapter3Voice.DeliverNam, () =>
        {
            DialogueManager.Instance?.ShowDialogue("Người nhận", Chapter3Dialogue.DeliverRecipient, Chapter3Voice.DeliverRecipient, () =>
                QuestManager.Instance?.CompleteStep("final_delivery"));
        });
    }
}
