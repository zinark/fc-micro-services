namespace FCMicroservices.Sagas.Endpoints.CardsApis;

public class StartWithdraw
{
    public string Tenant { get; set; }
    public double Amount { get; set; }
}