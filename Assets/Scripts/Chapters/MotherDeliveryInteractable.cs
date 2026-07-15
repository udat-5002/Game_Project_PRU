using UnityEngine;

public class MotherDeliveryInteractable : Interactable
{
    public override bool CanInteract() =>
        QuestManager.Instance != null && QuestManager.Instance.IsStepActive("deliver_mother");

    public override void Interact()
    {
        if (MailInventory.Instance == null || !MailInventory.Instance.HasMail)
            return;

        MailInventory.Instance.DeliverMail("Mẹ anh lính");
        DialogueManager.Instance?.ShowDialogue("Mẹ người lính", Chapter2Dialogue.MotherAsk, Chapter2Voice.MotherAsk, () =>
        {
            DialogueManager.Instance?.ShowDialogue("Nam", Chapter2Dialogue.MotherNam, Chapter2Voice.MotherNam, () =>
            {
                DialogueManager.Instance?.ShowDialogue("Mẹ người lính", Chapter2Dialogue.MotherThanks, Chapter2Voice.MotherThanks, () =>
                    QuestManager.Instance?.CompleteStep("deliver_mother"));
            });
        });
    }
}
