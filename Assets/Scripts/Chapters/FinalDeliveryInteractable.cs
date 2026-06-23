using UnityEngine;

public class FinalDeliveryInteractable : Interactable
{
    int delivered;

    public override bool CanInteract() =>
        QuestManager.Instance != null && QuestManager.Instance.IsStepActive("final_delivery");

    public override void Interact()
    {
        delivered++;
        string flashback = delivered switch
        {
            1 => "Hồi tưởng: Tiếng cười của người lính trước khi lên đường...",
            2 => "Hồi tưởng: Bà Lan đợi con từng ngày...",
            _ => "Hồi tưởng: Anh trai viết thư dưới ánh đèn dầu..."
        };

        DialogueManager.Instance?.ShowDialogue("Giao thư", flashback, () =>
        {
            if (delivered >= 3)
                QuestManager.Instance?.CompleteStep("final_delivery");
            else
                GameUI.Instance?.ShowNotification($"Đã giao {delivered}/3 lá thư");
        });
    }
}
