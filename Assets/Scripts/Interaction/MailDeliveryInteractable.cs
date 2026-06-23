using UnityEngine;

public class MailDeliveryInteractable : Interactable
{
    public string questStepId = "deliver_mail";
    public string recipientName = "Bà Lan - Làng Bình An";

    [TextArea] public string deliveryDialogue =
        "Cảm ơn cháu... hy vọng con tôi vẫn bình an.";

    [TextArea] public string wrongRecipientDialogue =
        "Cháu nhầm người rồi. Thư này không phải gửi cho ta.";

    public override bool CanInteract()
    {
        return QuestManager.Instance != null && QuestManager.Instance.IsStepActive(questStepId);
    }

    public override void Interact()
    {
        if (MailInventory.Instance == null || !MailInventory.Instance.HasMail)
        {
            DialogueManager.Instance?.ShowDialogue("???", "Cháu chưa có thư để giao.");
            return;
        }

        if (!MailInventory.Instance.DeliverMail(recipientName))
        {
            DialogueManager.Instance?.ShowDialogue("Người dân", wrongRecipientDialogue);
            return;
        }

        DialogueManager.Instance?.ShowDialogue("Bà Lan", deliveryDialogue, () =>
        {
            QuestManager.Instance?.CompleteStep(questStepId);
        });
    }
}
