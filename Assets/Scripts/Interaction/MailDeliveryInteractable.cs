using UnityEngine;

public class MailDeliveryInteractable : Interactable
{
    public string questStepId = "deliver_mail";
    public string recipientName = "Bà Lan - Làng Bình An";

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

        DialogueManager.Instance?.ShowDialogue("Bà Lan", Chapter1Dialogue.DeliverBaLan1, Chapter1Voice.DeliverBaLan1, () =>
        {
            DialogueManager.Instance?.ShowDialogue("Nam", Chapter1Dialogue.DeliverNam, Chapter1Voice.DeliverNam, () =>
            {
                DialogueManager.Instance?.ShowDialogue("Bà Lan", Chapter1Dialogue.DeliverBaLan2, Chapter1Voice.DeliverBaLan2, () =>
                    QuestManager.Instance?.CompleteStep(questStepId));
            });
        });
    }
}
