using System.Linq;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Configuration;
using Intent.Modules.Common.CSharp.DependencyInjection;
using Intent.Modules.Common.CSharp.Multitenancy;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using Intent.Modules.MongoDb.Settings;
using Intent.Modules.MongoDb.Templates;
using Intent.Modules.MongoDb.Templates.ApplicationMongoDbContext;
using Intent.Modules.MongoDb.Templates.MongoDbUnitOfWorkInterface;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;
using static Intent.Modules.Constants.TemplateRoles.Application;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.MongoDb.FactoryExtensions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class DependencyInjectionFactoryExtension : FactoryExtensionBase
    {
        public override string Id => "Intent.MongoDb.DependencyInjectionFactoryExtension";

        [IntentManaged(Mode.Ignore)]
        public override int Order => 0;

        protected override void OnAfterTemplateRegistrations(IApplication application)
        {
            var dbContext = application.FindTemplateInstance<ICSharpTemplate>(TemplateDependency.OnTemplate(ApplicationMongoDbContextTemplate.TemplateId));
            if (dbContext == null)
            {
                return;
            }

            var dependencyInjection = application.FindTemplateInstance<ICSharpFileBuilderTemplate>(TemplateRoles.Infrastructure.DependencyInjection);
            if (dependencyInjection == null)
            {
                return;
            }

            dependencyInjection.CSharpFile.OnBuild(file =>
            {
                file.AddUsing("MongoFramework");
                var method = file.Classes.First().FindMethod("AddInfrastructure");
                method.AddStatement($"services.AddScoped<{dependencyInjection.GetTypeName(dbContext.Id)}>();");

                if (application.Settings.GetMultitenancySettings()?.MongoDbDataIsolation()?.IsSeparateDatabase() == true)
                {
                    dependencyInjection.AddNugetDependency(NugetPackages.FinbuckleMultiTenant(dependencyInjection.OutputTarget));
                    method.AddStatement($"services.AddSingleton<{dependencyInjection.GetMongoDbMultiTenantConnectionFactoryName()}>();");

                    var teneantConnectionsTemplate = dependencyInjection.GetTemplate<ICSharpFileBuilderTemplate>("Intent.Modules.AspNetCore.MultiTenancy.TenantConnectionsInterfaceTemplate");
                    dependencyInjection.AddUsing("Finbuckle.MultiTenant");
                    method.AddStatement(@$"services.AddScoped<IMongoDbConnection>(provider =>
                    {{
                        var tenantConnections = provider.GetService<{dependencyInjection.GetTypeName(teneantConnectionsTemplate)}>();
                        if (tenantConnections is null || tenantConnections.MongoDbConnection is null)
                        {{
                            throw new Finbuckle.MultiTenant.MultiTenantException(""Failed to resolve tenant MongoDb connection information"");
                        }}
                        return provider.GetRequiredService<{dependencyInjection.GetMongoDbMultiTenantConnectionFactoryName()}>().GetConnection(tenantConnections.MongoDbConnection);
                    }});");
                }
                else
                {
                    method.AddStatement("services.AddSingleton<IMongoDbConnection>((c) => MongoDbConnection.FromConnectionString(configuration.GetConnectionString(\"MongoDbConnection\")));");
                }
            });

            if (application.Settings.GetMultitenancySettings()?.MongoDbDataIsolation()?.IsSeparateDatabase() == true)
            {
                application.EventDispatcher.Publish(new MultitenantConnectionStringRegistrationRequest("MongoDbConnection", $"mongodb://localhost/{application.Name.ToCSharpIdentifier()}-{{tenant}}"));
            }
            else
            {
                application.EventDispatcher.Publish(new ConnectionStringRegistrationRequest("MongoDbConnection", $"mongodb://localhost/{application.Name.ToCSharpIdentifier()}", string.Empty));

                application.EventDispatcher.Publish(new InfrastructureRegisteredEvent(Infrastructure.MongoDb.Name)
                    .WithProperty(Infrastructure.MongoDb.Property.ConnectionStringName, "MongoDbConnection"));

            }
            application.EventDispatcher.Publish(ContainerRegistrationRequest
                .ToRegister(dbContext)
                .ForInterface(dbContext.GetTemplate<IClassProvider>(MongoDbUnitOfWorkInterfaceTemplate.TemplateId))
                .ForConcern("Infrastructure")
                .WithResolveFromContainer());
        }
    }
}