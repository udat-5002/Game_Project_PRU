using UnityEngine;

public static class AudioSettings
{
    const string KeyMaster = "Volume_Master";
    const string KeyMusic = "Volume_Music";
    const string KeySfx = "Volume_Sfx";

    public static float Master
    {
        get => PlayerPrefs.GetFloat(KeyMaster, 0.85f);
        set { PlayerPrefs.SetFloat(KeyMaster, Mathf.Clamp01(value)); Apply(); PlayerPrefs.Save(); }
    }

    public static float Music
    {
        get => PlayerPrefs.GetFloat(KeyMusic, 0.45f);
        set { PlayerPrefs.SetFloat(KeyMusic, Mathf.Clamp01(value)); Apply(); PlayerPrefs.Save(); }
    }

    public static float Sfx
    {
        get => PlayerPrefs.GetFloat(KeySfx, 0.9f);
        set { PlayerPrefs.SetFloat(KeySfx, Mathf.Clamp01(value)); Apply(); PlayerPrefs.Save(); }
    }

    public static void Apply()
    {
        AudioListener.volume = Master;
    }

    public static float MusicScaled => Master * Music;
    public static float SfxScaled => Master * Sfx;
    public static float DialogueVoiceScaled => Mathf.Min(1f, SfxScaled * 1.65f);
}
