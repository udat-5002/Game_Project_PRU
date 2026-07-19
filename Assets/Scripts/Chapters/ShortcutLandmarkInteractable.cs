using UnityEngine;

/// <summary>
/// Mốc lối tắt Chương 1 — đi đúng thứ tự theo lời cụ già.
/// </summary>
public class ShortcutLandmarkInteractable : Interactable
{
    public string landmarkId;
    public string landmarkTitle;
    public string confirmLine;
    public string voiceKey;

    public override bool CanInteract()
    {
        var flow = ChapterFlowController.Active;
        if (flow == null || QuestManager.Instance == null) return false;
        if (!QuestManager.Instance.IsStepActive("find_shortcut")) return false;
        return flow.IsNextShortcutLandmark(landmarkId);
    }

    public override void Interact()
    {
        var flow = ChapterFlowController.Active;
        if (flow == null) return;

        string body = string.IsNullOrEmpty(confirmLine)
            ? $"Nam nhận ra mốc: {landmarkTitle}."
            : confirmLine;

        DialogueManager.Instance?.ShowDialogue("Nam", body, voiceKey, () =>
            flow.TryVisitShortcutLandmark(landmarkId));
    }
}
