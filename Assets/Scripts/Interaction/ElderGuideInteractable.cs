using UnityEngine;

public class ElderGuideInteractable : Interactable
{
    [TextArea] public string dialogue =
        "Con đi thẳng qua khu gỗ đổ, rồi sang làng Bình An.\n" +
        "Đường hơi vắng — cẩn thận bom mìn cũ nhé.";

    public override bool CanInteract() =>
        QuestManager.Instance != null && QuestManager.Instance.IsStepActive("ask_elder");

    public override void Interact()
    {
        DialogueManager.Instance?.ShowDialogue("Cụ già", dialogue, () =>
            QuestManager.Instance?.CompleteStep("ask_elder"));
    }
}
