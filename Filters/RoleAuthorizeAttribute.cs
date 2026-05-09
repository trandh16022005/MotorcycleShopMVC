using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;

namespace MotorcycleShopMVC.Filters
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
    public class RoleAuthorizeAttribute : Attribute, IAuthorizationFilter
    {
        private readonly string[] _roles;

        public RoleAuthorizeAttribute(params string[] roles)
        {
            _roles = roles ?? Array.Empty<string>();
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            //var userEmail = context.HttpContext.Session.GetString("UserEmail");
            //var userRole = context.HttpContext.Session.GetString("UserRole");

            //// Chưa đăng nhập -> về login
            //if (string.IsNullOrWhiteSpace(userEmail))
            //{
            //    var returnUrl = context.HttpContext.Request.Path + context.HttpContext.Request.QueryString;
            //    context.Result = new RedirectToActionResult("Login", "Account", new { returnUrl });
            //    return;
            //}

            //// Có đăng nhập nhưng không đúng role
            //if (_roles.Length > 0 && (string.IsNullOrWhiteSpace(userRole) || !_roles.Contains(userRole)))
            //{
            //    context.Result = new ForbidResult(); // hoặc RedirectToAction("AccessDenied","Home")
            //}

            var user = context.HttpContext.User;

            // Chưa đăng nhập
            if (user?.Identity == null || !user.Identity.IsAuthenticated)
            {
                var returnUrl = context.HttpContext.Request.Path + context.HttpContext.Request.QueryString;
                context.Result = new RedirectToActionResult("Login", "Account", new { returnUrl });
                return;
            }

            // Có role yêu cầu
            if (_roles.Length > 0)
            {
                var roleClaim = user.FindFirst(ClaimTypes.Role)?.Value;

                if (string.IsNullOrWhiteSpace(roleClaim) ||
                    !_roles.Any(r => string.Equals(r, roleClaim, StringComparison.OrdinalIgnoreCase)))
                {
                    context.Result = new RedirectToActionResult("AccessDenied", "Account", null);
                    return;
                }
            }
        }
    }
}
