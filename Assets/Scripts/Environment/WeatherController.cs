using UnityEngine;

public class WeatherController : MonoBehaviour
{
    public static WeatherController Instance { get; private set; }

    [Header("Cấu hình")]
    public WeatherPreset preset = WeatherPreset.Overcast;
    public bool stormOnStart;

    Light sunLight;
    ParticleSystem rainSystem;
    ParticleSystem windDebrisSystem;
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
    public float NormalizedRain => Mathf.Clamp01(currentRainRate / 3500f);
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

        if (rainFollowTarget != null && rainSystem != null)
            rainSystem.transform.position = rainFollowTarget.position + Vector3.up * 18f;

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
                SetTargets(fog: 0.006f, fogColor: new Color(0.55f, 0.58f, 0.62f),
                    light: 0.55f, lightColor: new Color(0.72f, 0.76f, 0.82f),
                    rain: 0f, wind: 0.35f);
                break;
            case WeatherPreset.DarkForest:
                SetTargets(fog: 0.022f, fogColor: new Color(0.18f, 0.22f, 0.28f),
                    light: 0.28f, lightColor: new Color(0.55f, 0.62f, 0.75f),
                    rain: 800f, wind: 0.7f);
                break;
            case WeatherPreset.Storm:
                SetTargets(fog: 0.035f, fogColor: new Color(0.12f, 0.14f, 0.18f),
                    light: 0.15f, lightColor: new Color(0.45f, 0.5f, 0.6f),
                    rain: 3500f, wind: 1.4f);
                stormActive = true;
                break;
            case WeatherPreset.Battlefield:
                SetTargets(fog: 0.028f, fogColor: new Color(0.25f, 0.22f, 0.2f),
                    light: 0.32f, lightColor: new Color(0.65f, 0.58f, 0.5f),
                    rain: 1200f, wind: 0.9f);
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
        float ambientScale = Mathf.Lerp(0.55f, 0.28f, NormalizedRain);
        var neutralSky = new Color(0.42f, 0.43f, 0.45f);
        var neutralEquator = new Color(0.32f, 0.33f, 0.34f);
        var neutralGround = new Color(0.22f, 0.22f, 0.23f);
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

        if (rainSystem != null)
        {
            var emission = rainSystem.emission;
            emission.rateOverTime = currentRainRate;
        }

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
        windZone.radius = 200f;
        windZone.windMain = currentWind;
        windZone.windTurbulence = 0.8f;
        windZone.windPulseMagnitude = 0.5f;
        windZone.windPulseFrequency = 0.25f;
    }

    void CreateRainParticles()
    {
        var go = new GameObject("Rain");
        go.transform.SetParent(transform);
        go.transform.position = Vector3.up * 20f;

        rainSystem = go.AddComponent<ParticleSystem>();
        var main = rainSystem.main;
        main.loop = true;
        main.startLifetime = 1.8f;
        main.startSpeed = 18f;
        main.startSize = 0.06f;
        main.maxParticles = 8000;
        main.simulationSpace = ParticleSystemSimulationSpace.World;
        main.startColor = new Color(0.75f, 0.8f, 0.9f, 0.55f);
        main.gravityModifier = 1.2f;

        var emission = rainSystem.emission;
        emission.rateOverTime = currentRainRate;

        var shape = rainSystem.shape;
        shape.shapeType = ParticleSystemShapeType.Box;
        shape.scale = new Vector3(45f, 1f, 45f);

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
        renderer.lengthScale = 0.35f;
        renderer.velocityScale = 0.08f;
        renderer.material = CreateParticleMaterial(new Color(0.8f, 0.85f, 0.95f, 0.6f));
    }

    void CreateWindDebris()
    {
        var go = new GameObject("WindDebris");
        go.transform.SetParent(transform);
        go.transform.position = Vector3.up * 8f;

        windDebrisSystem = go.AddComponent<ParticleSystem>();
        var main = windDebrisSystem.main;
        main.loop = true;
        main.startLifetime = 3f;
        main.startSpeed = new ParticleSystem.MinMaxCurve(4f, 9f);
        main.startSize = new ParticleSystem.MinMaxCurve(0.02f, 0.08f);
        main.maxParticles = 300;
        main.simulationSpace = ParticleSystemSimulationSpace.World;
        main.startColor = new Color(0.35f, 0.38f, 0.32f, 0.45f);

        var emission = windDebrisSystem.emission;
        emission.rateOverTime = currentWind * 12f;

        var shape = windDebrisSystem.shape;
        shape.shapeType = ParticleSystemShapeType.Box;
        shape.scale = new Vector3(50f, 8f, 50f);

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
