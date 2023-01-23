using Grpc.Core;
using Ticimax.MicroServices;
using Google.Protobuf;
using System.Text;
using fc.micro.services;
using fc.microservices.Components.Loggers;
using fc.microservices.Extensions;
using fc.microservices.Components.BUS;

namespace fc.microservices;

public class GrpcMicroService : MicroService.MicroServiceBase
{
    public ByteString ToByteString(Stream stream)
    {
        byte[] b;
        using (var memoryStream = new MemoryStream())
        {
            stream.CopyTo(memoryStream);
            b = memoryStream.ToArray();
        }
        return ByteString.CopyFrom(b);
    }
    public ByteString ToByteString(string text)
    {
        byte[] b = Encoding.UTF8.GetBytes(text).ToArray();
        return ByteString.CopyFrom(b);
    }

    private Bus _bus;
    private ITracer _tracer;

    public GrpcMicroService(Bus bus, ITracer tracer)
    {
        _bus = bus;
        _tracer = tracer;
    }

    public override Task<ByteReply> ByteExecute(ByteCommand request, ServerCallContext context)
    {
        var cmd = new Command()
        {
            Json = ParseByteString(request.Json),
            Type = ParseByteString(request.Type)
        };
        Reply reply = Execute(cmd, context).Result;

        var byteReply = new ByteReply()
        {
            Error = reply.Error,
            Json = ToByteString(reply.Json),
            Success = reply.Success,
            Type = ToByteString(reply.Type)
        };

        return Task.FromResult(byteReply);
    }

    private string ParseByteString(ByteString bytestr)
    {
        return bytestr.ToStringUtf8();
    }

    public override Task<ByteReplyList> BatchByteExecute(ByteCommandList request, ServerCallContext context)
    {
        var result = new ByteReplyList();
        foreach (ByteCommand cmd in request.Commands)
        {
            var reply = ByteExecute(cmd, context).Result;
            result.Replies.Add(reply);

        }
        return Task.FromResult(result);

    }

    public override Task<ReplyList> BatchExecute(CommandList request, ServerCallContext context)
    {
        var result = new ReplyList();
        foreach (Command cmd in request.Commands)
        {
            var reply = Execute(cmd, context).Result;
            result.Replies.Add(reply);

        }
        return Task.FromResult(result);
    }

    public override Task<ReplyList> BatchQuery(FilterList request, ServerCallContext context)
    {
        var result = new ReplyList();
        foreach (Filter filter in request.Filters)
        {
            var reply = Query(filter, context).Result;
            result.Replies.Add(reply);

        }
        return Task.FromResult(result);
    }

    public override Task<Reply> Query(Filter request, ServerCallContext context)
    {
        var js = request.Json;
        var type = request.Type;



        try
        {
            Bus.CheckAttribute<QueryAttribute>(type);
            var reply = _bus.Handle(type, js);
            return new Reply()
            {
                Success = true,
                Type = reply.GetType().FullName,
                Json = reply.ToJson()
            }.AsTask();
        }
        catch (ApiException ex)
        {
            return new Reply()
            {
                Success = false,
                Error = $"{ex.Message}\r\n{ex.StackTrace}",
                Json = ex.Data.ToJson(),
                Type = ex.GetType().Name
            }.AsTask();
        }
        catch (Exception ex)
        {
            return new Reply()
            {
                Success = false,
                Error = $"{ex.Message}\r\n{ex.StackTrace}",
                Json = "{}",
                Type = ex.GetType().Name
            }.AsTask();
        }
    }

    public override Task<Reply> Execute(Command request, ServerCallContext context)
    {
        string type = request.Type;
        string js = request.Json;

        _tracer.Trace($"Execute", new Dictionary<string, object>()
        {
            { "Type", type },
            {  "request.json", js }
        });

        try
        {
            Bus.CheckAttribute<CommandAttribute>(type);

            var reply = _bus.Handle(type, js);
            var result = new Reply()
            {
                Success = true,
                Type = reply.GetType().FullName,
                Json = reply.ToJson()
            };

            _tracer.Trace($"Reply - OK", "reply", result.ToJson(true));
            return result.AsTask();
        }
        catch (ApiException ex)
        {
            var reply = new Reply()
            {
                Success = false,
                Error = $"{ex.Message}\r\n{ex.StackTrace}",
                Json = ex.Data.ToJson(),
                Type = ex.GetType().Name
            };

            _tracer.Trace($"Reply - APIEXCEPTION", "reply", reply.ToJson(true));

            return reply.AsTask();
        }
        catch (Exception ex)
        {
            var reply = new Reply()
            {
                Success = false,
                Error = $"{ex.Message}\r\n{ex.StackTrace}",
                Json = "{}",
                Type = ex.GetType().Name
            };

            _tracer.Trace($"Reply - EXCEPTION", "reply", reply.ToJson(true));

            return reply.AsTask();
        }

    }

    public override Task<MicroMessageContracts> Contracts(None request, ServerCallContext context)
    {
        return new MicroMessageContractGenerator().Generate().AsTask();
    }
}
