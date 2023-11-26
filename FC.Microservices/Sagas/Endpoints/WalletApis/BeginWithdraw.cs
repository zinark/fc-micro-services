namespace FCMicroservices.Sagas.Endpoints.WalletApis;

public class BeginWithdraw
{
    public Withdraw Withdraw { get; set; } = new Withdraw();
}