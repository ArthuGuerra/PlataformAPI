using Asp.Versioning;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace APISolution.Controllers
{
    [ApiVersion("2.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class TesteController : ControllerBase
    {
        [HttpGet("teste")]
        public string GetString()
        {
            return $"api version 2";
        }
    }
}
