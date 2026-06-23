using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Gỡ collider cây/lá/bụi trên Map — chỉ giữ mặt đất.
/// </summary>
[DefaultExecutionOrder(-200)]
public class MapCollisionCleanup : MonoBehaviour
{
    static readonly string[] StripNameContains =
    {
        "Agave", "Banana", "leaves", "Leaves", "leaf", "Leaf",
        "fern", "Fern", "thatch", "Plant", "plant", "grass", "Grass",
        "tree", "Tree", "branch", "Branch", "foliage", "Foliage",
        "palm", "Palm", "bush", "Bush", "vine", "Vine", "canopy", "Canopy",
        "hedge", "Hedge", "shrub", "Shrub", "flower", "Flower"
    };

    static readonly string[] KeepGroundNameContains =
    {
        "Ground", "Landscape", "terrain", "Terrain", "Plane"
    };

    static string lastCleanedScene;
    static Transform mapRoot;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void AutoCleanup()
    {
        if (Object.FindFirstObjectByType<MapCollisionCleanup>() != null) return;
        var go = new GameObject("MapCollisionCleanup");
        go.AddComponent<MapCollisionCleanup>();
    }

    void Awake() => RunCleanup();

    void RunCleanup()
    {
        var scene = SceneManager.GetActiveScene().name;
        if (lastCleanedScene == scene) return;
        lastCleanedScene = scene;

        var mapGo = GameObject.Find("Map");
        mapRoot = mapGo != null ? mapGo.transform : null;

        int stripped = 0;
        foreach (var col in FindObjectsByType<Collider>(FindObjectsSortMode.None))
        {
            if (col == null || col.isTrigger) continue;
            if (!ShouldStrip(col)) continue;
            Destroy(col);
            stripped++;
        }

        if (stripped > 0)
            Debug.Log($"[MapCollisionCleanup] Gỡ {stripped} collider cây/lá/bụi.");

        if (mapRoot != null)
            AssignGroundLayer(mapRoot);
    }

    static void AssignGroundLayer(Transform map)
    {
        int groundLayer = LayerMask.NameToLayer("Ground");
        if (groundLayer < 0) return;

        foreach (var col in map.GetComponentsInChildren<Collider>(true))
        {
            if (col.isTrigger) continue;
            if (!IsGroundObject(col.gameObject)) continue;
            col.gameObject.layer = groundLayer;
        }
    }

    static bool ShouldStrip(Collider col)
    {
        if (mapRoot != null && col.transform.IsChildOf(mapRoot))
            return !IsGroundObject(col.gameObject);

        return ShouldStripByPlantName(col.gameObject);
    }

    static bool IsGroundObject(GameObject go)
    {
        for (var t = go.transform; t != null; t = t.parent)
        {
            if (t == mapRoot) break;
            foreach (var k in KeepGroundNameContains)
                if (t.name.IndexOf(k, System.StringComparison.OrdinalIgnoreCase) >= 0)
                    return true;
        }
        return false;
    }

    static bool ShouldStripByPlantName(GameObject go)
    {
        for (var t = go.transform; t != null; t = t.parent)
        {
            foreach (var k in StripNameContains)
                if (t.name.IndexOf(k, System.StringComparison.OrdinalIgnoreCase) >= 0)
                    return true;
        }
        return false;
    }
}
