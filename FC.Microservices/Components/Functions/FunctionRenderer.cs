using FCMicroservices.Extensions;
using FCMicroservices.MicroUtils;

using Microsoft.AspNetCore.Mvc;

namespace FCMicroservices.Components.Functions;

public class FunctionRenderer : IFunctionRenderer
{
    private MicroMessageContractGenerator _generator;

    public FunctionRenderer()
    {
        _generator = new MicroMessageContractGenerator();
    }

    public string Render(Function f)
    {
        var html = "";
        html += "<h3> <pre>[" + f.MessageType.Namespace + "] </pre> </h3>";
        string logo_color = "red";
        if (f.MicroMessageType == "[C]") logo_color = "green";
        if (f.MicroMessageType == "[Q]") logo_color = "orange";
        if (f.MicroMessageType == "[E]") logo_color = "blue";

        string logo = $"<span style='font-size: 16px; background-color:{logo_color}; color:white; padding:3px; margin:1px'> {f.MicroMessageType} </span>";
        html += "<h2> <pre>" + logo + " " + f.MessageType.Name + "</pre> </h2>";


        string flow = f.MessageType?.Name + " -> " + f.HandlerType.Name + "()";
        if (f.ReplyType != null)
        {
            flow += " => " + f.ReplyType?.Name;
        }

        html += "<div> <pre>" + flow + "</pre> </div>";
        html += "<div> <pre style='font-style:italic'>" + f.Description + "</pre> </div>";


        string msg = "{}", reply = "{}";
        if (f.MessageType != null)
        {
            msg = _generator.GenerateCode(f.MessageType).sampleJson;
        }

        if (f.ReplyType != null)
        {
            reply = _generator.GenerateCode(f.ReplyType).sampleJson;
        }

        html += "<div> " +
                "<table style='padding:10px; width:80%;'>" +
                $"<tr> " +
                $"<td> <h4><pre>{f.MessageType?.Name} </pre> </h4> </td> " +
                $"<td> <h4><pre>{f.ReplyType?.Name} </pre> </h4> </td> " +
                $"</tr>" +
                "<tr> " +
                $"<td> <pre><textarea id='msg' style='width:80%; padding:10px; margin:0px; height:320px'>{msg}</textarea><pre> </td>" +
                $"<td> <pre><div style='width:80%; max-width:320px; padding:10px; margin:0px; height:320px'><span id='reply'>{reply}</span></div></pre> </td>" +
                "</tr>" +
                "</table> </div>";

        string js = @"<script>
    function send () {
        
        var js = document.getElementById('msg').value;
        document.getElementById ('send')['disabled'] = 'disabled';

        const data = {
            type : window.f,
            json : js
            
        };

        url = '/execute';
        if (window.ftype == '[Q]') url = '/query';
        if (window.ftype == '[E]') url = '/publish';
        
        fetch(url, {
          method: 'POST',
          headers: {
            'Content-Type': 'application/json',
          },
          body: JSON.stringify(data),
        })
          .then((response) => response.json())
          .then((data) => {
            document.getElementById ('send')['disabled'] = null;
            console.log('Success:', data);

            response = ''
            response += 'POST ' + url;
            response += '\r\n';

            response += JSON.stringify(data, '', 4);
            if (data.json) {
                response += '\r\n\r\njson:' + JSON.stringify(JSON.parse (data.json), '', 4)
            }
            
            if (!data.success){
                response += '\r\n\r\nhata:' + data.error;
            }
            
            document.getElementById('reply').innerHTML = response
          })
          .catch((error) => {
            document.getElementById ('send')['disabled'] = null;
            document.getElementById('reply').innerHTML = 'ERR : ' + error;
          });        
    }
</script>";

        js += $"<script> window.f = '{f.MessageName}'; </script>";
        js += $"<script> window.ftype = '{f.MicroMessageType}'; </script>";

        string html_buttons = @"<button id='send' onclick=""send()""> EXECUTE </button>";

        return $"<html> <head> {js} </head> <body> <div style='margin:0px; padding:5px;'> " +
               html +
               "<div>" +
               "<pre> Connection Address/Port " + $"{f.ConnectionAddress} : {f.ConnectionPort}" + "</pre>" +
               "</div>" +
               "<div>" +
               html_buttons +
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