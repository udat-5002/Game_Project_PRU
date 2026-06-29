using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Phát file giọng đọc từ Resources/Audio/Dialogue/{voiceKey}.
/// </summary>
[RequireComponent(typeof(AudioSource))]
public class DialogueVoicePlayer : MonoBehaviour
{
    public static DialogueVoicePlayer Instance { get; private set; }

    [Range(0f, 1f)] public float volume = 1f;

    AudioSource source;
    readonly Dictionary<string, AudioClip> cache = new();

    public bool IsPlaying => source != null && source.isPlaying;
    public float CurrentClipLength => source != null && source.clip != null ? source.clip.length : 0f;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }

        Instance = this;
        source = GetComponent<AudioSource>();
        source.playOnAwake = false;
        source.spatialBlend = 0f;
        source.loop = false;
    }

    void OnDestroy()
    {
        Stop();
        if (Instance == this) Instance = null;
    }

    public bool Play(string voiceKey)
    {
        Stop();
        if (string.IsNullOrWhiteSpace(voiceKey))
            return false;

        var clip = LoadClip(voiceKey.Trim());
        if (clip == null)
        {
            Debug.LogWarning($"[DialogueVoice] Không tìm thấy Audio/Dialogue/{voiceKey}");
            return false;
        }

        source.clip = clip;
        source.volume = AudioSettings.DialogueVoiceScaled * volume;
        source.Play();
        return true;
    }

    public void Stop()
    {
        if (source != null && source.isPlaying)
            source.Stop();
        source.clip = null;
    }

    AudioClip LoadClip(string voiceKey)
    {
        if (cache.TryGetValue(voiceKey, out var cached))
            return cached;

        var clip = DialogueAudio.Load(voiceKey);
        if (clip != null)
            cache[voiceKey] = clip;
        return clip;
    }

    public void ClearCache() => cache.Clear();
}
