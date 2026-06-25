using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameMusicController : MonoBehaviour
{
    public static GameMusicController Instance { get; private set; }

    const string DemoPath = "Audio/demo";
    const string Chapter1Path = "Audio/chương1";
    const string Chapter2Path = "Audio/chương2";
    const string Chapter3Path = "Audio/chương3";
    const string EndPath = "Audio/end";

    [SerializeField] float baseVolume = 0.65f;

    AudioSource source;
    string currentTrackId;
    readonly Dictionary<string, AudioClip> clips = new();

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void EnsureInstance()
    {
        if (Instance != null) return;
        var go = new GameObject("GameMusicController");
        go.AddComponent<GameMusicController>();
    }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        source = gameObject.AddComponent<AudioSource>();
        source.loop = true;
        source.playOnAwake = false;
        source.spatialBlend = 0f;
        source.priority = 0;

        CacheClip("demo", DemoPath);
        CacheClip("ch1", Chapter1Path, "Audio/chuong1");
        CacheClip("ch2", Chapter2Path, "Audio/chuong2");
        CacheClip("ch3", Chapter3Path, "Audio/chuong3");
        CacheClip("end", EndPath);

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDestroy()
    {
        if (Instance == this)
            SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void Start()
    {
        PlayForScene(SceneManager.GetActiveScene().name);
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        PlayForScene(scene.name);
    }

    public void PlayEndingMusic()
    {
        PlayTrack("end");
    }

    void PlayForScene(string sceneName)
    {
        switch (sceneName)
        {
            case GameManager.SceneMainMenu:
                PlayTrack("demo");
                break;
            case GameManager.SceneChapter1:
                PlayTrack("ch1");
                break;
            case GameManager.SceneChapter2:
                PlayTrack("ch2");
                break;
            case GameManager.SceneChapter3:
                PlayTrack("ch3");
                break;
        }
    }

    void CacheClip(string trackId, params string[] resourcePaths)
    {
        foreach (var path in resourcePaths)
        {
            var clip = Resources.Load<AudioClip>(path);
            if (clip == null) continue;
            clips[trackId] = clip;
            return;
        }

        Debug.LogWarning($"GameMusicController: không tìm thấy nhạc cho '{trackId}'.");
    }

    void PlayTrack(string trackId)
    {
        if (currentTrackId == trackId && source != null && source.isPlaying)
            return;

        if (!clips.TryGetValue(trackId, out var clip) || clip == null)
        {
            Debug.LogWarning($"GameMusicController: chưa load được nhạc '{trackId}'.");
            return;
        }

        source.Stop();
        currentTrackId = trackId;
        source.clip = clip;
        source.volume = baseVolume * AudioSettings.MusicScaled;
        source.Play();
    }

    void Update()
    {
        if (source == null || !source.isPlaying) return;
        source.volume = baseVolume * AudioSettings.MusicScaled;
    }
}
