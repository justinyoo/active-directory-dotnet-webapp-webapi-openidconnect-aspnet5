namespace TodoListService
{
    using Microsoft.AspNet.Builder;
    using Microsoft.AspNet.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    public partial class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            // Setup configuration sources.
            this.Configuration = (new ConfigurationBuilder().AddJsonFile("config.json")).Build();
        }

        public IConfiguration Configuration { get; set; }

        // Configure is called after ConfigureServices is called.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            this.ConfigureAuth(app);

            app.UseStaticFiles();

            // Add MVC to the request pipeline.
            app.UseMvc(
                routes =>
                    {
                        routes.MapRoute(
                            name: "default",
                            template: "{controller}/{action}/{id?}",
                            defaults: new { controller = "Home", action = "Index" });
                    });
        }

        // This method gets called by a runtime.
        // Use this method to add services to the container
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
        }
    }
}