using UnityEngine;

public class SoldierLetterInteractable : Interactable
{
    [TextArea] public string dialogue =
        "Nếu tôi không trở về, hãy giúp tôi gửi lá thư này về cho mẹ.";

    public override bool CanInteract() =>
        QuestManager.Instance != null && QuestManager.Instance.IsStepActive("receive_letter");

    public override void Interact()
    {
        DialogueManager.Instance?.ShowDialogue("Người lính trẻ", dialogue, Chapter2Voice.SoldierLetter, () =>
        {
            MailInventory.Instance?.ReceiveMail("Mẹ anh lính", "Người lính trẻ", "Thư từ tiền tuyến...");
            QuestManager.Instance?.CompleteStep("receive_letter");
        });
    }
}
