using FCMicroservices.Extensions;
using FCMicroservices.Sagas.Endpoints.WalletApis;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace FCMicroservices.Sagas;

public class SagaHost
{
    private readonly Saga _saga;

    public SagaHost(string yamlFilePath)
    {
        var yaml = File.ReadAllText(yamlFilePath);

        var deserializer = new DeserializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .Build();


        _saga = deserializer.Deserialize<Saga>(yaml);
    }

    public void Init(SagaContext ctx)
    {
        ctx.Add("corrid", "saga-" + Guid.NewGuid().ToString().Replace("-", "").ToUpper());
        ctx.Add("id", _saga.Title);
        _saga.Init(ctx);
    }


    public BoxMessage? SendMessage(object message)
    {
        return _saga.SendMessage(_saga.FindStartStep(), message);
    }

    public void Host()
    {
        _saga.Context?.Values.Select(x => x.Key + " : " + x.Value).ToJson(true).Dump("SagaContext");

        while (true)
        {
            if (_saga != null)
            {
                foreach (var stepdef in _saga.Steps)
                {
                    var step = stepdef.Value;
                    if (step.Inbox.Messages.Sum(x=>x.Value.Count) > 0 ||
                        step.Outbox.Messages.Sum(x=>x.Value.Count) > 0 ||
                        step.Failbox.Messages.Sum(x=>x.Value.Count) > 0
                       )
                    {
                        new
                        {
                            Counts = step.Inbox.Messages.Sum(x=>x.Value.Count) + " -> " +
                                     step.Outbox.Messages.Sum(x=>x.Value.Count) + " | fails " +
                                     step.Failbox.Messages.Sum(x=>x.Value.Count),
                            Inbox = step.Inbox.Messages,
                            Outbox = step.Outbox.Messages,
                            Failbox = step.Failbox.Messages
                        }.ToYaml().Dump(stepdef.Key);
                    }
                }

                Console.WriteLine("----");
            }

            Thread.Sleep(5000);
        }

        string BoxReport(Box box)
        {
            return box.Messages.Select(x => x.Key + "=" + x.Value.Count).ToJson(true);
        }
    }
}