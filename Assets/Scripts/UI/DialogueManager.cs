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
    public bool voiceEnabled = true;
    public float charsPerSecond = 34f;
    public float minAutoPause = 1.8f;
    public float maxAutoPause = 6f;

    public bool IsShowing { get; private set; }

    InputAction continueAction;
    DialogueVoicePlayer voicePlayer;

    void Awake()
    {
        Instance = this;
        continueAction = new InputAction("Continue", binding: "<Keyboard>/space");
        continueAction.AddBinding("<Keyboard>/e");
        continueAction.Enable();

        voicePlayer = GetComponent<DialogueVoicePlayer>();
        if (voicePlayer == null)
            voicePlayer = gameObject.AddComponent<DialogueVoicePlayer>();
    }

    void OnDestroy()
    {
        continueAction?.Disable();
        voicePlayer?.Stop();
        if (Instance == this) Instance = null;
    }

    public DialogueDisplayOptions DefaultOptions => new DialogueDisplayOptions
    {
        typewriter = typewriter,
        autoAdvance = autoAdvance,
        charsPerSecond = charsPerSecond,
        minAutoPause = minAutoPause,
        maxAutoPause = maxAutoPause,
        allowSkip = true,
        voiceKey = null
    };

    public void ShowDialogue(string speaker, string message, Action callback = null) =>
        ShowDialogue(speaker, message, callback, DefaultOptions);

    public void ShowDialogue(string speaker, string message, string voiceKey, Action callback = null)
    {
        var options = DefaultOptions;
        options.voiceKey = voiceKey;
        ShowDialogue(speaker, message, callback, options);
    }

    public void ShowDialogue(string speaker, string message, Action callback, DialogueDisplayOptions options)
    {
        if (IsShowing) return;
        StartCoroutine(DialogueRoutine(speaker, message, callback, options));
    }

    IEnumerator DialogueRoutine(string speaker, string message, Action callback, DialogueDisplayOptions options)
    {
        IsShowing = true;
        GameManager.Instance?.LockInput(true);
        GameMusicController.Instance?.SetMusicDuck(0.22f);
        GameUI.Instance?.PrepareDialogue(speaker, message);

        bool voiceStarted = voiceEnabled && !string.IsNullOrWhiteSpace(options.voiceKey)
            && voicePlayer != null && voicePlayer.Play(options.voiceKey);

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
            float readPause = ComputeAutoPause(message, options, voiceStarted);
            GameUI.Instance?.SetDialogueHint("Space / E để bỏ qua");
            float endTime = Time.time + readPause;
            while (Time.time < endTime || (voiceStarted && voicePlayer != null && voicePlayer.IsPlaying))
            {
                if (options.allowSkip && continueAction.WasPressedThisFrame())
                {
                    voicePlayer?.Stop();
                    break;
                }
                yield return null;
            }
        }
        else
        {
            GameUI.Instance?.SetDialogueHint("Space / E để tiếp tục");
            while (!continueAction.WasPressedThisFrame())
                yield return null;
            voicePlayer?.Stop();
        }

        voicePlayer?.Stop();
        GameMusicController.Instance?.SetMusicDuck(1f);
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
                voicePlayer?.Stop();
                break;
            }
            yield return null;
        }
    }

    static float ComputeAutoPause(string message, DialogueDisplayOptions options, bool voiceStarted)
    {
        int length = string.IsNullOrEmpty(message) ? 0 : message.Length;
        float byLength = length / Mathf.Max(options.charsPerSecond, 1f) * 0.45f + options.minAutoPause;
        float pause = Mathf.Clamp(byLength, options.minAutoPause, options.maxAutoPause);

        if (voiceStarted && DialogueVoicePlayer.Instance != null)
            pause = Mathf.Max(pause, DialogueVoicePlayer.Instance.CurrentClipLength + 0.2f);

        return pause;
    }
}
