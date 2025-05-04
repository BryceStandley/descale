using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace WMS.Receiving.API.Filters
{
    public class ApiExceptionFilterAttribute : ExceptionFilterAttribute
    {
        private readonly ILogger<ApiExceptionFilterAttribute> _logger;

        public ApiExceptionFilterAttribute(ILogger<ApiExceptionFilterAttribute> logger)
        {
            _logger = logger;
        }

        public override void OnException(ExceptionContext context)
        {
            _logger.LogError(context.Exception, "Unhandled exception");

            var code = StatusCodes.Status500InternalServerError;
            var result = string.Empty;

            switch (context.Exception)
            {
                case InvalidOperationException invalidOpEx:
                    code = StatusCodes.Status400BadRequest;
                    result = invalidOpEx.Message;
                    break;
                case ArgumentException argEx:
                    code = StatusCodes.Status400BadRequest;
                    result = argEx.Message;
                    break;
                case UnauthorizedAccessException:
                    code = StatusCodes.Status401Unauthorized;
                    result = "Unauthorized access";
                    break;
                case KeyNotFoundException:
                    code = StatusCodes.Status404NotFound;
                    result = "The requested resource was not found";
                    break;
                default:
                    result = "An error occurred while processing your request";
                    break;
            }

            context.Result = new ObjectResult(new { error = result })
            {
                StatusCode = code
            };

            context.ExceptionHandled = true;
        }
    }
}