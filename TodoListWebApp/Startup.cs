namespace TodoListWebApp
{
    using System;

    using Microsoft.AspNet.Authentication;
    using Microsoft.AspNet.Authentication.Cookies;
    using Microsoft.AspNet.Builder;
    using Microsoft.AspNet.Diagnostics;
    using Microsoft.AspNet.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;

    public partial class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            // Setup configuration sources.
            this.Configuration =
                (new ConfigurationBuilder().AddJsonFile("config.json").AddEnvironmentVariables()).Build();
        }

        public IConfiguration Configuration { get; set; }

        // Configure is called after ConfigureServices is called.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerfactory)
        {
            // Configure the HTTP request pipeline.
            // Add the console logger.
            loggerfactory.AddConsole();

            // Add the following to the request pipeline only in development environment.
            if (string.Equals(env.EnvironmentName, "Development", StringComparison.OrdinalIgnoreCase))
            {
                app.UseBrowserLink();
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // Add Error handling middleware which catches all application specific errors and
                // send the request to the following path or controller action.
                app.UseExceptionHandler("/Home/Error");
            }

            // Configure the OpenIdConnect Auth Pipeline and required services.
            this.ConfigureAuth(app);

            // Add MVC to the request pipeline.
            app.UseMvc(
                routes =>
                    {
                        routes.MapRoute(
                            name: "default",
                            template: "{controller}/{action}/{id?}",
                            defaults: new { controller = "Home", action = "Index" });

                        // Uncomment the following line to add a route for porting Web API 2 controllers.
                        // routes.MapWebApiRoute("DefaultApi", "api/{controller}/{id?}");
                    });
        }

        // This method gets called by the runtime.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add MVC services to the services container.
            services.AddMvc();

            // Add Session Middleware
            services.AddCaching();
            services.AddSession();

            // Add Cookie Middleware
            services.Configure<AuthenticationOptions>(
                opt => opt.AuthenticationScheme = CookieAuthenticationDefaults.AuthenticationScheme);
        }
    }
}