using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.Startup", Version = "1.0")]

namespace IdentityServer4CoHosted
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
            services.AddControllers();

            ConfigureAuthentication(services);
            var idServerBuilder = services.AddIdentityServer();
            CustomIdentityServerConfiguration(idServerBuilder);
            idServerBuilder.AddDeveloperSigningCredential();
            idServerBuilder.AddInMemoryClients(Configuration.GetSection("IdentityServer:Clients"))
                .AddInMemoryApiResources(Configuration.GetSection("IdentityServer:ApiResources"))
                .AddInMemoryApiScopes(Configuration.GetSection("IdentityServer:ApiScopes"))
                .AddInMemoryIdentityResources(Configuration.GetSection("IdentityServer:IdentityResources"));
            services.AddInfrastructure(Configuration);
            ConfigureSwagger(services);

            services.AddTransient<Contracts.ITestService, Services.TestService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseIdentityServer();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
            InitializeSwagger(app);

        }
        private void ConfigureSwagger(IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "IdentityServer4_CoHosted API", Version = "v1" });
            });
        }

        private void InitializeSwagger(IApplicationBuilder app)
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "IdentityServer4_CoHosted API V1");
            });
        }

        private void ConfigureAuthentication(IServiceCollection services)
        {
            JwtSecurityTokenHandler.DefaultMapInboundClaims = false;

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddLocalApi(JwtBearerDefaults.AuthenticationScheme, opt =>
                {
                    opt.SaveToken = true;
                });
        }

        [IntentManaged(Mode.Ignore)]
        private void CustomIdentityServerConfiguration(IIdentityServerBuilder idServerBuilder)
        {
            // Uncomment to have a test user handy
            idServerBuilder.AddTestUsers(new List<IdentityServer4.Test.TestUser>
            {
                new IdentityServer4.Test.TestUser
                {
                    SubjectId = "testuser",
                    Username = "testuser",
                    Password = "P@ssw0rd",
                    IsActive = true,
                    Claims = new[] { new Claim("role", "MyRole") }
                }
            });
        }
    }
}