using UnityEngine;

/// <summary>
/// Cấu hình độ khó tăng dần theo chương.
/// </summary>
public static class ChapterDifficulty
{
    public struct PatrolSetup
    {
        public Vector3 pointA;
        public Vector3 pointB;
        public float moveSpeed;
        public float detectRadius;
        public float detectSeconds;
        public bool mustHideToPass;
        public string activeQuestId;
    }

    public static float TimeLimitMinutes(int chapter) => chapter switch
    {
        1 => 18f,
        2 => 16f,
        3 => 18f,
        _ => 0f
    };

    public static PatrolSetup[] GetPatrols(int chapter, string questId)
    {
        // Không còn thử thách lính tuần tra ở các chương.
        return System.Array.Empty<PatrolSetup>();
    }

    static Vector3 PickPatrolPoint(int chapter, string questId, int index)
    {
        return (chapter, questId) switch
        {
            (1, "sneak_patrol") => AlongRoute(
                ForestZoneLayout.Ch1PatrolStart,
                ForestZoneLayout.Ch1PatrolEnd,
                index, 5f),

            (2, "stealth_cross") => AlongRoute(
                ForestZoneLayout.Ch2PatrolStart,
                ForestZoneLayout.Ch2PatrolEnd,
                index, 6f),

            _ => AlongRoute(
                ForestZoneLayout.Ch1PatrolStart,
                ForestZoneLayout.Ch1PatrolEnd,
                index, 8f)
        };
    }

    static Vector3 AlongRoute(Vector3 start, Vector3 end, int index, float sideOffset)
    {
        float t = Mathf.Clamp01((index % 5 + 1) / 6f);
        var pos = Vector3.Lerp(start, end, t);
        var forward = (end - start);
        forward.y = 0f;
        if (forward.sqrMagnitude < 0.01f)
            forward = Vector3.forward;
        forward.Normalize();
        var side = Vector3.Cross(Vector3.up, forward);
        pos += side * ((index % 2 == 0 ? 1f : -1f) * sideOffset);
        pos += new Vector3(Random.Range(-2f, 2f), 0f, Random.Range(-2f, 2f));
        return pos;
    }

    public static float DangerExposureLimit(int chapter) => chapter switch
    {
        1 => 0f,
        2 => 0f,
        3 => 2.4f,
        _ => 0f
    };
}
