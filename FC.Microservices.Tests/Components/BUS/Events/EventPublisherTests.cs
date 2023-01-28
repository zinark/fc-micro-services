using FCMicroservices.Components.BUS.Events;
using FCMicroservices.Components.EnterpriseBUS.Events;
using FCMicroservices.Components.Loggers;
using FCMicroservices.Extensions;

namespace FCMicroservices.Tests.Components.BUS.Events;

[TestClass]
public class EventPublisherTests
{
    private const string URL = "http://localhost:4222";

    [TestMethod]
    public void PublishTest()
    {
        using var sub = new EventSubscriber(null, new NoTracer(), URL);

        var messageReceived = false;

        sub.Subscribe<SiparisOlusturuldu>(x =>
        {
            x.ToJson(true).Dump("event");
            messageReceived = true;
        });

        var evnt = new SiparisOlusturuldu
        {
            OrderId = 1,
            TotalAmount = 100
        };
        new EventPublisher(URL).Publish(evnt);

        Thread.Sleep(1000);
        Assert.IsTrue(messageReceived);
    }

    [Event]
    public class SiparisOlusturuldu
    {
        public int OrderId { get; set; }
        public double TotalAmount { get; set; }
    }
}