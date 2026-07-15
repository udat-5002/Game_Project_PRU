using UnityEngine;

public class MailPickupInteractable : Interactable
{
    public string questStepId = "pickup_mail";
    public string recipientName = "Bà Lan - Làng Bình An";
    public string senderName = "Trạm Liên Lạc";
    [TextArea] public string mailSummary = "Thư từ con trai ở tiền tuyến...";

    public override bool CanInteract()
    {
        return QuestManager.Instance != null &&
               QuestManager.Instance.IsStepActive(questStepId) &&
               (MailInventory.Instance == null || !MailInventory.Instance.HasMail);
    }

    public override void Interact()
    {
        DialogueManager.Instance?.ShowDialogue("Trạm Liên Lạc", Chapter1Dialogue.PickupStation, Chapter1Voice.PickupStation, () =>
        {
            DialogueManager.Instance?.ShowDialogue("Nam", Chapter1Dialogue.PickupNam, Chapter1Voice.PickupNam, () =>
            {
                MailInventory.Instance?.ReceiveMail(recipientName, senderName, mailSummary);
                QuestManager.Instance?.CompleteStep(questStepId);
                GameUI.Instance?.ShowNotification(Chapter1Dialogue.CheckMapPrompt, 5f, CrispUiText.Gold);
            });
        });
    }
}
