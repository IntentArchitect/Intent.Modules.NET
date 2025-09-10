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
using Intent.Modules.MongoDb.Templates.MongoConfigurationExtensions;
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
            var dependencyInjection = application.FindTemplateInstance<ICSharpFileBuilderTemplate>(TemplateRoles.Infrastructure.DependencyInjection);
            if (dependencyInjection == null)
            {
                return;
            }

            dependencyInjection.CSharpFile.AfterBuild(file =>
            {
                file.AddUsing("MongoDB.Driver");
                file.AddUsing("Microsoft.Extensions.DependencyInjection.Extensions");
                file.AddUsing("System");

                dependencyInjection.GetTypeName(MongoConfigurationExtensionsTemplate.TemplateId);

                var method = file.Classes.First().FindMethod("AddInfrastructure");

                if (application.Settings.GetMultitenancySettings()?.MongoDbDataIsolation()?.IsSeparateDatabase() == true)
                {
                    dependencyInjection.AddNugetDependency(NugetPackages.FinbuckleMultiTenant(dependencyInjection.OutputTarget));
                    method.AddStatement($"services.AddSingleton<{dependencyInjection.GetMongoDbMultiTenantConnectionFactoryName()}>();");

                    var teneantConnectionsTemplate = dependencyInjection.GetTemplate<ICSharpFileBuilderTemplate>("Intent.Modules.AspNetCore.MultiTenancy.TenantConnectionsInterfaceTemplate");
                    dependencyInjection.AddUsing("Finbuckle.MultiTenant");
                    method.AddStatement(@$"services.AddScoped<IMongoDatabase>(provider =>
                    {{
                        var tenantConnections = provider.GetService<{dependencyInjection.GetTypeName(teneantConnectionsTemplate)}>();
                        if (tenantConnections is null || tenantConnections.MongoDbConnection is null)
                        {{
                            throw new Finbuckle.MultiTenant.MultiTenantException(""Failed to resolve tenant MongoDb connection information"");
                        }}
                        return provider.GetRequiredService<{dependencyInjection.GetMongoDbMultiTenantConnectionFactoryName()}>().GetConnection(tenantConnections.MongoDbConnection);
                    }});");

                    method.AddStatement("services.RegisterMongoCollections(typeof(DependencyInjection).Assembly);");

                    foreach (var model in dependencyInjection.ExecutionContext.FindTemplateInstances<ICSharpFileBuilderTemplate>("Intent.MongoDb.MongoDbDocument"))
                    {
                        dependencyInjection.GetTypeName(model);
                        if (!model.CSharpFile.Classes.First().IsAbstract)
                        {
                            if (model.ClassName == "BaseTypeDocument")
                            {
                                var p = 1;
                            }
                            method.AddStatement(@$"services.AddScoped<IMongoCollection<{model.ClassName}>>(sp =>
                            {{
                                var database = sp.GetRequiredService<IMongoDatabase>();
                                return database.GetCollection<{model.ClassName}>(""{model.ClassName.Replace("Document", "")}"");
                            }});");
                        }
                    }
                }
                else
                {
                    method.AddStatement("var cs = configuration.GetConnectionString(\"MongoDbConnection\");");
                    method.AddStatement("services.TryAddSingleton<IMongoClient>(_ => new MongoClient(cs));");


                    method.AddStatement(@$"services.TryAddSingleton<IMongoDatabase>(sp =>
                    {{
                        var dbName = new MongoUrl(cs).DatabaseName
                                     ?? throw new InvalidOperationException(
                                         ""MongoDbConnection must include a database name."");
                        return sp.GetRequiredService<IMongoClient>().GetDatabase(dbName);
                    }});");

                    method.AddStatement("services.RegisterMongoCollections(typeof(DependencyInjection).Assembly);");

                    foreach (var model in dependencyInjection.ExecutionContext.FindTemplateInstances<ICSharpFileBuilderTemplate>("Intent.MongoDb.MongoDbDocument"))
                    {
                        dependencyInjection.GetTypeName(model);
                        if (!model.CSharpFile.Classes.First().IsAbstract)
                        {
                            if (model.ClassName == "BaseTypeDocument")
                            {
                                var p = 1;
                            }
                            method.AddStatement(@$"services.AddSingleton<IMongoCollection<{model.ClassName}>>(sp =>
                            {{
                                var database = sp.GetRequiredService<IMongoDatabase>();
                                return database.GetCollection<{model.ClassName}>(""{model.ClassName.Replace("Document", "")}"");
                            }});");
                        }
                    }
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
            //application.EventDispatcher.Publish(ContainerRegistrationRequest
            //    .ToRegister(dbContext)
            //    .ForInterface(dbContext.GetTemplate<IClassProvider>(MongoDbUnitOfWorkInterfaceTemplate.TemplateId))
            //    .ForConcern("Infrastructure")
            //    .WithResolveFromContainer());
        }
    }
}