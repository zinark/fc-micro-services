namespace FCMicroservices.Sagas;

public class SagaStep
{
    public List<string> Needs { get; set; } = new ();
    public SagaStepType Type { get; set; }
    public string Name { get; set; }
    public Box Inbox { get; set; } = new Box();
    public Box Outbox { get; set; } = new Box();
    public Box Failbox { get; set; } = new Box();
    public SagaStepAct Act { get; set; }

    public void Listen(Saga saga)
    {
        Task.Run(() => { ListenMessages(saga); });
    }

    void ListenMessages(Saga saga)
    {
        var ctx = saga.Context;
        Console.WriteLine($"> [{Name}] Listening...");

        while (true)
        {
            var msg = Inbox.Dequeue(ctx);

            if (msg == null)
            {
                Thread.Sleep(100);
                continue;
            }

            Console.WriteLine($"> [{Name}] SagaStep dequeued message : " + msg.Id);

            try
            {
                var response = Act?.Execute(ctx, msg);
                Outbox.AddMessage(ctx, response);
                // TODO : Ferhat, next isini disardaki sistemler yapacak mesaji onlar biliyor
                // var nextStep = saga.FindNextStep(this);
                // if (nextStep != null)
                // {
                //     nextStep.Inbox.AddMessage(ctx, response);
                // }
            }
            catch (Exception e)
            {
                Failbox.AddMessage(ctx, e.ToString());
            }

            Thread.Sleep(100);
        }
    }
}