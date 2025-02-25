using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace VresToRestau2.Models
{
    public class AuthorizeUserAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var userId = context.HttpContext.Session.GetInt32("UserId");
            var adminId = context.HttpContext.Session.GetInt32("AdminId");
            var profId = context.HttpContext.Session.GetInt32("ProfId");

            // If no user is in session, go to the login page.
            if (!userId.HasValue && !adminId.HasValue && !profId.HasValue)
            {
                context.Result = new RedirectToActionResult("Login", "Visitors", null);
            }



            /*var userId = context.HttpContext.Session.GetInt32("UserId");
            if (!userId.HasValue)
            {
                context.Result = new RedirectToActionResult("Login", "Visitors", null);
            }*/
            base.OnActionExecuting(context);
        }

    }
}
