using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; private set; }

    [Header("Hội thoại mặc định")]
    public bool autoAdvance = true;
    public bool typewriter = true;
    public float charsPerSecond = 34f;
    public float minAutoPause = 1.8f;
    public float maxAutoPause = 6f;

    public bool IsShowing { get; private set; }

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

    public DialogueDisplayOptions DefaultOptions => new DialogueDisplayOptions
    {
        typewriter = typewriter,
        autoAdvance = autoAdvance,
        charsPerSecond = charsPerSecond,
        minAutoPause = minAutoPause,
        maxAutoPause = maxAutoPause,
        allowSkip = true
    };

    public void ShowDialogue(string speaker, string message, Action callback = null) =>
        ShowDialogue(speaker, message, callback, DefaultOptions);

    public void ShowDialogue(string speaker, string message, Action callback, DialogueDisplayOptions options)
    {
        if (IsShowing) return;
        StartCoroutine(DialogueRoutine(speaker, message, callback, options));
    }

    IEnumerator DialogueRoutine(string speaker, string message, Action callback, DialogueDisplayOptions options)
    {
        IsShowing = true;
        GameManager.Instance?.LockInput(true);
        GameUI.Instance?.PrepareDialogue(speaker, message);

        yield return null;
        while (continueAction.IsPressed())
            yield return null;
        yield return null;

        if (options.typewriter)
        {
            GameUI.Instance?.SetDialogueHint("Space / E để hiện hết");
            yield return RunTypewriterWithSkip(message, options);
        }
        else if (GameUI.Instance != null)
        {
            GameUI.Instance.SetDialogueBody(message);
        }

        if (options.autoAdvance)
        {
            float readPause = ComputeAutoPause(message, options);
            GameUI.Instance?.SetDialogueHint("Space / E để bỏ qua");
            float endTime = Time.time + readPause;
            while (Time.time < endTime)
            {
                if (options.allowSkip && continueAction.WasPressedThisFrame())
                    break;
                yield return null;
            }
        }
        else
        {
            GameUI.Instance?.SetDialogueHint("Space / E để tiếp tục");
            yield return new WaitUntil(() => continueAction.WasPressedThisFrame());
        }

        GameUI.Instance?.HideDialogue();
        GameManager.Instance?.LockInput(false);
        IsShowing = false;
        callback?.Invoke();
    }

    IEnumerator RunTypewriterWithSkip(string message, DialogueDisplayOptions options)
    {
        var ui = GameUI.Instance;
        if (ui == null) yield break;

        var typing = ui.StartTypewriter(message, options.charsPerSecond);
        while (typing != null && !ui.IsTypewriterComplete)
        {
            if (options.allowSkip && continueAction.WasPressedThisFrame())
            {
                ui.CompleteTypewriter(message);
                break;
            }
            yield return null;
        }
    }

    static float ComputeAutoPause(string message, DialogueDisplayOptions options)
    {
        int length = string.IsNullOrEmpty(message) ? 0 : message.Length;
        float byLength = length / Mathf.Max(options.charsPerSecond, 1f) * 0.45f + options.minAutoPause;
        return Mathf.Clamp(byLength, options.minAutoPause, options.maxAutoPause);
    }
}
