/// <summary>
/// Trạng thái phiên chơi — menu + hướng dẫn bản đồ theo chương.
/// </summary>
public static class GameSession
{
    public static bool StartedFromMenu { get; set; }

    /// <summary>
    /// Đã mở bản đồ để mở khóa mũi tên / chỉ đường trên HUD.
    /// Bật sau bước check_map từng chương.
    /// </summary>
    public static bool MapRouteUnlocked { get; private set; }

    public static void ResetChapterGuidance()
    {
        MapRouteUnlocked = false;
    }

    public static void UnlockMapRoute()
    {
        MapRouteUnlocked = true;
    }
}
