using FCMicroservices.Components.BUS;
using FCMicroservices.Components.BUS.Events;
using FCMicroservices.Components.CustomerDomainResolvers;
using FCMicroservices.Tests.Messages.Events;

namespace FCMicroservices.Tests.Messages.Commands;

public class SumHandler : Handler<Sum, SumReply>
{
    private readonly IEventPublisher publisher;
    private readonly ITenantResolver _tenantResolver;

    public SumHandler(ITenantResolver resolver, IEventPublisher publisher)
    {
        _tenantResolver = resolver;
        this.publisher = publisher;
    }

    public override SumReply Handle(Sum input)
    {
        if (string.IsNullOrWhiteSpace(input.TenantId)) input.TenantId = _tenantResolver.Resolve();

        publisher.Publish(new HesaplamaBitti
        {
            ToplamTutar = input.X * input.Y
        });

        return new SumReply
        {
            Sum = input.X + input.Y,
            Tenant = input.TenantId
        };
    }
}