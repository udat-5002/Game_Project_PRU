using UnityEngine;

public class ElderGuideInteractable : Interactable
{
    public override bool CanInteract() =>
        QuestManager.Instance != null && QuestManager.Instance.IsStepActive("ask_elder");

    public override void Interact()
    {
        // ct1-3: dặn xem bản đồ — đúng vai cụ già, không còn hiện ở Trạm.
        DialogueManager.Instance?.ShowDialogue("Cụ già", Chapter1Dialogue.ElderMapAdvice, Chapter1Voice.PickupStation, () =>
        {
            DialogueManager.Instance?.ShowDialogue("Cụ già", Chapter1Dialogue.Elder, Chapter1Voice.Elder, () =>
            {
                DialogueManager.Instance?.ShowDialogue("Nam", Chapter1Dialogue.ElderNam, Chapter1Voice.ElderNam, () =>
                {
                    QuestManager.Instance?.CompleteStep("ask_elder");
                    GameUI.Instance?.ShowNotification(
                        "Theo lời cụ: tìm ngã ba cây đa, rồi tới giếng hoang.", 4.5f, CrispUiText.Gold);
                });
            });
        });
    }
}
