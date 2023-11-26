using System.Diagnostics;
using System.Web.Http;
using FCMicroservices.Extensions;
using FCMicroservices.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace FCMicroservices.Components.Filters;

public class ApiExceptionFilter : IActionFilter, IOrderedFilter
{
    private static readonly ActivitySource _src = new(AssemblyUtils.API_FULL_NAME);
    public int Order => int.MaxValue - 10;

    public void OnActionExecuting(ActionExecutingContext context) { }

    public void OnActionExecuted(ActionExecutedContext context)
    {
        Exception ex = context.Exception;
        if (ex == null) return;
        
        if (ex is ApiException apiex)
        {
            using var activity = _src.StartActivity("ApiException");
            activity?.SetTag("Message", ex.Message);
            activity?.SetTag("Exception", ex.ToString());
            activity?.SetStatus(ActivityStatusCode.Error);
            context.Result = new ObjectResult(new
            {
                apiex.ErrorCode,
                apiex.StatusCode,
                apiex.Data,
                apiex.Message,
            })
            {
                StatusCode = 400
            };
        }
        else 
        {
            using var activity = _src.StartActivity("Exception");
            activity?.SetTag("Message", ex.Message);
            activity?.SetTag("Exception", ex.ToString());
            activity?.SetStatus(ActivityStatusCode.Error);
            var content = ex.Message + Environment.NewLine + Environment.NewLine + ex.StackTrace;
            context.Result = new ContentResult()
            {
                StatusCode = 500,
                ContentType = "text/html",
                Content = content
            };

        }
        
        context.ExceptionHandled = true;

    }
}