using UnityEngine;

/// <summary>
/// Ba khu rừng — vị trí nhiệm vụ dàn trải hơn theo độ khó từng chương.
/// </summary>
public static class ForestZoneLayout
{
    public struct Zone
    {
        public int chapter;
        public string shortName;
        public string fullName;
        public Color groundColor;
        public Color signColor;
        public Vector3 center;
        public Vector3 groundSize;
    }

    public static readonly Zone Zone1 = new Zone
    {
        chapter = 1,
        shortName = "KHU 1: RÌA LÀNG",
        fullName = "Chương 1 — Con đường hy vọng",
        groundColor = new Color(0.15f, 0.65f, 0.28f, 0.22f),
        signColor = new Color(0.2f, 0.85f, 0.35f),
        center = new Vector3(2f, 0f, -34f),
        groundSize = new Vector3(48f, 0.05f, 32f)
    };

    public static readonly Zone Zone2 = new Zone
    {
        chapter = 2,
        shortName = "KHU 2: RỪNG SÂU",
        fullName = "Chương 2 — Bóng tối chiến tranh",
        groundColor = new Color(0.12f, 0.22f, 0.55f, 0.24f),
        signColor = new Color(0.35f, 0.45f, 0.95f),
        center = new Vector3(30f, 0f, -22f),
        groundSize = new Vector3(40f, 0.05f, 30f)
    };

    public static readonly Zone Zone3 = new Zone
    {
        chapter = 3,
        shortName = "KHU 3: VÙNG CHIẾN SỰ",
        fullName = "Chương 3 — Lá thư cuối cùng",
        groundColor = new Color(0.55f, 0.18f, 0.1f, 0.24f),
        signColor = new Color(0.9f, 0.35f, 0.15f),
        center = new Vector3(18f, 0f, -12f),
        groundSize = new Vector3(38f, 0.05f, 28f)
    };

    public static Zone GetZone(int chapter) => chapter switch
    {
        1 => Zone1,
        2 => Zone2,
        3 => Zone3,
        _ => Zone1
    };

    public static Zone[] All => new[] { Zone1, Zone2, Zone3 };

    // Chương 1 — đường dài + 1 lính tuần tra
    public static readonly Vector3 Ch1Spawn = new Vector3(22f, 0f, -44f);
    public static readonly Vector3 Ch1MailStation = new Vector3(10f, 0f, -42f);
    public static readonly Vector3 Ch1Elder = new Vector3(-2f, 0f, -40f);
    public static readonly Vector3 Ch1Obstacle = new Vector3(-10f, 0f, -37f);
    public static readonly Vector3 Ch1PatrolStart = new Vector3(-8f, 0f, -33f);
    public static readonly Vector3 Ch1PatrolEnd = new Vector3(-16f, 0f, -31f);
    public static readonly Vector3 Ch1HideSpot = new Vector3(-13f, 0f, -32f);
    public static readonly Vector3 Ch1StealthEnd = new Vector3(-18f, 0f, -29f);
    public static readonly Vector3 Ch1Delivery = new Vector3(-24f, 0f, -25f);

    // Chương 2 — 2 lính tuần tra + mưa bão
    public static readonly Vector3 Ch2Spawn = new Vector3(22f, 0f, -38f);
    public static readonly Vector3 Ch2Soldier = new Vector3(18f, 0f, -36f);
    public static readonly Vector3 Ch2PatrolStart = new Vector3(24f, 0f, -30f);
    public static readonly Vector3 Ch2PatrolEnd = new Vector3(32f, 0f, -26f);
    public static readonly Vector3 Ch2Patrol2Start = new Vector3(28f, 0f, -24f);
    public static readonly Vector3 Ch2Patrol2End = new Vector3(36f, 0f, -20f);
    public static readonly Vector3 Ch2HideSpot = new Vector3(30f, 0f, -27f);
    public static readonly Vector3 Ch2StealthEnd = new Vector3(38f, 0f, -18f);
    public static readonly Vector3 Ch2RainShelter = new Vector3(42f, 0f, -14f);
    public static readonly Vector3 Ch2Mother = new Vector3(46f, 0f, -8f);

    // Chương 3 — manh mối xa + pháo kích
    public static readonly Vector3 Ch3Spawn = new Vector3(12f, 0f, -22f);
    public static readonly Vector3 Ch3Clue1 = new Vector3(16f, 0f, -20f);
    public static readonly Vector3 Ch3Clue2 = new Vector3(22f, 0f, -16f);
    public static readonly Vector3 Ch3Clue3 = new Vector3(28f, 0f, -12f);
    public static readonly Vector3 Ch3PatrolStart = new Vector3(18f, 0f, -17f);
    public static readonly Vector3 Ch3PatrolEnd = new Vector3(24f, 0f, -14f);
    public static readonly Vector3 Ch3Patrol2Start = new Vector3(25f, 0f, -10f);
    public static readonly Vector3 Ch3Patrol2End = new Vector3(30f, 0f, -7f);
    public static readonly Vector3 Ch3DangerZone = new Vector3(26f, 0f, -9f);
    public static readonly Vector3 Ch3DangerReset = new Vector3(20f, 0f, -15f);
    public static readonly Vector3 Ch3FinalDelivery = new Vector3(32f, 0f, -4f);

    public static readonly Vector3 Gate1To2 = new Vector3(12f, 0f, -28f);
    public static readonly Vector3 Gate2To3 = new Vector3(24f, 0f, -16f);

    public static Vector3 SnapPoint(Vector3 xzPos) => GroundSnap.Snap(xzPos);
}
