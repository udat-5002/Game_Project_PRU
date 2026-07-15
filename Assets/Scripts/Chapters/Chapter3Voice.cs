/// <summary>
/// Giọng đọc Chương 3 — file trong Resources/Audio/Dialogue/
/// khớp tên file: ct3-1.mp3 … ct3-12.mp3
/// Kết game narrator: Resources/Audio/end.mp3
/// </summary>
public static class Chapter3Voice
{
    /// <summary>Màn chào chương.</summary>
    public const string Transition = "ct3-1";

    /// <summary>Intro mục tiêu.</summary>
    public const string IntroHud = "ct3-2";

    /// <summary>Manh mối — nhà bỏ hoang.</summary>
    public const string ClueHouse = "ct3-3";

    /// <summary>Manh mối — hầm trú ẩn (1 voice: ct3-4).</summary>
    public const string ClueBunker = "ct3-4";

    /// <summary>Manh mối — đồn đổ nát (1 voice: ct3-5).</summary>
    public const string ClueFort = "ct3-5";

    /// <summary>Đủ 3 manh mối.</summary>
    public const string CluesComplete = "ct3-6";

    /// <summary>Nam nhận ra chữ anh trai.</summary>
    public const string BrotherFound = "ct3-7";

    /// <summary>Nội dung thư anh trai.</summary>
    public const string BrotherLetter = "ct3-8";

    /// <summary>Nam — Anh yên tâm, em sẽ làm.</summary>
    public const string BrotherNam = "ct3-9";

    /// <summary>Nam — giao thư cuối.</summary>
    public const string DeliverNam = "ct3-10";

    /// <summary>Người nhận.</summary>
    public const string DeliverRecipient = "ct3-11";

    /// <summary>Nam — kết chương.</summary>
    public const string EndNam = "ct3-12";

    /// <summary>Narrator — màn kết game (Resources/Audio/end).</summary>
    public const string Ending = "end";

    // Alias cũ
    public const string FlashbackSoldier = DeliverNam;
    public const string FlashbackBaLan = DeliverRecipient;
    public const string FlashbackBrother = DeliverNam;
}
