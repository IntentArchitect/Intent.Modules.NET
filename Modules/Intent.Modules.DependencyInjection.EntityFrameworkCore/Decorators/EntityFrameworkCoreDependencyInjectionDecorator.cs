using System.Collections.Generic;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Configuration;
using Intent.Modules.Common.VisualStudio;
using Intent.Modules.Constants;
using Intent.Modules.DependencyInjection.EntityFrameworkCore.Settings;
using Intent.Modules.EntityFrameworkCore;
using Intent.Modules.EntityFrameworkCore.Templates.DbContext;
using Intent.Modules.Infrastructure.DependencyInjection.Templates.DependencyInjection;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateDecorator", Version = "1.0")]

namespace Intent.Modules.DependencyInjection.EntityFrameworkCore.Decorators
{
    [IntentManaged(Mode.Merge)]
    public class EntityFrameworkCoreDependencyInjectionDecorator : DependencyInjectionDecorator, IDeclareUsings
    {
        [IntentManaged(Mode.Fully)]
        public const string DecoratorId = "Intent.DependencyInjection.EntityFrameworkCore.EntityFrameworkCoreDependencyInjectionDecorator";

        [IntentManaged(Mode.Fully)]
        private readonly DependencyInjectionTemplate _template;
        [IntentManaged(Mode.Fully)]
        private readonly IApplication _application;

        [IntentManaged(Mode.Merge, Body = Mode.Ignore)]
        public EntityFrameworkCoreDependencyInjectionDecorator(DependencyInjectionTemplate template, IApplication application)
        {
            _template = template;
            _application = application;
            _template.AddNugetDependency(NugetPackages.EntityFrameworkCoreSqlServer(_template.Project));
            _template.AddNugetDependency(NugetPackages.EntityFrameworkCoreInMemory(_template.Project));
            if (_template.Project.IsNet5App())
            {
                _template.AddNugetDependency("Microsoft.Extensions.Configuration.Binder", "5.0.0");
            }
            if (_template.Project.IsNet6App())
            {
                _template.AddNugetDependency("Microsoft.Extensions.Configuration.Binder", "6.0.0");
            }
            if (_template.ExecutionContext.Settings.GetMultitenancySettings()?.DataIsolation().IsSeparateDatabases() == true)
            {
                _template.AddNugetDependency("Finbuckle.MultiTenant", "6.5.1");
            }

            _template.ExecutionContext.EventDispatcher.Publish(new AppSettingRegistrationRequest(
                key: "UseInMemoryDatabase",
                value: "true"));
            _template.ExecutionContext.EventDispatcher.Publish(new ConnectionStringRegistrationRequest(
                name: "DefaultConnection",
                connectionString: $"Server=.;Initial Catalog={ _template.OutputTarget.Application.Name };Integrated Security=true;MultipleActiveResultSets=True",
                providerName: "System.Data.SqlClient"));
        }

        public override string ServiceRegistration()
        {
            if (_template.ExecutionContext.Settings.GetMultitenancySettings()?.DataIsolation().IsSeparateDatabases() == true)
            {
                _template.AddNugetDependency("Finbuckle.MultiTenant.EntityFrameworkCore", "6.5.1");
                return $@"
            if (configuration.GetValue<bool>(""UseInMemoryDatabase""))
            {{
                services.AddDbContext<{_template.GetTypeName(DbContextTemplate.TemplateId)}>((sp, options) =>
                {{
                    var tenantInfo = sp.GetService<{_template.UseType("Finbuckle.MultiTenant.ITenantInfo")}>() ?? throw new {_template.UseType("Finbuckle.MultiTenant.MultiTenantException")}(""Failed to resolve tenant info."");
                    options.UseInMemoryDatabase(tenantInfo.ConnectionString);
                    options.UseLazyLoadingProxies();
                }});
            }}
            else
            {{
                services.AddDbContext<{_template.GetTypeName(DbContextTemplate.TemplateId)}>((sp, options) =>
                {{
                    var tenantInfo = sp.GetService<{_template.UseType("Finbuckle.MultiTenant.ITenantInfo")}>() ?? throw new {_template.UseType("Finbuckle.MultiTenant.MultiTenantException")}(""Failed to resolve tenant info."");
                    options.UseSqlServer(
                        configuration.GetConnectionString(tenantInfo.ConnectionString),
                        b => b.MigrationsAssembly(typeof({_template.GetTypeName(DbContextTemplate.TemplateId)}).Assembly.FullName));
                    options.UseLazyLoadingProxies();
                }});
            }}

            services.AddScoped<{_template.GetTypeName(TemplateFulfillingRoles.Persistence.UnitOfWorkInterface)}>(provider => provider.GetService<{_template.GetTypeName(DbContextTemplate.TemplateId)}>());";

            }
            return $@"
            if (configuration.GetValue<bool>(""UseInMemoryDatabase""))
            {{
                services.AddDbContext<{_template.GetTypeName(DbContextTemplate.TemplateId)}>(options =>
                {{
                    options.UseInMemoryDatabase(""{_template.OutputTarget.Application.Name}"");
                    options.UseLazyLoadingProxies();
                }});
            }}
            else
            {{
                services.AddDbContext<{_template.GetTypeName(DbContextTemplate.TemplateId)}>(options =>
                {{
                    options.UseSqlServer(
                        configuration.GetConnectionString(""DefaultConnection""),
                        b => b.MigrationsAssembly(typeof({_template.GetTypeName(DbContextTemplate.TemplateId)}).Assembly.FullName));
                    options.UseLazyLoadingProxies();
                }});
            }}

            services.AddScoped<{_template.GetTypeName(TemplateFulfillingRoles.Persistence.UnitOfWorkInterface)}>(provider => provider.GetService<{_template.GetTypeName(DbContextTemplate.TemplateId)}>());";
        }

        public IEnumerable<string> DeclareUsings()
        {
            yield return "Microsoft.EntityFrameworkCore";
        }
    }
}