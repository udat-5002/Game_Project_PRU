using UnityEngine;

public class FamilyRecipient : Interactable
{
    public string recipientId;
    public string recipientName;
    [TextArea] public string initialDialogue;
    [TextArea] public string flashbackDialogue;

    public static int DeliveredCount { get; set; } = 0;
    bool alreadyDelivered = false;

    void Start()
    {
        promptText = $"Nhấn E - Giao thư cho {recipientName}";
    }

    public override bool CanInteract()
    {
        return !alreadyDelivered && QuestManager.Instance != null && QuestManager.Instance.IsStepActive("final_delivery");
    }

    public override void Interact()
    {
        if (alreadyDelivered) return;
        alreadyDelivered = true;
        DeliveredCount++;

        DialogueManager.Instance?.ShowDialogue(recipientName, initialDialogue, () =>
        {
            DialogueManager.Instance?.ShowDialogue("Hồi tưởng", flashbackDialogue, () =>
            {
                if (DeliveredCount >= 3)
                {
                    QuestManager.Instance?.CompleteStep("final_delivery");
                }
                else
                {
                    GameUI.Instance?.ShowNotification($"Đã giao {DeliveredCount}/3 lá thư");
                }
                gameObject.SetActive(false);
            });
        });
    }

    public static void ResetCounter()
    {
        DeliveredCount = 0;
    }
}
