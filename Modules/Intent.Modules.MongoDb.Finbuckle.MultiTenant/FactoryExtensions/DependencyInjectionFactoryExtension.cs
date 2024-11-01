using System.Linq;
using Intent.Engine;
using Intent.Modules.AspNetCore.MultiTenancy.Settings;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using Intent.Modules.MongoDb.Finbuckle.MultiTenant.Settings;
using Intent.Modules.MongoDb.Finbuckle.MultiTenant.Templates;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.MongoDb.Finbuckle.MultiTenant.FactoryExtensions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class DependencyInjectionFactoryExtension : FactoryExtensionBase
    {
        public override string Id => "Intent.MongoDb.Finbuckle.MultiTenant.DependencyInjectionFactoryExtension";

        [IntentManaged(Mode.Ignore)]
        public override int Order => 0;

        protected override void OnBeforeTemplateExecution(IApplication application)
        {
            var dbContext = application.FindTemplateInstance<ICSharpTemplate>(TemplateDependency.OnTemplate("Intent.MongoDb.ApplicationMongoDbContext"));
            if (dbContext == null)
            {
                return;
            }

            var dependencyInjection = application.FindTemplateInstance<ICSharpFileBuilderTemplate>(TemplateRoles.Infrastructure.DependencyInjection);
            if (dependencyInjection == null)
            {
                return;
            }

            if (!application.Settings.GetMultitenancySettings().MongoDbDataIsolation().IsNone())
            {
                return;
            }

            dependencyInjection.CSharpFile.OnBuild(file =>
            {
                var method = file.Classes.First().FindMethod("AddInfrastructure");

                //Remove Current Connection registration
                method.FindStatement(s => s.GetText("").StartsWith("services.AddSingleton<IMongoDbConnection>"))?.Remove();

                dependencyInjection.AddNugetDependency(NugetPackages.FinbuckleMultiTenant(dependencyInjection.OutputTarget));
                method.AddStatement($"services.AddSingleton<{dependencyInjection.GetMongoDbConnectionFactoryName()}>();");
                method.AddStatement(@$"services.AddScoped<IMongoDbConnection>(provider =>
                    {{
                        var tenantInfo = provider.GetService<{dependencyInjection.UseType("Finbuckle.MultiTenant.ITenantInfo")}>() ?? throw new Finbuckle.MultiTenant.MultiTenantException(""Failed to resolve tenant info."");
                        return provider.GetRequiredService<{dependencyInjection.GetMongoDbConnectionFactoryName()}>().GetConnection(tenantInfo);
                    }});");
            });
        }
    }
}