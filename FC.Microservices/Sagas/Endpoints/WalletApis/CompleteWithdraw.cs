namespace FCMicroservices.Sagas.Endpoints.WalletApis;

public class CompleteWithdraw
{
    public bool Success { get; set; }
    public string Error { get; set; }

    public double Amount { get; set; }

    public string ReceiptNo { get; set; }
    public string ReceiverTCKNorVKN { get; set; }

    public string WithdrawTrackId { get; set; }
}