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
        DialogueManager.Instance?.ShowDialogue("Mẹ anh lính",
            "Cảm ơn cháu... Ta biết con ta đã hy sinh rồi.", Chapter2Voice.MotherDeliver, () =>
            {
                QuestManager.Instance?.CompleteStep("deliver_mother");
            });
    }
}
