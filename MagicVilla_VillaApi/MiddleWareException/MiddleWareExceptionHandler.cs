using Microsoft.AspNetCore.Identity;

namespace CQRS_test.MiddleWareException
{
    public class MiddleWareExceptionHandler
    {
        public ILogger<MiddleWareExceptionHandler> _Logger { get; }
        public RequestDelegate _Next { get; }

        public MiddleWareExceptionHandler(ILogger<MiddleWareExceptionHandler> logger,
            RequestDelegate Next)
        {
            _Logger = logger;
            _Next = Next;
        }
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _Next(context);
            }
            catch (Exception ex)
            {
                var IdError= Guid.NewGuid().ToString();
                _Logger.LogError(ex, $"{IdError} : {ex.Message}");
                context.Response.StatusCode =(int) StatusCodes.Status500InternalServerError;
                context.Response.ContentType = "application/json";
                var errorResponse = new
                {
                    Message = "An unexpected error occurred. Please try again later.",
                    ErrorId = IdError 
                };
                await context.Response.WriteAsJsonAsync(errorResponse);
            }
        } 
    }
}
