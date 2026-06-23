using UnityEngine;

public class Interactable : MonoBehaviour
{
    public string promptText = "Nhấn E để tương tác";

    public virtual bool CanInteract() => true;
    public virtual void Interact() { }

    public string PromptText => promptText;
}
