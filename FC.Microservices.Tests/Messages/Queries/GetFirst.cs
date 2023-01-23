using FCMicroservices.Components.BUS;
namespace FCMicroservices.Tests.Messages.Queries;

[Query]
public class GetFirst
{
    public int Index { get; set; }
    public List<int> Numbers { get; set; } = new();
}
