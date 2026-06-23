using UnityEngine;

public class MailPickupInteractable : Interactable
{
    public string questStepId = "pickup_mail";
    public string recipientName = "Bà Lan - Làng Bình An";
    public string senderName = "Trạm Liên Lạc";
    [TextArea] public string mailSummary = "Thư từ con trai ở tiền tuyến...";

    [TextArea] public string pickupDialogue =
        "Nam ơi, mang túi thư này đến làng Bình An giúp cô.";

    public override bool CanInteract()
    {
        return QuestManager.Instance != null &&
               QuestManager.Instance.IsStepActive(questStepId) &&
               (MailInventory.Instance == null || !MailInventory.Instance.HasMail);
    }

    public override void Interact()
    {
        DialogueManager.Instance?.ShowDialogue("Trạm Liên Lạc", pickupDialogue, () =>
        {
            MailInventory.Instance?.ReceiveMail(recipientName, senderName, mailSummary);
            QuestManager.Instance?.CompleteStep(questStepId);
        });
    }
}
