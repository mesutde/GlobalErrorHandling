using System.Net;
using System.Text.Json;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc.Controllers;
using System.ComponentModel.DataAnnotations;

namespace GlobalErrorHandling.Middleware;
public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception error)
        {
            var response = context.Response;
            response.ContentType = "application/json";

            Console.WriteLine("Error: " + error.Message);
            Console.WriteLine("Error Details: " + context.Request.Path);

            if (error.InnerException != null)
                Console.WriteLine("Inner Error: " + error.InnerException.Message);


			var st = new StackTrace(error, true);
			// Get the top stack frame
			var frame = st.GetFrame(0);
			// Get the line number from the stack frame
			var ErrorLine = frame.GetFileLineNumber();

			var controllerActionDescriptor = context
				   .GetEndpoint()
				   .Metadata
				   .GetMetadata<ControllerActionDescriptor>();
			var controllerName = controllerActionDescriptor.ControllerName;
			var actionName = controllerActionDescriptor.ActionName;
			var message = error.Message;






			string logMessage = "Error Message Name : " + message + "\n" +
								"Controller Name : " + controllerName + "\n" +
								"Action Name : " + actionName + "\n" +
								"Error Line Number : " + ErrorLine;



			//Return StatusCode, Message, Details as result
			await response.WriteAsync("Error");
        }
    }
}