namespace FCMicroservices.Sagas.Endpoints.WalletApis;

public class Withdraw
{
    public string ExternalId { get; set; }
    public long AmountMantis { get; set; } = 100;
    public int AmountExponent { get; set; } = -2;
    public string Currency { get; set; } = "TRY";
    public string Description { get; set; }
    public Sender Sender { get; set; } = new();

    public string TargetIBAN { get; set; }
    public string TargetIBANFullName { get; set; }
    public string? TargetTCKNorVKN { get; set; }
    public bool TargetIBANIsMine { get; set; }

    public bool TestMode { get; set; } = false;
}

public class Sender
{
    public int? WalletId { get; set; }
    public string? WalletCode { get; set; }

    public int? ActorId { get; set; }
    public string? ActorCode { get; set; }

    public string? TCKN { get; set; }
    public string? VKN { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
}
