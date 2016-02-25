using Microsoft.AspNet.Http.Abstractions;
using Microsoft.AspNet.Builder;
using System;

namespace TodoListService
{
    public partial class Startup
    {
        public void ConfigureAuth(IApplicationBuilder app)
        {
            // Configure the app to use OAuth Bearer Authentication
            app.UseOAuthBearerAuthentication(options =>
            {
                options.Audience = Configuration.GetSection("AzureAd:Audience").Value;
                options.Authority = String.Format(Configuration.GetSection("AzureAd:AadInstance").Value, Configuration.GetSection("AzureAd:Tenant").Value);
            });
        }
    }
}