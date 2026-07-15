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
        1 => 12f,
        2 => 10f,
        3 => 18f,
        _ => 0f
    };

    public static PatrolSetup[] GetPatrols(int chapter, string questId)
    {
        if (chapter == 3)
            return System.Array.Empty<PatrolSetup>();

        int count = chapter switch
        {
            1 => 5,
            2 => 5,
            _ => 4
        };

        var patrols = new PatrolSetup[count];
        for (int i = 0; i < count; i++)
        {
            bool hide = questId is "stealth_cross";
            var pointA = PickPatrolPoint(chapter, questId, i);
            var pointB = PickPatrolPoint(chapter, questId, i + 3);

            patrols[i] = new PatrolSetup
            {
                pointA = GroundSnap.Snap(pointA),
                pointB = GroundSnap.Snap(pointB),
                moveSpeed = Random.Range(2.2f, 3.2f),
                detectRadius = Random.Range(6.5f, 8.5f),
                detectSeconds = Random.Range(1.1f, 1.6f),
                mustHideToPass = hide,
                activeQuestId = questId
            };
        }

        return patrols;
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
