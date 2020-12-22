using System.Collections.Generic;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Configuration;
using Intent.Modules.EntityFrameworkCore;
using Intent.Modules.EntityFrameworkCore.Templates.DbContext;
using Intent.Modules.EntityFrameworkCore.Templates.DbContextInterface;
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

        private readonly DependencyInjectionTemplate _template;

        public EntityFrameworkCoreDependencyInjectionDecorator(DependencyInjectionTemplate template)
        {
            _template = template;
            _template.AddNugetDependency(NugetPackages.EntityFrameworkCoreSqlServer);
            _template.AddNugetDependency(NugetPackages.EntityFrameworkCoreInMemory);

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
            return $@"
            if (configuration.GetValue<bool>(""UseInMemoryDatabase""))
            {{
                services.AddDbContext<{_template.GetTypeName(DbContextTemplate.Identifier)}>(options =>
                    options.UseInMemoryDatabase(""{_template.OutputTarget.Application.Name}""));
            }}
            else
            {{
                services.AddDbContext<{_template.GetTypeName(DbContextTemplate.Identifier)}>(options =>
                    options.UseSqlServer(
                        configuration.GetConnectionString(""DefaultConnection""),
                        b => b.MigrationsAssembly(typeof({_template.GetTypeName(DbContextTemplate.Identifier)}).Assembly.FullName)));
            }}

            services.AddScoped<{_template.GetTypeName(DbContextInterfaceTemplate.Identifier)}>(provider => provider.GetService<{_template.GetTypeName(DbContextTemplate.Identifier)}>());";
        }

        public IEnumerable<string> DeclareUsings()
        {
            yield return "Microsoft.EntityFrameworkCore";
        }
    }
}