using Alura.ListaLeitura.HttpClients;
using Alura.WebAPI.WebApp.Utils;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Alura.ListaLeitura.WebApp
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration config)
        {
            Configuration = config;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(options =>
            {
                options.LoginPath = "/Usuario/Login";
            });

            services.AddHttpContextAccessor();

            var appSettings = Configuration.GetSection("AppSettings").Get<AppSettings>();

            services.AddHttpClient<LivrosApiClient>(client =>
            {
                client.BaseAddress = new System.Uri(appSettings.ApiUrl);
            });

            services.AddHttpClient<AuthApiClient>(client =>
            {
                client.BaseAddress = new System.Uri(appSettings.ApiAuthUrl);
            });

            services.AddOptions();
            services.Configure<AppSettings>(Configuration.GetSection("AppSettings"));

            services.AddMvc();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseStaticFiles();
            app.UseAuthentication();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}