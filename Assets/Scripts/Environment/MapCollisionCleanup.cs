using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Chỉ gỡ collider cỏ, bụi nhỏ và chuối — cây gỗ, đá, nhà vẫn chặn đường.
/// </summary>
[DefaultExecutionOrder(-200)]
public class MapCollisionCleanup : MonoBehaviour
{
    static readonly string[] PassThroughNameContains =
    {
        "Banana", "banana",
        "grass", "Grass",
        "Agave", "agave",
        "fern", "Fern", "ferns",
        "bush", "Bush", "shrub", "Shrub",
        "hedge", "Hedge", "thatch",
        "flower", "Flower", "vine", "Vine"
    };

    static readonly string[] SolidObstacleNameContains =
    {
        "Rock", "rock", "Cliff",
        "NewTree", "tree", "Tree",
        "trunk", "Trunk", "branch", "Branch",
        "palm", "Palm", "canopy", "Canopy",
        "house", "House", "fence", "Fence", "wall", "Wall"
    };

    static readonly string[] KeepGroundNameContains =
    {
        "Ground", "Landscape", "terrain", "Terrain", "Plane", "Floor", "floor"
    };

    static Transform mapRoot;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void AutoCleanup() => ForceRun();

    void Awake() => ForceRun();

    public static void ForceRun()
    {
        CacheMapRoot();

        int stripped = 0;
        foreach (var col in Object.FindObjectsByType<Collider>(FindObjectsSortMode.None))
        {
            if (col == null || col.isTrigger) continue;
            if (!ShouldStrip(col)) continue;

            col.enabled = false;
            Object.Destroy(col);
            stripped++;
        }

        if (stripped > 0)
            Debug.Log($"[MapCollisionCleanup] Gỡ {stripped} collider cỏ/bụi/chuối.");

        if (mapRoot != null)
            AssignGroundLayer(mapRoot);
    }

    static void CacheMapRoot()
    {
        var mapGo = GameObject.Find("Map");
        if (mapGo != null)
        {
            mapRoot = mapGo.transform;
            return;
        }

        foreach (var root in SceneManager.GetActiveScene().GetRootGameObjects())
        {
            if (root.name.IndexOf("Map", System.StringComparison.OrdinalIgnoreCase) >= 0)
            {
                mapRoot = root.transform;
                return;
            }
        }

        mapRoot = null;
    }

    static void AssignGroundLayer(Transform map)
    {
        int groundLayer = LayerMask.NameToLayer("Ground");
        if (groundLayer < 0) return;

        foreach (var col in map.GetComponentsInChildren<Collider>(true))
        {
            if (col == null || !col.enabled || col.isTrigger) continue;
            if (!IsGroundObject(col.gameObject)) continue;
            col.gameObject.layer = groundLayer;
        }
    }

    static bool ShouldStrip(Collider col)
    {
        if (IsGroundObject(col.gameObject))
            return false;

        if (IsSolidObstacle(col.gameObject))
            return false;

        return IsPassThroughVegetation(col.gameObject);
    }

    static bool IsSolidObstacle(GameObject go)
    {
        for (var t = go.transform; t != null; t = t.parent)
        {
            foreach (var k in SolidObstacleNameContains)
            {
                if (t.name.IndexOf(k, System.StringComparison.OrdinalIgnoreCase) >= 0)
                    return true;
            }
        }

        return false;
    }

    static bool IsGroundObject(GameObject go)
    {
        for (var t = go.transform; t != null; t = t.parent)
        {
            if (mapRoot != null && t == mapRoot) break;
            foreach (var k in KeepGroundNameContains)
            {
                if (t.name.IndexOf(k, System.StringComparison.OrdinalIgnoreCase) >= 0)
                    return true;
            }
        }

        return false;
    }

    static bool IsPassThroughVegetation(GameObject go)
    {
        for (var t = go.transform; t != null; t = t.parent)
        {
            foreach (var k in PassThroughNameContains)
            {
                if (t.name.IndexOf(k, System.StringComparison.OrdinalIgnoreCase) >= 0)
                    return true;
            }
        }

        return false;
    }
}
