using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

[RequireComponent(typeof(WeatherController))]
public class RainVideoOverlay : MonoBehaviour
{
    [Header("Video mưa (mp4)")]
    [Tooltip("Kéo file mp4 đã import vào Unity. Nếu để trống sẽ tìm Resources/Weather/RainVideo")]
    public VideoClip rainVideo;
    [Range(0f, 1f)] public float maxOpacity = 0.85f;
    [Tooltip("Giảm particle mưa code khi dùng video (0 = tắt hẳn)")]
    [Range(0f, 1f)] public float particleRainBlend = 0.15f;

    VideoPlayer player;
    RawImage screen;
    Canvas overlayCanvas;
    float currentOpacity;

    public bool IsActive => rainVideo != null && player != null;
    public float ParticleRainMultiplier => IsActive ? particleRainBlend : 1f;

    void Start()
    {
        if (rainVideo == null)
            rainVideo = Resources.Load<VideoClip>("Weather/RainVideo");

        if (rainVideo == null) return;

        BuildOverlay();
    }

    void Update()
    {
        if (!IsActive || WeatherController.Instance == null) return;

        float target = WeatherController.Instance.NormalizedRain * maxOpacity;
        currentOpacity = Mathf.Lerp(currentOpacity, target, Time.deltaTime * 2.5f);

        if (screen != null)
        {
            var c = screen.color;
            c.a = currentOpacity;
            screen.color = c;
        }

        if (player != null && !player.isPlaying && currentOpacity > 0.01f)
            player.Play();
    }

    void BuildOverlay()
    {
        var root = new GameObject("RainVideoOverlay");
        root.transform.SetParent(transform);

        overlayCanvas = root.AddComponent<Canvas>();
        overlayCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        overlayCanvas.sortingOrder = 60;
        overlayCanvas.pixelPerfect = false;

        var scaler = root.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.matchWidthOrHeight = 1f;

        var panel = new GameObject("RainScreen");
        panel.transform.SetParent(root.transform, false);
        screen = panel.AddComponent<RawImage>();
        screen.raycastTarget = false;
        screen.color = new Color(1f, 1f, 1f, 0f);
        screen.material = CreateRainBlendMaterial();

        var rect = screen.rectTransform;
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;

        var rt = new RenderTexture(1920, 1080, 0, RenderTextureFormat.ARGB32);
        rt.Create();

        player = root.AddComponent<VideoPlayer>();
        player.clip = rainVideo;
        player.renderMode = VideoRenderMode.RenderTexture;
        player.targetTexture = rt;
        player.isLooping = true;
        player.playOnAwake = false;
        player.audioOutputMode = VideoAudioOutputMode.None;

        screen.texture = rt;
    }

    static Material CreateRainBlendMaterial()
    {
        var shader = Shader.Find("Legacy Shaders/Particles/Additive")
                     ?? Shader.Find("Mobile/Particles/Additive")
                     ?? Shader.Find("Particles/Additive")
                     ?? Shader.Find("UI/Default");

        return new Material(shader);
    }
}
