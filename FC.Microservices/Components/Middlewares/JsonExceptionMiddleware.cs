using System.Diagnostics;
using System.Security;
using FCMicroservices.Components.EnterpriseBUS;
using FCMicroservices.Extensions;
using FCMicroservices.Utils;
using Microsoft.AspNetCore.Http;

namespace FCMicroservices.Components.Middlewares;

public class JsonExceptionMiddleware
{
    private static readonly ActivitySource _src = new(AssemblyUtils.API_FULL_NAME);
    private readonly RequestDelegate _next;

    public JsonExceptionMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next.Invoke(context);
        }

        catch (Exception ex)
        {
            var status = BuildHttpStatus(ex);
            using var activity = _src.StartActivity("ApiException");
            // activity?.SetTag(context.Request.Body)
            activity?.SetTag("Message", ex.GetBaseException().Message);
            activity?.SetTag("Exception", ex.ToString());

            await HandleResponse(context, status, ex);
        }
    }

    private async Task HandleResponse(HttpContext context, int status, Exception ex)
    {
        context.Response.StatusCode = status;
        if (context.Response.Body.CanWrite)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = 500;

            object jsonObject = new { };

            if (ex is ApiException)
            {
                var castedEx = ex as ApiException;
                context.Response.StatusCode = castedEx.StatusCode;
                jsonObject = new
                {
                    code = castedEx.ErrorCode,
                    message = castedEx.Message,
                    data = castedEx.Data
                };
            }
            else
            {
                jsonObject = new
                {
                    code = 0,
                    message = ex.Message,
                    data = new
                    {
                        trace = ex.StackTrace
                    }
                };
            }

            using (var writter = new StreamWriter(context.Response.Body))
            {
                var json = jsonObject.ToJson();
                await writter.WriteAsync(json);
            }
        }
    }

    private int BuildHttpStatus(Exception ex)
    {
        // TODO : Ferhat, || ex is AdapterException : Custom Exceptionlar icin bir cozum lazim
        if (ex is ArgumentException || ex is FormatException || ex is ApiException) return 400; //Bad Request 
        if (ex is NotImplementedException) return 501; //Not Implemented
        if (ex is SecurityException || ex is UnauthorizedAccessException) return 403; //Forbidden
        return 500; //Internal Server Error
    }
}