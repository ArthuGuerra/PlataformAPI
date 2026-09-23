using Microsoft.AspNetCore.Diagnostics;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text.Json;

namespace APISolution.MiddlawareException
{
    public static class ConfigureExceptionHandler
    {
        
        public static void ConfigureExceptionMiddlewareExtensions(this IApplicationBuilder app)
        {
            app.UseExceptionHandler(appError =>
            {
                appError.Run(async context =>
                {

                    var logger = context.RequestServices.GetRequiredService<ILoggerFactory>().CreateLogger("GlobalExceptionHandler");

                    var exceptionFeature = context.Features.Get<IExceptionHandlerFeature>();

                    var exception = exceptionFeature?.Error;

                    var traceId = context.TraceIdentifier;

                    logger.LogError(exception, "Erro nao tratado. TraceId: {traceId}", traceId);


                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

                    context.Response.ContentType = "application/problem+json";

                    var response = new
                    {
                        type = "https://httpstatuses.com/500",
                        title = "Erro interno do servidor",
                        status = 500,
                        traceId
                    };

                    await context.Response.WriteAsync(JsonSerializer.Serialize(response));

                });
            });
        }
    }
}
