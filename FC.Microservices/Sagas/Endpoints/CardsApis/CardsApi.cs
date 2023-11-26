using FCMicroservices.Sagas.Endpoints.WalletApis;

namespace FCMicroservices.Sagas.Endpoints.CardsApis;

public class CardsApi
{
    private readonly SagaHost _sagaHost;

    public CardsApi(SagaHost sagaHost)
    {
        _sagaHost = sagaHost;
    }

    public StartWithdrawReply StartWithdraw(StartWithdraw cmd)
    {
        // _sagaRunner.SendMessage(new Withdraw()
        // {
        // });
        
        return new StartWithdrawReply();
    }

    public string CompleteWithdraw(string paratekCompleteSignal)
    {
        var cmd = new CompleteWithdraw()
        {
            Success = true,
            Amount = 100,
            Error = "",
            ReceiptNo = "Receipt-1",
            WithdrawTrackId = "11",
            ReceiverTCKNorVKN = "TCKN"
        };
        
        var msg = _sagaHost.SendMessage(cmd);

        return "paratek istedigi neyse ver gec";
    }
}