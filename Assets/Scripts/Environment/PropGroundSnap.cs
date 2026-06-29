using UnityEngine;

/// <summary>
/// Hạ khúc gỗ / log bay lơ lửng xuống mặt đất.
/// </summary>
public static class PropGroundSnap
{
    static readonly string[] NameHints =
    {
        "Log", "log", "Fallen", "Wood", "wood", "gỗ"
    };

    public static void SnapLogsInScene()
    {
        var moved = new System.Collections.Generic.HashSet<Transform>();

        foreach (var renderer in Object.FindObjectsByType<Renderer>(FindObjectsSortMode.None))
        {
            if (renderer == null) continue;
            var root = FindLogRoot(renderer.transform);
            if (root == null || !moved.Add(root)) continue;
            SnapRootToGround(root);
        }
    }

    static Transform FindLogRoot(Transform t)
    {
        for (var cur = t; cur != null; cur = cur.parent)
        {
            if (IsLogLike(cur.name))
                return cur;
        }

        return null;
    }

    static bool IsLogLike(string name)
    {
        foreach (var hint in NameHints)
        {
            if (name.IndexOf(hint, System.StringComparison.OrdinalIgnoreCase) >= 0)
                return true;
        }

        return false;
    }

    static void SnapRootToGround(Transform root)
    {
        var renderers = root.GetComponentsInChildren<Renderer>();
        if (renderers.Length == 0) return;

        float minY = float.MaxValue;
        foreach (var r in renderers)
            minY = Mathf.Min(minY, r.bounds.min.y);

        if (minY == float.MaxValue) return;
        if (!GroundSnap.TryGetGroundY(root.position, out float groundY)) return;

        float delta = groundY - minY;
        if (Mathf.Abs(delta) < 0.02f) return;

        root.position += new Vector3(0f, delta, 0f);
    }
}
