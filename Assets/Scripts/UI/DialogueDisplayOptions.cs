using System;
using UnityEngine;

[Serializable]
public struct DialogueDisplayOptions
{
    public bool typewriter;
    public bool autoAdvance;
    public float charsPerSecond;
    public float minAutoPause;
    public float maxAutoPause;
    public bool allowSkip;
    public string voiceKey;

    public static DialogueDisplayOptions AutoPlay => new DialogueDisplayOptions
    {
        typewriter = true,
        autoAdvance = true,
        charsPerSecond = 34f,
        minAutoPause = 1.8f,
        maxAutoPause = 6f,
        allowSkip = true,
        voiceKey = null
    };

    public static DialogueDisplayOptions Manual => new DialogueDisplayOptions
    {
        typewriter = false,
        autoAdvance = false,
        charsPerSecond = 34f,
        minAutoPause = 0f,
        maxAutoPause = 0f,
        allowSkip = true,
        voiceKey = null
    };
}
