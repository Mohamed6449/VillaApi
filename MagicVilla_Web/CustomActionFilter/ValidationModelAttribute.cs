using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace MagicVilla_Web.CustomActionFilter
{
    public class ValidationModelAttribute: ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (context.ModelState.IsValid)
            {
                return;
            }
            context.Result = new BadRequestObjectResult(context.ModelState);
        }
    }
}
