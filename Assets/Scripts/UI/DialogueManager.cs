using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; private set; }

    public bool IsShowing { get; private set; }

    Action onComplete;
    InputAction continueAction;

    void Awake()
    {
        Instance = this;
        continueAction = new InputAction("Continue", binding: "<Keyboard>/space");
        continueAction.AddBinding("<Keyboard>/e");
        continueAction.Enable();
    }

    void OnDestroy()
    {
        continueAction?.Disable();
        if (Instance == this) Instance = null;
    }

    public void ShowDialogue(string speaker, string message, Action callback = null)
    {
        if (IsShowing) return;
        StartCoroutine(DialogueRoutine(speaker, message, callback));
    }

    IEnumerator DialogueRoutine(string speaker, string message, Action callback)
    {
        IsShowing = true;
        GameManager.Instance?.LockInput(true);
        GameUI.Instance?.ShowDialogue(speaker, message);

        yield return new WaitUntil(() => continueAction.WasPressedThisFrame());

        GameUI.Instance?.HideDialogue();
        GameManager.Instance?.LockInput(false);
        IsShowing = false;

        callback?.Invoke();
        onComplete = null;
    }
}
