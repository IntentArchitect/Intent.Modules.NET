using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Entities.PrivateSetters.TestApplication.Api.Configuration;
using Entities.PrivateSetters.TestApplication.Api.Filters;
using Entities.PrivateSetters.TestApplication.Application;
using Entities.PrivateSetters.TestApplication.Domain.Common.Interfaces;
using Entities.PrivateSetters.TestApplication.Domain.Repositories;
using Entities.PrivateSetters.TestApplication.Domain.Repositories.Aggregational;
using Entities.PrivateSetters.TestApplication.Domain.Repositories.Compositional;
using Entities.PrivateSetters.TestApplication.Domain.Repositories.Mapping;
using Entities.PrivateSetters.TestApplication.Infrastructure.Persistence;
using Entities.PrivateSetters.TestApplication.Infrastructure.Repositories;
using Entities.PrivateSetters.TestApplication.Infrastructure.Repositories.Aggregational;
using Entities.PrivateSetters.TestApplication.Infrastructure.Repositories.Compositional;
using Entities.PrivateSetters.TestApplication.Infrastructure.Repositories.Mapping;
using Intent.RoslynWeaver.Attributes;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.Startup", Version = "1.0")]

namespace Entities.PrivateSetters.TestApplication.Api
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
            services.ConfigureProblemDetails();
            services.AddScoped<IUnitOfWork, ApplicationDbContext>();
            services.AddTransient<IInvoiceRepository, InvoiceRepository>();
            services.AddTransient<ITagRepository, TagRepository>();
            services.AddTransient<IManyToManyDestRepository, ManyToManyDestRepository>();
            services.AddTransient<IManyToManySourceRepository, ManyToManySourceRepository>();
            services.AddTransient<IManyToOneDestRepository, ManyToOneDestRepository>();
            services.AddTransient<IManyToOneSourceRepository, ManyToOneSourceRepository>();
            services.AddTransient<IOptionalToManyDestRepository, OptionalToManyDestRepository>();
            services.AddTransient<IOptionalToManySourceRepository, OptionalToManySourceRepository>();
            services.AddTransient<IOptionalToOneDestRepository, OptionalToOneDestRepository>();
            services.AddTransient<IOptionalToOneSourceRepository, OptionalToOneSourceRepository>();
            services.AddTransient<IOneToManySourceRepository, OneToManySourceRepository>();
            services.AddTransient<IOneToOneSourceRepository, OneToOneSourceRepository>();
            services.AddTransient<IOneToOptionalSourceRepository, OneToOptionalSourceRepository>();
            services.AddTransient<IMappingRootRepository, MappingRootRepository>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseExceptionHandler();
            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}