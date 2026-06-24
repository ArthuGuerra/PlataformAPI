using Microsoft.AspNetCore.Mvc.Filters;
using Serilog;

namespace APISolution.Filter
{
    public class ApiLoggingFilter : IAsyncActionFilter
    {
        private readonly ILogger<ApiLoggingFilter> _logger;

        public ApiLoggingFilter (ILogger<ApiLoggingFilter> logger)
        {
            _logger = logger;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
           
            _logger.LogInformation("### Executando");
            _logger.LogInformation("######################## ");
            _logger.LogInformation($"### Executando em {DateTime.UtcNow.ToLocalTime().ToLongTimeString()} ");
            _logger.LogInformation($"### Executando em {DateTime.UtcNow.ToLocalTime().ToLongDateString()}");


            _logger.LogInformation("ModelState: {ModelState}",context.ModelState.IsValid);

            _logger.Log(LogLevel.Information, "texto");

            _logger.LogInformation("Controller: {Controlle} | Action: {Action}", context.RouteData.Values["controller"], context.RouteData.Values["action"]);

            _logger.LogInformation("Rota: {Path}", context.HttpContext.Request.Path);

            _logger.LogInformation("Metodo: {Metodo}", context.HttpContext.Request.Method);

            _logger.LogInformation("Usuario: {Usuario}", context.HttpContext.User.Identity?.Name);


            var inicio = DateTime.UtcNow;
            var exec = await next();
            var tempo = DateTime.UtcNow - inicio;

            _logger.LogInformation("Tempo de execução: {tempo} ms", tempo.TotalMilliseconds);

            _logger.LogInformation($"Finalizando: {next.Method.Name}");

            _logger.LogInformation($"Status code: {exec.HttpContext.Response.StatusCode}");


        }
    }
}
