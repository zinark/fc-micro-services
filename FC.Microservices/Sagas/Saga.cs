using FCMicroservices.Extensions;
using FCMicroservices.Sagas.Endpoints.WalletApis;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace FCMicroservices.Sagas;

public class Saga
{
    public string Title { get; set; }
    public Dictionary<string, SagaStep> Steps { get; set; }

    public SagaContext Context { get; set; }
    public string Status { get; set; } = "ready";
    public string CurrentStep { get; set; }

    public BoxMessage? SendMessage(string stepName, object message)
    {
        CurrentStep = stepName;
        Console.WriteLine("> Send Message :" + stepName + " " + message.ToJson());
        if (!Steps.ContainsKey(stepName)) return null;
        
        SagaStep step = Steps[stepName];

        var boxedMsg = step.Inbox.AddMessage(Context, message.ToJson());
        return boxedMsg;
    }

    public string FindStartStep()
    {
        var found = Steps
            .FirstOrDefault(x => x.Value?.Needs == null || x.Value?.Needs?.Count == 0);

        if (found.Value == null) throw new Exception("needs = 0 olan bir step yok!");

        return found.Key;
    }

    public void Init(SagaContext ctx)
    {
        Status = "in-progress";
        Context = ctx;

        foreach (var stepkey in Steps.Keys)
        {
            SagaStep step = Steps[stepkey];
            step.Listen(this);
        }
    }

    public SagaStep? FindNextStep(SagaStep currentStep)
    {
        foreach (var step in Steps.Values)
        {
            if (step.Needs.Contains(currentStep.Name))
            {
                return step;
            }
        }
        
        return null;
    }
}

public static class ObjectExtensions
{
    public static string ToYaml(this object obj)
    {
        var serializer = new SerializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .Build();

        var yaml = serializer.Serialize(obj);
        return yaml;
    }

}