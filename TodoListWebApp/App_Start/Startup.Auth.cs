using Microsoft.AspNet.Builder;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System;
using System.Threading.Tasks;
using TodoListWebApp.Utils;
using Microsoft.IdentityModel.Protocols;

namespace TodoListWebApp
{
    using Microsoft.AspNet.Authentication.OpenIdConnect;

    public partial class Startup
    {
        public static string Authority = String.Empty;
        public static string ClientId = String.Empty;
        public static string AppKey = String.Empty;
        public static string TodoListResourceId = String.Empty;
        public static string TodoListBaseAddress = String.Empty;
        public static string GraphResourceId = String.Empty;

        public void ConfigureAuth(IApplicationBuilder app)
        {
            // Populate AzureAd Configuration Values
            Authority = String.Format(Configuration.GetSection("AzureAd:AadInstance").Value, Configuration.GetSection("AzureAd:Tenant").Value);
            ClientId = Configuration.GetSection("AzureAd:ClientId").Value;
            AppKey = Configuration.GetSection("AzureAd:AppKey").Value;
            TodoListResourceId = Configuration.GetSection("AzureAd:TodoListResourceId").Value;
            TodoListBaseAddress = Configuration.GetSection("AzureAd:TodoListBaseAddress").Value;
            GraphResourceId = Configuration.GetSection("AzureAd:GraphResourceId").Value;

            // Configure the Session Middleware, Used for Storing Tokens
            app.UseSession();

            // Configure OpenId Connect Authentication Middleware
            app.UseCookieAuthentication(options => { });
            app.UseOpenIdConnectAuthentication(
                options =>
                    {
                        options.ClientId = Configuration.GetSection("AzureAd:ClientId").Value;
                        options.Authority = Authority;
                        options.PostLogoutRedirectUri = Configuration.GetSection("AzureAd:PostLogoutRedirectUri").Value;
                        options.Events = new OpenIdConnectEvents()
                                             {
                                                 OnAuthenticationFailed = (context) =>
                                                     {
                                                         context.Response.Redirect($"/Home/error?message={context.Exception.Message}");
                                                         return Task.FromResult(0);
                                                     },
                                                 OnAuthorizationCodeReceived = async (context) =>
                                                     {
                                                         string userObjectId = context.AuthenticationTicket.Principal.FindFirst("http://schemas.microsoft.com/identity/claims/objectidentifier").Value;
                                                         ClientCredential clientCred = new ClientCredential(ClientId, AppKey);
                                                         AuthenticationContext authContext = new AuthenticationContext(Authority, new NaiveSessionCache(userObjectId, context.HttpContext.Session));
                                                         AuthenticationResult authResult = await authContext.AcquireTokenByAuthorizationCodeAsync(context.Code, new Uri(context.RedirectUri), clientCred, Startup.GraphResourceId);
                                                     }
                                                 //options.Notifications = new OpenIdConnectAuthenticationNotifications
                                                 //{
                                                 //    AuthorizationCodeReceived = OnAuthorizationCodeReceived,
                                                 //    AuthenticationFailed = OnAuthenticationFailed
                                                 //};
                                             };
                    });
        }

        //private Task OnAuthenticationFailed(AuthenticationFailedNotification<OpenIdConnectMessage, OpenIdConnectAuthenticationOptions> notification)
        //{
        //    notification.HandleResponse();
        //    notification.Response.Redirect("/Home/Error?message=" + notification.Exception.Message);
        //    return Task.FromResult(0);
        //}

        //public async Task OnAuthorizationCodeReceived(AuthorizationCodeReceivedNotification notification)
        //{
        //    // Acquire a Token for the Graph API and cache it.  In the TodoListController, we'll use the cache to acquire a token to the Todo List API
        //    string userObjectId = notification.AuthenticationTicket.Principal.FindFirst("http://schemas.microsoft.com/identity/claims/objectidentifier").Value;
        //    ClientCredential clientCred = new ClientCredential(ClientId, AppKey);
        //    AuthenticationContext authContext = new AuthenticationContext(Authority, new NaiveSessionCache(userObjectId, notification.HttpContext.Session));
        //    AuthenticationResult authResult = await authContext.AcquireTokenByAuthorizationCodeAsync(
        //        notification.Code, new Uri(notification.RedirectUri), clientCred, Startup.GraphResourceId);
        //}
    }
}