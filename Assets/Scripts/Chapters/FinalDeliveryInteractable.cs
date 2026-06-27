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

        string voiceKey = delivered switch
        {
            1 => Chapter3Voice.FlashbackSoldier,
            2 => Chapter3Voice.FlashbackBaLan,
            _ => Chapter3Voice.FlashbackBrother
        };

        DialogueManager.Instance?.ShowDialogue("Giao thư", flashback, voiceKey, () =>
        {
            if (delivered >= 3)
                QuestManager.Instance?.CompleteStep("final_delivery");
            else
                GameUI.Instance?.ShowNotification($"Đã giao {delivered}/3 lá thư");
        });
    }
}
