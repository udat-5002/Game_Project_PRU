using UnityEngine;

public class ObstacleCrossInteractable : Interactable
{
    [TextArea] public string dialogue =
        "Cây gỗ đổ chắn ngang đường...\n" +
        "Nam len qua từng khúc gỗ, cẩn thận không vấp.";

    public override bool CanInteract() =>
        QuestManager.Instance != null && QuestManager.Instance.IsStepActive("cross_obstacle");

    public override void Interact()
    {
        DialogueManager.Instance?.ShowDialogue("Nam", dialogue, () =>
            QuestManager.Instance?.CompleteStep("cross_obstacle"));
    }
}
