// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace TodoListWebApp.Controllers
{
    using Microsoft.AspNet.Authentication.Cookies;
    using Microsoft.AspNet.Authentication.OpenIdConnect;
    using Microsoft.AspNet.Http.Authentication;
    using Microsoft.AspNet.Mvc;

    public class AccountController : Controller
    {
        // GET: /Account/Login
        [HttpGet]
        public IActionResult Login(string returnUrl = null)
        {
            if (this.HttpContext.User == null || !this.HttpContext.User.Identity.IsAuthenticated)
            {
                return new ChallengeResult(
                    OpenIdConnectDefaults.AuthenticationScheme,
                    new AuthenticationProperties { RedirectUri = "/" });
            }

            return this.RedirectToAction("Index", "Home");
        }

        // GET: /Account/LogOff
        [HttpGet]
        public IActionResult LogOff()
        {
            if (this.HttpContext.User.Identity.IsAuthenticated)
            {
                this.HttpContext.Authentication.SignOutAsync(OpenIdConnectDefaults.AuthenticationScheme);
                this.HttpContext.Authentication.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            }

            return this.RedirectToAction("Index", "Home");
        }
    }
}