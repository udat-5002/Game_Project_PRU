using UnityEngine;

public static class GroundSnap
{
    const float RayStartHeight = 80f;
    const float RayLength = 120f;
    const float DefaultFootOffset = 0.08f;
    const float FallbackGroundY = 3.09f;
    const float PlayerFootToPivot = 1.5f;

    public static bool TryGetGroundY(Vector3 worldPos, out float groundY, LayerMask? layers = null)
    {
        int mask = layers ?? LayerMask.GetMask("Ground", "Default");
        var origin = new Vector3(worldPos.x, worldPos.y + RayStartHeight, worldPos.z);

        var hits = Physics.RaycastAll(origin, Vector3.down, RayLength, mask, QueryTriggerInteraction.Ignore);
        if (hits.Length == 0)
        {
            groundY = FallbackGroundY;
            return false;
        }

        groundY = hits[0].point.y;
        for (int i = 1; i < hits.Length; i++)
        {
            if (hits[i].point.y < groundY)
                groundY = hits[i].point.y;
        }
        return true;
    }

    public static Vector3 Snap(Vector3 worldPos, float footOffset = DefaultFootOffset)
    {
        if (TryGetGroundY(worldPos, out float y))
            worldPos.y = y + footOffset;
        return worldPos;
    }

    public static Vector3 SnapPlayer(Vector3 worldPos, Transform player = null)
    {
        if (TryGetGroundY(worldPos, out float y))
            worldPos.y = y + PlayerFootToPivot + 0.1f;
        else
            worldPos.y = FallbackGroundY + PlayerFootToPivot;
        return worldPos;
    }
}
