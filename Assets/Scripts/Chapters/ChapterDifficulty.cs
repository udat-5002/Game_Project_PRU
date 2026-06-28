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

    public static PatrolSetup[] GetPatrols(int chapter)
    {
        var zone = ForestZoneLayout.GetZone(chapter);
        var patrols = new PatrolSetup[10];

        // Đảm bảo seed ngẫu nhiên nhưng thay đổi theo thời gian thực (để lính đổi vị trí mỗi lần chơi)
        // Hoặc giữ nguyên để dễ test. Ở đây dùng random thực.

        for (int i = 0; i < 10; i++)
        {
            string questId = "";
            bool hide = false;

            if (chapter == 1)
            {
                questId = "sneak_patrol";
                hide = false;
            }
            else if (chapter == 2)
            {
                questId = "stealth_cross";
                hide = true;
            }
            else if (chapter == 3)
            {
                // Chia đều cho 2 nhiệm vụ
                questId = i < 6 ? "find_clues" : "cross_danger";
                hide = i >= 6; // cross_danger thì bắt buộc núp
            }

            float rx = zone.center.x + UnityEngine.Random.Range(-zone.groundSize.x * 0.45f, zone.groundSize.x * 0.45f);
            float rz = zone.center.z + UnityEngine.Random.Range(-zone.groundSize.z * 0.45f, zone.groundSize.z * 0.45f);

            patrols[i] = new PatrolSetup
            {
                pointA = new Vector3(rx, 0, rz),
                pointB = new Vector3(rx + UnityEngine.Random.Range(-15f, 15f), 0, rz + UnityEngine.Random.Range(-15f, 15f)),
                moveSpeed = UnityEngine.Random.Range(2.2f, 3.5f),
                detectRadius = UnityEngine.Random.Range(6.75f, 9.75f),
                detectSeconds = UnityEngine.Random.Range(1.0f, 1.8f),
                mustHideToPass = hide,
                activeQuestId = questId
            };
        }

        return patrols;
    }

    public static float DangerExposureLimit(int chapter) => chapter switch
    {
        1 => 0f,
        2 => 0f,
        3 => 2.4f,
        _ => 0f
    };
}
