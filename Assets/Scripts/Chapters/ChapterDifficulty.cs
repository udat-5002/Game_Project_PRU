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
        3 => 14f,
        _ => 0f
    };

    public static PatrolSetup[] GetPatrols(int chapter) => chapter switch
    {
        1 => new[]
        {
            new PatrolSetup
            {
                pointA = ForestZoneLayout.Ch1PatrolStart,
                pointB = ForestZoneLayout.Ch1PatrolEnd,
                moveSpeed = 2.2f,
                detectRadius = 5.5f,
                detectSeconds = 1.5f,
                mustHideToPass = false,
                activeQuestId = "sneak_patrol"
            }
        },
        2 => new[]
        {
            new PatrolSetup
            {
                pointA = ForestZoneLayout.Ch2PatrolStart,
                pointB = ForestZoneLayout.Ch2PatrolEnd,
                moveSpeed = 2.8f,
                detectRadius = 6f,
                detectSeconds = 1.2f,
                mustHideToPass = true,
                activeQuestId = "stealth_cross"
            },
            new PatrolSetup
            {
                pointA = ForestZoneLayout.Ch2Patrol2Start,
                pointB = ForestZoneLayout.Ch2Patrol2End,
                moveSpeed = 3.2f,
                detectRadius = 5.5f,
                detectSeconds = 1f,
                mustHideToPass = true,
                activeQuestId = "stealth_cross"
            }
        },
        3 => new[]
        {
            new PatrolSetup
            {
                pointA = ForestZoneLayout.Ch3PatrolStart,
                pointB = ForestZoneLayout.Ch3PatrolEnd,
                moveSpeed = 3f,
                detectRadius = 7f,
                detectSeconds = 0.9f,
                mustHideToPass = false,
                activeQuestId = "find_clues"
            },
            new PatrolSetup
            {
                pointA = ForestZoneLayout.Ch3Patrol2Start,
                pointB = ForestZoneLayout.Ch3Patrol2End,
                moveSpeed = 3.5f,
                detectRadius = 6.5f,
                detectSeconds = 0.8f,
                mustHideToPass = false,
                activeQuestId = "find_clues"
            },
            new PatrolSetup
            {
                pointA = ForestZoneLayout.Ch3DangerPatrolStart,
                pointB = ForestZoneLayout.Ch3DangerPatrolEnd,
                moveSpeed = 3.8f,
                detectRadius = 7.5f,
                detectSeconds = 0.7f,
                mustHideToPass = true,
                activeQuestId = "cross_danger"
            }
        },
        _ => System.Array.Empty<PatrolSetup>()
    };

    public static float DangerExposureLimit(int chapter) => chapter switch
    {
        1 => 0f,
        2 => 0f,
        3 => 2.4f,
        _ => 0f
    };
}
