using FCMicroservices.Components.FunctionRegistries;
using FCMicroservices.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace FCMicroservices.Components.Functions;

public class HtmlFunctionRenderer : IFunctionRenderer
{
    public string Render(Function f)
    {
        var html = "<h1> <pre>" + f.MicroMessageType + " " + f.MessageType.Name + "</pre> </h1>";
        html += "<div> <pre> NAMESPACE : " + f.MessageType.Namespace + "</pre> </div>";
        html += "<div> <pre> HANDLER / SUBSCRIPTION : " + f.HandlerType.Name + "</pre> </div>";


        string msg = "", reply = "";
        if (f.MessageType != null)
        {
            msg = "<div> <pre>" + Activator.CreateInstance(f.MessageType).ToJson(true) + "</pre> </div>";
        }

        if (f.ReplyType != null)
        {
            reply = "<div> <pre>" + Activator.CreateInstance(f.ReplyType).ToJson(true) + "</pre> </div>";
        }

        html += "<div> <table style='padding:10px; width:75%;'>" +
                "<tr> <td> MESSAGE </td>" +
                "<td> REPLY </td> </tr>" +
                "<tr> " +
                $"<td> {msg} </td>" +
                $"<td> {reply} </td>" +
                "</tr>" +
                "</table> </div>";

        return "<html> <body> <div style='margin:0px; padding:5px;'> " +
               html +
               "<div>" +
               "<pre> Connection Address/Port " + $"{f.ConnectionAddress} : {f.ConnectionPort}" + "</pre>" +
               "</div>" +
               "</div> </body> </html>";

        // var messageSignatures = functions.Select(x => new
        // {
        //     Message = x.MessageName,
        //     MessageSample = x.MessageType != null ? Activator.CreateInstance(x.MessageType) : null,
        //     Handler = x.HandlerName,
        //     Reply = x.ReplyName,
        //     ReplySample = x.ReplyType != null ? Activator.CreateInstance(x.ReplyType) : null
        // }).ToList();
    }
}