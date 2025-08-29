using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MagicVilla_VillaApi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [ApiVersionNeutral]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class ErrorHandlingController : ControllerBase
    {
        [Route("ProcessError")]
        public IActionResult ProcessError([FromServices]IHostEnvironment hostingEnvironment)
        {
            if (hostingEnvironment.IsDevelopment())
            {
                var feature = HttpContext.Features.Get<IExceptionHandlerFeature>();
                return Problem(detail:feature.Error.StackTrace,title:feature.Error.Message,instance:hostingEnvironment.EnvironmentName);

            }
            else
            {
                return Problem();
            }

        }
       

    }
}
