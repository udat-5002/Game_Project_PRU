using UnityEngine;

public class SoldierLetterInteractable : Interactable
{
    public override bool CanInteract() =>
        QuestManager.Instance != null && QuestManager.Instance.IsStepActive("receive_letter");

    public override void Interact()
    {
        DialogueManager.Instance?.ShowDialogue("Người lính trẻ", Chapter2Dialogue.Soldier, Chapter2Voice.Soldier, () =>
        {
            DialogueManager.Instance?.ShowDialogue("Nam", Chapter2Dialogue.SoldierNam, Chapter2Voice.SoldierNam, () =>
            {
                DialogueManager.Instance?.ShowDialogue("Nam", Chapter2Dialogue.StealthNam, Chapter2Voice.StealthNam, () =>
                {
                    MailInventory.Instance?.ReceiveMail("Mẹ anh lính", "Người lính trẻ", "Thư từ tiền tuyến...");
                    QuestManager.Instance?.CompleteStep("receive_letter");
                    GameUI.Instance?.ShowNotification(Chapter2Dialogue.CheckMapPrompt, 5f, CrispUiText.Gold);
                });
            });
        });
    }
}
