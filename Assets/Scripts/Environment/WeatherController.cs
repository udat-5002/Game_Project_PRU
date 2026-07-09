using System.Collections.Generic;
using UnityEngine;

public class WeatherController : MonoBehaviour
{
    public static WeatherController Instance { get; private set; }

    [Header("Cấu hình")]
    public WeatherPreset preset = WeatherPreset.Overcast;
    public bool stormOnStart;

    [Header("Hiệu ứng mưa")]
    [Tooltip("Kéo prefab mưa vào đây. Nếu để trống sẽ tự tìm Resources/Weather/RainEffect")]
    public GameObject rainPrefab;
    [Tooltip("Nhân cường độ mưa khi dùng prefab (1 = như trong prefab)")]
    public float rainPrefabIntensityScale = 1f;

    const float RainEmitterSize = 200f;
    const float RainEmitterHeight = 28f;
    const int RainMaxParticles = 18000;
    const float DebrisEmitterSize = 180f;
    const float StormRainRate = 7000f;

    Light sunLight;
    ParticleSystem rainSystem;
    readonly List<ParticleSystem> rainPrefabSystems = new();
    readonly List<float> rainPrefabBaseRates = new();
    GameObject rainPrefabInstance;
    ParticleSystem windDebrisSystem;
    RainVideoOverlay rainVideoOverlay;
    WindZone windZone;
    Transform rainFollowTarget;

    WeatherPreset activePreset;
    float targetRainRate;
    float currentRainRate;
    float targetWind;
    float currentWind;
    float targetLightIntensity;
    float currentLightIntensity;
    float targetFogDensity;
    float currentFogDensity;

    Color targetFogColor;
    Color currentFogColor;
    Color targetLightColor;
    Color currentLightColor;

    bool stormActive;
    float lightningTimer;

    public bool IsStormActive => stormActive;
    public WeatherPreset CurrentPreset => activePreset;
    public float NormalizedRain => Mathf.Clamp01(currentRainRate / 9000f);
    public float NormalizedWind => Mathf.Clamp01(currentWind / 1.4f);

    public enum WeatherPreset
    {
        Overcast,       // Ch1: trời u ám, gió nhẹ
        DarkForest,     // Ch2: rừng đêm, âm u
        Storm,          // Ch2: mưa bão
        Battlefield     // Ch3: chiến trường, sương + mưa lất phất
    }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    void Start()
    {
        sunLight = FindSunLight();
        ApplyPreset(preset, instant: true);

        if (stormOnStart)
            StartStorm(instant: true);

        CreateWindZone();
        CreateRainParticles();
        CreateWindDebris();

        if (GetComponent<RainVideoOverlay>() == null)
            rainVideoOverlay = gameObject.AddComponent<RainVideoOverlay>();
        else
            rainVideoOverlay = GetComponent<RainVideoOverlay>();

        if (GetComponent<WeatherAudio>() == null)
            gameObject.AddComponent<WeatherAudio>();
    }

    void LateUpdate()
    {
        if (rainFollowTarget == null)
        {
            GameManager.Instance?.FindPlayer();
            rainFollowTarget = GameManager.Instance?.player;
        }

        if (rainFollowTarget != null)
        {
            var rainPos = rainFollowTarget.position + Vector3.up * RainEmitterHeight;
            if (rainPrefabInstance != null)
                rainPrefabInstance.transform.position = rainPos;
            else if (rainSystem != null)
                rainSystem.transform.position = rainPos;

            if (windDebrisSystem != null)
                windDebrisSystem.transform.position = rainFollowTarget.position + Vector3.up * 12f;
        }

        SmoothWeatherTransition();

        if (stormActive && sunLight != null)
            UpdateLightning();
    }

    void UpdateLightning()
    {
        lightningTimer -= Time.deltaTime;
        if (lightningTimer > 0f) return;

        lightningTimer = Random.Range(4f, 9f);
        StartCoroutine(LightningFlash());
    }

    System.Collections.IEnumerator LightningFlash()
    {
        if (sunLight == null) yield break;
        float original = sunLight.intensity;
        sunLight.intensity = Mathf.Max(original, 0.85f);
        sunLight.color = new Color(0.85f, 0.88f, 1f);
        yield return new WaitForSeconds(0.08f);
        sunLight.intensity = original * 0.5f;
        yield return new WaitForSeconds(0.05f);
        sunLight.intensity = original;
        sunLight.color = targetLightColor;
    }

    public static WeatherController Create(WeatherPreset preset, bool storm = false)
    {
        if (Instance != null)
        {
            Instance.preset = preset;
            Instance.stormOnStart = storm;
            Instance.ApplyPreset(preset, true);
            if (storm) Instance.StartStorm(true);
            return Instance;
        }

        var go = new GameObject("WeatherController");
        var w = go.AddComponent<WeatherController>();
        w.preset = preset;
        w.stormOnStart = storm;
        return w;
    }

    public void ApplyPreset(WeatherPreset p, bool instant = false)
    {
        activePreset = p;
        switch (p)
        {
            case WeatherPreset.Overcast:
                SetTargets(fog: 0.003f, fogColor: new Color(0.7f, 0.73f, 0.76f),
                    light: 0.88f, lightColor: new Color(0.88f, 0.91f, 0.96f),
                    rain: 0f, wind: 0.35f);
                break;
            case WeatherPreset.DarkForest:
                SetTargets(fog: 0.008f, fogColor: new Color(0.4f, 0.44f, 0.5f),
                    light: 0.62f, lightColor: new Color(0.78f, 0.84f, 0.92f),
                    rain: 2200f, wind: 0.7f);
                break;
            case WeatherPreset.Storm:
                SetTargets(fog: 0.012f, fogColor: new Color(0.34f, 0.38f, 0.44f),
                    light: 0.5f, lightColor: new Color(0.74f, 0.8f, 0.9f),
                    rain: 7000f, wind: 1.4f);
                stormActive = true;
                break;
            case WeatherPreset.Battlefield:
                SetTargets(fog: 0.006f, fogColor: new Color(0.5f, 0.46f, 0.44f),
                    light: 0.78f, lightColor: new Color(0.9f, 0.84f, 0.78f),
                    rain: 2200f, wind: 0.75f);
                break;
        }

        RenderSettings.fog = true;
        RenderSettings.fogMode = FogMode.ExponentialSquared;

        if (instant)
        {
            currentRainRate = targetRainRate;
            currentWind = targetWind;
            currentLightIntensity = targetLightIntensity;
            currentFogDensity = targetFogDensity;
            currentFogColor = targetFogColor;
            currentLightColor = targetLightColor;
            ApplyImmediate();
        }
    }

    public void StartStorm(bool instant = false)
    {
        stormActive = true;
        ApplyPreset(WeatherPreset.Storm, instant);
        GameUI.Instance?.ShowNotification("Mưa giông! Gió mạnh! Tìm chỗ trú ngay!");
    }

    public void EndStorm()
    {
        stormActive = false;
        ApplyPreset(WeatherPreset.DarkForest, instant: false);
    }

    void SetTargets(float fog, Color fogColor, float light, Color lightColor, float rain, float wind)
    {
        targetFogDensity = fog;
        targetFogColor = fogColor;
        targetLightIntensity = light;
        targetLightColor = lightColor;
        targetRainRate = rain;
        targetWind = wind;
    }

    void SmoothWeatherTransition()
    {
        float dt = Time.deltaTime * 0.35f;
        currentRainRate = Mathf.Lerp(currentRainRate, targetRainRate, dt);
        currentWind = Mathf.Lerp(currentWind, targetWind, dt);
        currentLightIntensity = Mathf.Lerp(currentLightIntensity, targetLightIntensity, dt);
        currentFogDensity = Mathf.Lerp(currentFogDensity, targetFogDensity, dt);
        currentFogColor = Color.Lerp(currentFogColor, targetFogColor, dt);
        currentLightColor = Color.Lerp(currentLightColor, targetLightColor, dt);
        ApplyImmediate();
    }

    void ApplyImmediate()
    {
        RenderSettings.fogDensity = currentFogDensity;
        RenderSettings.fogColor = currentFogColor;
        RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Trilight;

        // Ambient trung tính — tránh nhuốm xanh toàn scene (kể cả nhân vật)
        float ambientScale = Mathf.Lerp(0.65f, 0.36f, NormalizedRain);
        var neutralSky = new Color(0.48f, 0.49f, 0.51f);
        var neutralEquator = new Color(0.38f, 0.39f, 0.4f);
        var neutralGround = new Color(0.28f, 0.28f, 0.29f);
        RenderSettings.ambientSkyColor = Color.Lerp(neutralSky, currentFogColor * 0.35f, NormalizedRain * 0.5f) * ambientScale;
        RenderSettings.ambientEquatorColor = Color.Lerp(neutralEquator, currentFogColor * 0.25f, NormalizedRain * 0.5f) * ambientScale;
        RenderSettings.ambientGroundColor = Color.Lerp(neutralGround, currentFogColor * 0.15f, NormalizedRain * 0.5f) * ambientScale;

        if (sunLight != null)
        {
            sunLight.intensity = currentLightIntensity;
            sunLight.color = currentLightColor;
            if (activePreset == WeatherPreset.DarkForest || activePreset == WeatherPreset.Storm)
                sunLight.transform.rotation = Quaternion.Euler(12f, 160f, 0f);
        }

        ApplyRainIntensity();

        if (windZone != null)
            windZone.windMain = currentWind;

        if (windDebrisSystem != null)
        {
            var emission = windDebrisSystem.emission;
            emission.rateOverTime = currentWind * 12f;
        }
    }

    Light FindSunLight()
    {
        var lights = FindObjectsByType<Light>(FindObjectsSortMode.None);
        foreach (var l in lights)
        {
            if (l.type == LightType.Directional)
                return l;
        }
        return null;
    }

    void CreateWindZone()
    {
        var go = new GameObject("WindZone");
        go.transform.SetParent(transform);
        go.transform.position = Vector3.zero;
        windZone = go.AddComponent<WindZone>();
        windZone.mode = WindZoneMode.Directional;
        windZone.radius = 250f;
        windZone.windMain = currentWind;
        windZone.windTurbulence = 0.8f;
        windZone.windPulseMagnitude = 0.5f;
        windZone.windPulseFrequency = 0.25f;
    }

    void CreateRainParticles()
    {
        var prefab = rainPrefab != null ? rainPrefab : Resources.Load<GameObject>("Weather/RainEffect");
        if (prefab != null)
        {
            CreateRainFromPrefab(prefab);
            return;
        }

        var go = new GameObject("Rain");
        go.transform.SetParent(transform);
        go.transform.position = Vector3.up * 20f;

        rainSystem = go.AddComponent<ParticleSystem>();
        var main = rainSystem.main;
        main.loop = true;
        main.startLifetime = 2.4f;
        main.startSpeed = 20f;
        main.startSize = 0.08f;
        main.maxParticles = RainMaxParticles;
        main.simulationSpace = ParticleSystemSimulationSpace.World;
        main.startColor = new Color(0.75f, 0.8f, 0.9f, 0.65f);
        main.gravityModifier = 1.35f;

        var emission = rainSystem.emission;
        emission.rateOverTime = currentRainRate;

        var shape = rainSystem.shape;
        shape.shapeType = ParticleSystemShapeType.Box;
        shape.scale = new Vector3(RainEmitterSize, 1f, RainEmitterSize);

        var velocity = rainSystem.velocityOverLifetime;
        velocity.enabled = true;
        velocity.x = new ParticleSystem.MinMaxCurve(2.5f, 2.5f);
        velocity.y = new ParticleSystem.MinMaxCurve(0f, 0f);
        velocity.z = new ParticleSystem.MinMaxCurve(1f, 1f);

        var collision = rainSystem.collision;
        collision.enabled = true;
        collision.type = ParticleSystemCollisionType.World;
        collision.mode = ParticleSystemCollisionMode.Collision3D;
        collision.collidesWith = LayerMask.GetMask("Ground");
        collision.lifetimeLoss = 0.05f;

        var renderer = go.GetComponent<ParticleSystemRenderer>();
        renderer.renderMode = ParticleSystemRenderMode.Stretch;
        renderer.lengthScale = 0.45f;
        renderer.velocityScale = 0.1f;
        renderer.material = CreateParticleMaterial(new Color(0.8f, 0.85f, 0.95f, 0.6f));
    }

    void CreateRainFromPrefab(GameObject prefab)
    {
        rainPrefabInstance = Instantiate(prefab, transform);
        rainPrefabInstance.name = "Rain (Prefab)";
        rainPrefabInstance.transform.localPosition = Vector3.up * RainEmitterHeight;

        rainPrefabSystems.Clear();
        rainPrefabBaseRates.Clear();

        foreach (var ps in rainPrefabInstance.GetComponentsInChildren<ParticleSystem>(true))
        {
            rainPrefabSystems.Add(ps);
            rainPrefabBaseRates.Add(ps.emission.rateOverTime.constantMax);

            var main = ps.main;
            main.simulationSpace = ParticleSystemSimulationSpace.World;

            var emission = ps.emission;
            emission.rateOverTime = 0f;
        }

        if (rainPrefabSystems.Count > 0)
            rainSystem = rainPrefabSystems[0];
    }

    void ApplyRainIntensity()
    {
        float videoBlend = rainVideoOverlay != null ? rainVideoOverlay.ParticleRainMultiplier : 1f;
        float rainRate = currentRainRate * videoBlend;

        if (rainPrefabSystems.Count > 0)
        {
            float intensity = Mathf.Clamp01(rainRate / StormRainRate) * rainPrefabIntensityScale;
            for (int i = 0; i < rainPrefabSystems.Count; i++)
            {
                var emission = rainPrefabSystems[i].emission;
                emission.rateOverTime = rainPrefabBaseRates[i] * intensity;
            }
            return;
        }

        if (rainSystem != null)
        {
            var emission = rainSystem.emission;
            emission.rateOverTime = rainRate;
        }
    }

    void CreateWindDebris()
    {
        var go = new GameObject("WindDebris");
        go.transform.SetParent(transform);
        go.transform.position = Vector3.up * 8f;

        windDebrisSystem = go.AddComponent<ParticleSystem>();
        var main = windDebrisSystem.main;
        main.loop = true;
        main.startLifetime = 3.5f;
        main.startSpeed = new ParticleSystem.MinMaxCurve(4f, 9f);
        main.startSize = new ParticleSystem.MinMaxCurve(0.02f, 0.08f);
        main.maxParticles = 800;
        main.simulationSpace = ParticleSystemSimulationSpace.World;
        main.startColor = new Color(0.35f, 0.38f, 0.32f, 0.45f);

        var emission = windDebrisSystem.emission;
        emission.rateOverTime = currentWind * 12f;

        var shape = windDebrisSystem.shape;
        shape.shapeType = ParticleSystemShapeType.Box;
        shape.scale = new Vector3(DebrisEmitterSize, 10f, DebrisEmitterSize);

        var velocity = windDebrisSystem.velocityOverLifetime;
        velocity.enabled = true;
        velocity.x = new ParticleSystem.MinMaxCurve(6f, 6f);
        velocity.y = new ParticleSystem.MinMaxCurve(-0.5f, 1.5f);
        velocity.z = new ParticleSystem.MinMaxCurve(0f, 0f);

        var noise = windDebrisSystem.noise;
        noise.enabled = true;
        noise.strength = 0.4f;
        noise.frequency = 0.15f;

        var renderer = go.GetComponent<ParticleSystemRenderer>();
        renderer.material = CreateParticleMaterial(new Color(0.4f, 0.42f, 0.38f, 0.5f));
    }

    Material CreateParticleMaterial(Color color)
    {
        var shader = Shader.Find("Universal Render Pipeline/Particles/Unlit")
                     ?? Shader.Find("Particles/Standard Unlit")
                     ?? Shader.Find("Unlit/Color");

        var mat = new Material(shader);
        mat.color = color;
        return mat;
    }
}
