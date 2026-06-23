using UnityEngine;

public class MailInventory : MonoBehaviour
{
    public static MailInventory Instance { get; private set; }

    public bool HasMail { get; private set; }
    public string recipientName;
    public string senderName;
    [TextArea] public string mailSummary;

    void Awake()
    {
        Instance = this;
    }

    void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    public void ReceiveMail(string recipient, string sender, string summary)
    {
        HasMail = true;
        recipientName = recipient;
        senderName = sender;
        mailSummary = summary;
        GameUI.Instance?.ShowNotification($"Đã nhận thư gửi: {recipient}");
    }

    public bool DeliverMail(string expectedRecipient)
    {
        if (!HasMail) return false;
        if (recipientName != expectedRecipient) return false;

        HasMail = false;
        return true;
    }
}
