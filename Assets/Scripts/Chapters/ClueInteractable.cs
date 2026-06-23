using System;
using UnityEngine;

public class ClueInteractable : Interactable
{
    public string clueTitle;
    [TextArea] public string clueHint;
    public Action onClueFound;
    bool found;

    public override bool CanInteract() =>
        !found && QuestManager.Instance != null && QuestManager.Instance.IsStepActive("find_clues");

    public override void Interact()
    {
        if (found) return;
        found = true;

        DialogueManager.Instance?.ShowDialogue(clueTitle, clueHint, () =>
        {
            onClueFound?.Invoke();
            gameObject.SetActive(false);
        });
    }
}
