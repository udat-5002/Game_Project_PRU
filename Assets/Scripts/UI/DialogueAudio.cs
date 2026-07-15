using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// Load file giọng từ Resources/Audio/Dialogue/{voiceKey}.
/// Trong Editor: luôn đọc file mới nhất (tránh cache Resources cũ sau khi thay mp3).
/// </summary>
public static class DialogueAudio
{
    const string ResourcesFolder = "Assets/Resources/Audio/Dialogue";

    public static AudioClip Load(string voiceKey, bool refreshFromDisk = false)
    {
        if (string.IsNullOrWhiteSpace(voiceKey))
            return null;

        voiceKey = voiceKey.Trim();

#if UNITY_EDITOR
        if (refreshFromDisk)
            ReimportFromDisk(voiceKey);

        var editorClip = LoadFromAssetDatabase(voiceKey);
        if (editorClip != null)
            return PrepareClip(editorClip);
#endif

        var clip = Resources.Load<AudioClip>($"Audio/Dialogue/{voiceKey}");
        if (clip == null)
            clip = Resources.Load<AudioClip>($"Audio/{voiceKey}");
        if (clip == null)
        {
            foreach (var candidate in Resources.LoadAll<AudioClip>("Audio/Dialogue"))
            {
                if (string.Equals(candidate.name, voiceKey, System.StringComparison.OrdinalIgnoreCase))
                {
                    clip = candidate;
                    break;
                }
            }
        }
        if (clip == null)
        {
            foreach (var candidate in Resources.LoadAll<AudioClip>("Audio"))
            {
                if (string.Equals(candidate.name, voiceKey, System.StringComparison.OrdinalIgnoreCase))
                {
                    clip = candidate;
                    break;
                }
            }
        }

        return PrepareClip(clip);
    }

#if UNITY_EDITOR
    static void ReimportFromDisk(string voiceKey)
    {
        foreach (var path in CandidatePaths(voiceKey))
        {
            if (!System.IO.File.Exists(path))
                continue;

            AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
            return;
        }
    }

    static AudioClip LoadFromAssetDatabase(string voiceKey)
    {
        foreach (var path in CandidatePaths(voiceKey))
        {
            var clip = AssetDatabase.LoadAssetAtPath<AudioClip>(path);
            if (clip != null)
                return clip;
        }

        return null;
    }

    static System.Collections.Generic.IEnumerable<string> CandidatePaths(string voiceKey)
    {
        var keys = new[]
        {
            voiceKey,
            voiceKey.ToLowerInvariant(),
            voiceKey.ToUpperInvariant()
        };

        foreach (var key in keys)
        {
            yield return $"{ResourcesFolder}/{key}.mp3";
            yield return $"{ResourcesFolder}/{key}.wav";
            yield return $"{ResourcesFolder}/{key}.ogg";
            yield return $"Assets/Resources/Audio/{key}.mp3";
            yield return $"Assets/Resources/Audio/{key}.wav";
            yield return $"Assets/Resources/Audio/{key}.ogg";
        }
    }
#endif

    static AudioClip PrepareClip(AudioClip clip)
    {
        if (clip == null)
            return null;

        if (clip.loadState == AudioDataLoadState.Unloaded)
            clip.LoadAudioData();

        return clip;
    }

    public static void Preload(bool refreshFromDisk, params string[] voiceKeys)
    {
        if (voiceKeys == null) return;
        foreach (var key in voiceKeys)
            Load(key, refreshFromDisk);
    }

    public static void Preload(params string[] voiceKeys) => Preload(false, voiceKeys);

    /// <summary>Dừng mọi AudioSource 2D đang phát (trừ nguồn giữ lại và nhạc nền).</summary>
    public static void StopOtherVoices(AudioSource keep = null)
    {
        foreach (var src in Object.FindObjectsByType<AudioSource>(FindObjectsSortMode.None))
        {
            if (src == null || src == keep || !src.isPlaying)
                continue;
            if (src.spatialBlend > 0.01f)
                continue;
            if (src.GetComponent<GameMusicController>() != null)
                continue;
                
            src.Stop();
            src.clip = null;
        }
    }
}
