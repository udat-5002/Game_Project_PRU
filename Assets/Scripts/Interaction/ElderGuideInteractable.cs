using UnityEngine;

public class ElderGuideInteractable : Interactable
{
    public override bool CanInteract() =>
        QuestManager.Instance != null && QuestManager.Instance.IsStepActive("ask_elder");

    public override void Interact()
    {
        // ct1-5 → ct1-6 → ct1-7
        DialogueManager.Instance?.ShowDialogue("Cụ già", Chapter1Dialogue.ElderMapAdvice, Chapter1Voice.ElderMapAdvice, () =>
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
