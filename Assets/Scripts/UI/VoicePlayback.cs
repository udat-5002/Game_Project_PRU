using UnityEngine;

/// <summary>
/// Dừng mọi giọng đọc trước khi phát đoạn mới (tránh chồng menu + chương).
/// </summary>
public static class VoicePlayback
{
    public static void StopAll()
    {
        DialogueAudio.StopOtherVoices();

        if (DialogueVoicePlayer.Instance != null)
        {
            DialogueVoicePlayer.Instance.Stop();
            DialogueVoicePlayer.Instance.ClearCache();
        }

        var menu = Object.FindFirstObjectByType<MainMenuUI>();
        menu?.StopIntroVoiceNow();

        SceneTransition.Instance?.StopChapterVoice();
    }
}
