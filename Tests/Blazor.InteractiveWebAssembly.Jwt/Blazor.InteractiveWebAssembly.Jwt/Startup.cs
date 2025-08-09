using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blazor.InteractiveWebAssembly.Jwt.Client;
using Blazor.InteractiveWebAssembly.Jwt.Common;
using Blazor.InteractiveWebAssembly.Jwt.Components;
using Blazor.InteractiveWebAssembly.Jwt.Components.Account;
using Blazor.InteractiveWebAssembly.Jwt.Configuration;
using Intent.RoslynWeaver.Attributes;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MudBlazor.Services;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.Startup", Version = "1.0")]

namespace Blazor.InteractiveWebAssembly.Jwt
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
            services.ConfigureProblemDetails();
            services.AddClientServices(Configuration);
            services.AddCascadingAuthenticationState();
            services.AddHttpContextAccessor();
            services.AddHttpClient("jwtClient", client => client.BaseAddress = Configuration.GetValue<Uri?>("TokenEndpoint:Uri"));
            services.AddScoped<IdentityRedirectManager>();
            services.AddScoped<AuthenticationStateProvider, PersistingServerAuthenticationStateProvider>();
            services.AddScoped<IAuthService, JwtAuthService>();
            services.AddAuthorization();
            services.AddAuthentication(options =>
                                    {
                                        options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                                    }).AddCookie();
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
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseExceptionHandler();
            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseStaticFiles();
            app.UseAntiforgery();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorComponents<App>()
                    .AddInteractiveWebAssemblyRenderMode()
                    .AddAdditionalAssemblies(typeof(Client._Imports).Assembly);
            });
        }
    }
}