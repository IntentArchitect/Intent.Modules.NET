using Intent.RoslynWeaver.Attributes;
using Microsoft.Extensions.Options;
using MudBlazor.ExampleApp.Api.Components;
using MudBlazor.ExampleApp.Api.Configuration;
using MudBlazor.ExampleApp.Api.Filters;
using MudBlazor.ExampleApp.Application;
using MudBlazor.ExampleApp.Client;
using MudBlazor.ExampleApp.Client.HttpClients;
using MudBlazor.ExampleApp.Infrastructure;
using MudBlazor.Services;
using Serilog;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.Startup", Version = "1.0")]

namespace MudBlazor.ExampleApp.Api
{
    [IntentManaged(Mode.Merge)]
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {

            services.AddControllers(
                opt =>
                {
                    opt.Filters.Add<ExceptionFilter>();
                });
            services.AddApplication(Configuration);
            services.ConfigureApplicationSecurity(Configuration);
            services.ConfigureHealthChecks(Configuration);
            services.ConfigureProblemDetails();
            services.ConfigureApiVersioning();
            services.AddInfrastructure(Configuration);
            services.ConfigureSwagger(Configuration);
            services.AddClientServices(Configuration);

            services.AddAuthorization();

            services.AddRazorComponents()
                .AddInteractiveWebAssemblyComponents();

            services.AddMudServices();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseWebAssemblyDebugging();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseSerilogRequestLogging();
            app.UseExceptionHandler();
            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseStaticFiles();
            app.UseAntiforgery();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultHealthChecks();
                endpoints.MapControllers();

                endpoints.MapRazorComponents<App>()
                    .AddInteractiveWebAssemblyRenderMode()
                    .AddAdditionalAssemblies(typeof(Client._Imports).Assembly);
            });
            app.UseSwashbuckle(Configuration);
        }
    }
}