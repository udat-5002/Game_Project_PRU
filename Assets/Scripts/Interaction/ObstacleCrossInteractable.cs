using UnityEngine;

public class ObstacleCrossInteractable : Interactable
{
    public override bool CanInteract() =>
        QuestManager.Instance != null && QuestManager.Instance.IsStepActive("cross_obstacle");

    public override void Interact()
    {
        DialogueManager.Instance?.ShowDialogue("Nam", Chapter1Dialogue.CrossObstacle, Chapter1Voice.CrossObstacle, () =>
            QuestManager.Instance?.CompleteStep("cross_obstacle"));
    }
}
