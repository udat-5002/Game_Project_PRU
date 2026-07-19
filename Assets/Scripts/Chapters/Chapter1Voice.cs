/// <summary>
/// Giọng đọc Chương 1 — file trong Resources/Audio/Dialogue/
/// khớp tên file: ct1-1.mp3 … ct1-11.mp3
/// </summary>
public static class Chapter1Voice
{
    /// <summary>Màn chào chương.</summary>
    public const string Transition = "ct1-1";

    /// <summary>Intro mục tiêu khi vào bản đồ.</summary>
    public const string IntroHud = "ct1-2";

    /// <summary>Cụ già — dặn xem bản đồ (file ct1-3; trước gắn nhầm Trạm).</summary>
    public const string PickupStation = "ct1-3";

    /// <summary>Nam — đáp nhận thư.</summary>
    public const string PickupNam = "ct1-4";

    /// <summary>Cụ già — hỏi đường.</summary>
    public const string Elder = "ct1-5";

    /// <summary>Alias — ct1-3 thuộc cụ già.</summary>
    public const string ElderMapAdvice = PickupStation;

    /// <summary>Nam — đáp cụ già.</summary>
    public const string ElderNam = "ct1-6";

    /// <summary>Nam — vượt gỗ đổ.</summary>
    public const string CrossObstacle = "ct1-7";

    /// <summary>Bà Lan — câu 1 khi nhận thư.</summary>
    public const string DeliverBaLan1 = "ct1-8";

    /// <summary>Nam — giao thư.</summary>
    public const string DeliverNam = "ct1-9";

    /// <summary>Bà Lan — câu 2.</summary>
    public const string DeliverBaLan2 = "ct1-10";

    /// <summary>Nam — kết chương.</summary>
    public const string EndNam = "ct1-11";

    // Alias cũ (tránh lỗi nếu chỗ nào còn gọi)
    public const string PickupMail = PickupStation;
    public const string ElderGuide = Elder;
    public const string DeliverBaLan = DeliverBaLan1;
}
