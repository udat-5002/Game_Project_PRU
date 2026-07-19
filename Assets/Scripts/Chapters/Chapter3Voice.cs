/// <summary>
/// Giọng Chương 3 — Resources/Audio/Dialogue/ct3-*.mp3
/// Khớp bảng thoại đã lọc:
///
///   ct3-1  Transition         — chào chương
///   ct3-2  IntroHud           — mục tiêu HUD
///   ct3-3  ClueHouse          — nhà bỏ hoang
///   ct3-4  ClueFort           — đồn đổ nát
///   ct3-5  ClueBunker         — hầm trú ẩn
///   ct3-6  CluesComplete      — đủ 3 manh mối
///   ct3-7  BrotherFound       — nhận ra chữ anh trai
///   ct3-8  BrotherNam         — Anh yên tâm, em sẽ làm
///   ct3-9  DeliverNam         — Nam giao thư cuối
///   ct3-10 DeliverRecipient   — người nhận
///   ct3-11 EndNam             — kết chương
///   ct3-12 Ending             — narrator kết game
///
/// Thư anh trai — chỉ hiện chữ (không có file giọng).
/// </summary>
public static class Chapter3Voice
{
    public const string Transition = "ct3-1";
    public const string IntroHud = "ct3-2";
    public const string ClueHouse = "ct3-3";
    public const string ClueFort = "ct3-4";
    public const string ClueBunker = "ct3-5";
    public const string CluesComplete = "ct3-6";
    public const string BrotherFound = "ct3-7";
    public const string BrotherLetter = "";
    public const string BrotherNam = "ct3-8";
    public const string DeliverNam = "ct3-9";
    public const string DeliverRecipient = "ct3-10";
    public const string EndNam = "ct3-11";
    public const string Ending = "ct3-12";

    public const string FlashbackSoldier = DeliverNam;
    public const string FlashbackBaLan = DeliverRecipient;
    public const string FlashbackBrother = DeliverNam;
}
