using UnityEngine;

public class ObstacleCrossInteractable : Interactable
{
    [TextArea] public string dialogue =
        "Cây gỗ đổ chắn ngang đường, Nam vượt qua từng khúc gỗ.";

    public override bool CanInteract() =>
        QuestManager.Instance != null && QuestManager.Instance.IsStepActive("cross_obstacle");

    public override void Interact()
    {
        DialogueManager.Instance?.ShowDialogue("Nam", dialogue, Chapter1Voice.CrossObstacle, () =>
            QuestManager.Instance?.CompleteStep("cross_obstacle"));
    }
}
