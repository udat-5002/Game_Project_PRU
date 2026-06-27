using UnityEngine;

/// <summary>
/// Giúp lính tuần tra dễ nhìn trong cảnh tối / mưa.
/// </summary>
public static class PatrolVisibility
{
    public static void Apply(Transform patrolRoot)
    {
        if (patrolRoot == null) return;

        AddHighlightLight(patrolRoot);
        AddGroundRing(patrolRoot);
        BrightenRenderers(patrolRoot);
    }

    static void AddHighlightLight(Transform patrolRoot)
    {
        if (patrolRoot.Find("PatrolHighlightLight") != null) return;

        var lightGo = new GameObject("PatrolHighlightLight");
        lightGo.transform.SetParent(patrolRoot, false);
        lightGo.transform.localPosition = new Vector3(0f, 2.1f, 0f);

        var light = lightGo.AddComponent<Light>();
        light.type = LightType.Point;
        light.color = new Color(1f, 0.94f, 0.78f, 1f);
        light.intensity = 2.2f;
        light.range = 10f;
        light.shadows = LightShadows.None;
    }

    static void AddGroundRing(Transform patrolRoot)
    {
        if (patrolRoot.Find("PatrolGroundRing") != null) return;

        var ring = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        ring.name = "PatrolGroundRing";
        ring.transform.SetParent(patrolRoot, false);

        float localY = -1.5f; // Default fallback
        if (GroundSnap.TryGetGroundY(patrolRoot.position, out float groundY))
        {
            localY = groundY - patrolRoot.position.y + 0.06f;
        }

        ring.transform.localPosition = new Vector3(0f, localY, 0f);
        ring.transform.localScale = new Vector3(2.4f, 0.02f, 2.4f);

        var col = ring.GetComponent<Collider>();
        if (col != null) Object.Destroy(col);

        var shader = Shader.Find("Universal Render Pipeline/Lit") ?? Shader.Find("Standard");
        var mat = new Material(shader);
        var ringColor = new Color(1f, 0.78f, 0.15f, 0.55f);
        mat.SetColor("_BaseColor", ringColor);
        mat.color = ringColor;
        mat.EnableKeyword("_EMISSION");
        mat.SetColor("_EmissionColor", new Color(1f, 0.65f, 0.1f) * 0.35f);

        var renderer = ring.GetComponent<Renderer>();
        if (renderer != null)
            renderer.material = mat;
    }

    static void BrightenRenderers(Transform patrolRoot)
    {
        foreach (var renderer in patrolRoot.GetComponentsInChildren<Renderer>(true))
        {
            if (renderer == null || renderer.material == null) continue;
            if (renderer.gameObject.name == "PatrolGroundRing") continue;

            var mat = renderer.material;
            if (mat.name.Contains("demo_soldier") || mat.name.Contains("demo_weapon")) continue;

            if (mat.HasProperty("_BaseColor"))
            {
                var c = mat.GetColor("_BaseColor");
                mat.SetColor("_BaseColor", Color.Lerp(c, Color.white, 0.22f));
            }
            else if (mat.HasProperty("_Color"))
            {
                var c = mat.color;
                mat.color = Color.Lerp(c, Color.white, 0.22f);
            }

            if (mat.HasProperty("_EmissionColor"))
            {
                mat.EnableKeyword("_EMISSION");
                mat.SetColor("_EmissionColor", new Color(0.35f, 0.3f, 0.22f) * 0.18f);
            }
        }
    }
}
