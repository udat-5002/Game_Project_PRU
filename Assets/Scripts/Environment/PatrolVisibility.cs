using UnityEngine;

/// <summary>
/// Giúp lính tuần tra dễ nhìn trong cảnh tối / mưa.
/// </summary>
public static class PatrolVisibility
{
    public static void Apply(Transform patrolRoot, float detectRadius)
    {
        if (patrolRoot == null) return;

        AddHighlightLight(patrolRoot);
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
