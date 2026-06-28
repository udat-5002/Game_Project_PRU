using System.Collections;
using UnityEngine;

/// <summary>
/// Hiệu ứng bom/pháo rơi trong vùng pháo kích Chương 3.
/// </summary>
public class ArtilleryBarrage : MonoBehaviour
{
    public string activeQuestId = "cross_danger";
    public float minDelay = 0.5f;
    public float maxDelay = 1.6f;
    public float zoneRadius = 6.5f;
    public float shellHeight = 38f;

    float nextImpactTime;
    Material particleMat;

    void Start()
    {
        particleMat = CreateParticleMaterial(new Color(1f, 0.55f, 0.15f, 0.9f));
        ScheduleNextImpact(Random.Range(0.2f, 0.9f));
    }

    void Update()
    {
        if (!IsQuestActive()) return;
        if (Time.time < nextImpactTime) return;

        ScheduleNextImpact(Random.Range(minDelay, maxDelay));
        StartCoroutine(ShellStrikeRoutine(GetRandomImpactPoint()));
    }

    bool IsQuestActive() =>
        QuestManager.Instance != null && QuestManager.Instance.IsStepActive(activeQuestId);

    void ScheduleNextImpact(float delay) => nextImpactTime = Time.time + delay;

    Vector3 GetRandomImpactPoint()
    {
        var offset = new Vector3(
            Random.Range(-zoneRadius, zoneRadius),
            0f,
            Random.Range(-zoneRadius, zoneRadius));

        var point = transform.position + offset;
        if (GroundSnap.TryGetGroundY(point, out float y))
            point.y = y + 0.05f;
        return point;
    }

    IEnumerator ShellStrikeRoutine(Vector3 impactPoint)
    {
        GameObject shell = null;
        var prefab = Resources.Load<GameObject>("RustyShell");
        if (prefab != null)
        {
            shell = Object.Instantiate(prefab);
            shell.transform.localScale = Vector3.one * 1.5f; // Chỉnh scale cho hợp lý
            shell.transform.rotation = Quaternion.Euler(90f, 0f, 0f); // Mũi đạn hướng xuống đất
        }
        else
        {
            shell = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            shell.transform.localScale = Vector3.one * 0.35f;
            var shellCol = shell.GetComponent<Collider>();
            if (shellCol != null) Destroy(shellCol);

            var shellR = shell.GetComponent<Renderer>();
            if (shellR != null)
            {
                var mat = new Material(Shader.Find("Universal Render Pipeline/Lit") ?? Shader.Find("Standard"));
                mat.color = new Color(0.25f, 0.22f, 0.2f, 1f);
                shellR.material = mat;
            }
        }
        
        shell.name = "IncomingShell";

        var start = impactPoint + Vector3.up * shellHeight;
        shell.transform.position = start;

        float fallTime = 0.55f;
        float t = 0f;
        while (t < fallTime)
        {
            t += Time.deltaTime;
            float p = t / fallTime;
            shell.transform.position = Vector3.Lerp(start, impactPoint, p * p);
            yield return null;
        }

        Destroy(shell);
        SpawnExplosion(impactPoint);
    }

    void SpawnExplosion(Vector3 point)
    {
        var burstGo = new GameObject("ShellBurst");
        burstGo.transform.position = point;

        var ps = burstGo.AddComponent<ParticleSystem>();
        var main = ps.main;
        main.loop = false;
        main.duration = 0.35f;
        main.startLifetime = new ParticleSystem.MinMaxCurve(0.35f, 0.8f);
        main.startSpeed = new ParticleSystem.MinMaxCurve(4f, 11f);
        main.startSize = new ParticleSystem.MinMaxCurve(0.25f, 0.75f);
        main.maxParticles = 80;
        main.simulationSpace = ParticleSystemSimulationSpace.World;
        main.startColor = new Color(1f, 0.65f, 0.2f, 1f);
        main.gravityModifier = 0.6f;

        var emission = ps.emission;
        emission.rateOverTime = 0f;
        emission.SetBursts(new[] { new ParticleSystem.Burst(0f, 28, 40) });

        var shape = ps.shape;
        shape.shapeType = ParticleSystemShapeType.Sphere;
        shape.radius = 0.15f;

        var renderer = burstGo.GetComponent<ParticleSystemRenderer>();
        renderer.material = particleMat;

        var smokeGo = new GameObject("ShellSmoke");
        smokeGo.transform.SetParent(burstGo.transform);
        smokeGo.transform.position = point;
        var smoke = smokeGo.AddComponent<ParticleSystem>();
        var smokeMain = smoke.main;
        smokeMain.loop = false;
        smokeMain.duration = 1.2f;
        smokeMain.startLifetime = new ParticleSystem.MinMaxCurve(0.8f, 1.6f);
        smokeMain.startSpeed = new ParticleSystem.MinMaxCurve(1f, 3f);
        smokeMain.startSize = new ParticleSystem.MinMaxCurve(0.8f, 2.2f);
        smokeMain.maxParticles = 40;
        smokeMain.simulationSpace = ParticleSystemSimulationSpace.World;
        smokeMain.startColor = new Color(0.35f, 0.32f, 0.3f, 0.55f);

        var smokeEmission = smoke.emission;
        smokeEmission.rateOverTime = 0f;
        smokeEmission.SetBursts(new[] { new ParticleSystem.Burst(0f, 12, 18) });

        var lightGo = new GameObject("Flash");
        lightGo.transform.position = point + Vector3.up * 1.5f;
        var light = lightGo.AddComponent<Light>();
        light.type = LightType.Point;
        light.color = new Color(1f, 0.7f, 0.35f);
        light.intensity = 3.5f;
        light.range = 14f;
        Destroy(lightGo, 0.18f);

        Destroy(burstGo, 2.5f);
    }

    static Material CreateParticleMaterial(Color color)
    {
        var shader = Shader.Find("Universal Render Pipeline/Particles/Unlit")
                     ?? Shader.Find("Particles/Standard Unlit")
                     ?? Shader.Find("Legacy Shaders/Particles/Alpha Blended");
        var mat = new Material(shader);
        mat.color = color;
        return mat;
    }
}
