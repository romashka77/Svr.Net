using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.SpaServices.Webpack;
using System.Globalization;
using Microsoft.AspNetCore.Localization;

namespace Svr.Web
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        public Startup(IConfiguration configuration)
        {

            Configuration = configuration;
        }

        //Этот метод вызывается средой выполнения. Используйте этот метод для добавления служб в контейнер.
        public void ConfigureServices(IServiceCollection services)
        {
            // Localization & options
            // добавление ApplicationDbContext для взаимодействия с базой данных
            services.AddDbContext();

            //Добавление служб приложений.
            services.AddRepository();
            services.AddTransientServices();
            services.AddMvc();
        }

        //Этот метод вызывается средой выполнения. Используйте этот метод для настройки конвейера HTTP-запросов.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {

            if (env.IsDevelopment()) //Development/Production
            {
                loggerFactory.AddConsole();
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
                //app.UseWebpackDevMiddleware(new WebpackDevMiddlewareOptions {HotModuleReplacement =true,ReactHotModuleReplacement = true}); // мы включаем поддержку webpack, теперь у нас не будет болеть голова за сборку клиентских ресурсов и нам не нужно будет каждый раз вручную ее запускать, прописывать куда нибудь в PostBuildEvents или держать открытым окошко консоли с запущенной webpack –watch. Этой строкой мы создаем инстанс webpack-а в памяти, который будет отслеживать изменения в файлах и запускать инкрементальную сборку. 
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            var supportedCultures = new[]
           {
                new CultureInfo("ru-RU"),
                new CultureInfo("ru"),
            };
            app.UseRequestLocalization(new RequestLocalizationOptions
            {
                DefaultRequestCulture = new RequestCulture("ru-RU"),
                SupportedCultures = supportedCultures,
                SupportedUICultures = supportedCultures
            });


            app.UseStaticFiles();

            app.UseAuthentication();

            app.UseMvc(routes =>
            {
                
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
                //routes.MapRoute(
                //    name: "defaultApi",
                //    template: "api/{controller}/{id?}");
            });
        }
    }
}
