using Microsoft.AspNetCore.Diagnostics;
using System.Net;
using System.Net.Mime;
using System.Text.Json;

namespace PowerplantCodingChallenge.API.Infrastructure.Middleware
{
    public static class ErrorHandlerMiddleware
    {
        public static void UseCustomErrors(this IApplicationBuilder app)
        {
            app.Use(WriteResponse);
        }

        private static Task WriteResponse(HttpContext httpContext, Func<Task> next)
            => WriteResponseAsync(httpContext);

        private static async Task WriteResponseAsync(HttpContext httpContext)
        {
            var exceptionDetails = httpContext.Features.Get<IExceptionHandlerFeature>();
            var ex = exceptionDetails?.Error;
            if (ex == null) return;

            var logger = httpContext.RequestServices.GetRequiredService<ILoggerFactory>().CreateLogger(typeof(ErrorHandlerMiddleware));
            logger.LogError(ex, ex.Message);
            await HandleExceptionAsync(httpContext, ex);
        }

        private static async Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            context.Response.ContentType = MediaTypeNames.Application.Json;
            await context.Response.WriteAsync(JsonSerializer.Serialize(ex.Message));
        }
    }
}
