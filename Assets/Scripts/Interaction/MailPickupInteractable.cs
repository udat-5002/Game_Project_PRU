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
        StartCoroutine(PickupRoutine());
    }

    private System.Collections.IEnumerator PickupRoutine()
    {
        // 1. Lock input so the player cannot move
        GameManager.Instance?.LockInput(true);

        // 2. Play the picking up animation on the player
        var tpc = FindFirstObjectByType<ThirdPersonController>();
        if (tpc != null && tpc.animator != null)
        {
            // Reset movement animation parameters
            tpc.animator.SetFloat("Speed", 0f);
            tpc.animator.SetBool("Jump", false);
            tpc.animator.SetBool("FreeFall", false);
            tpc.animator.SetBool("Grounded", true);

            // Play picking up animation
            tpc.animator.CrossFadeInFixedTime("Picking Up", 0.1f);
        }

        // 3. Wait for the animation to play
        yield return new WaitForSeconds(1.5f);

        if (tpc != null && tpc.animator != null)
        {
            // Smoothly crossfade back to Locomotion (Idle)
            tpc.animator.CrossFadeInFixedTime("Locomotion", 0.2f);
        }

        // 4. Start dialogue flow
        // Trạm chỉ giao thư — không phát thoại chỉ đường (ct1-3 thuộc cụ già).
        DialogueManager.Instance?.ShowDialogue("Trạm Liên Lạc", Chapter1Dialogue.PickupStation, null, () =>
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
