using Microsoft.AspNetCore.Mvc.Filters;

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
           
            _logger.LogInformation("### Executando ");
            _logger.LogInformation("######################## ");
            _logger.LogInformation($"### Executando em {DateTime.UtcNow.ToLongTimeString()} ");
            _logger.LogInformation($"### Executando em {DateTime.UtcNow.ToLongDateString()}");

            _logger.LogInformation($"ModelState: {context.ModelState.IsValid}");


            _logger.LogInformation($"Status code: {context.HttpContext.Response.StatusCode}");


            _logger.LogInformation($"Finalizando: {next.Method.Name}");



            
            //// DEVO VOLTAR AOS FILTROS DEPOIS 




        }
    }
}
