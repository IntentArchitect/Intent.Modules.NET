using System.Linq;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Configuration;
using Intent.Modules.Common.CSharp.DependencyInjection;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using Intent.Modules.MongoDb.Templates.ApplicationMongoDbContext;
using Intent.Modules.MongoDb.Templates.MongoDbUnitOfWorkInterface;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;

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
            
            var dependencyInjection = application.FindTemplateInstance<ICSharpFileBuilderTemplate>(TemplateFulfillingRoles.Infrastructure.DependencyInjection);
            if (dependencyInjection == null)
            {
                return;
            }

            dependencyInjection.CSharpFile.OnBuild(file =>
            {
                file.Classes.First().FindMethod("AddInfrastructure")
                    .AddStatement(new CSharpInvocationStatement($"services.AddScoped<{dependencyInjection.GetTypeName(dbContext.Id)}>")
                            .AddArgument(new CSharpLambdaBlock("provider")
                                .AddStatement($@"var connectionString = configuration.GetConnectionString(""MongoDbConnection"");")
                                .AddStatement($@"var url = MongoDB.Driver.MongoUrl.Create(connectionString);")
                                .AddStatement($@"return new {dependencyInjection.GetTypeName(dbContext.Id)}(connectionString, url.DatabaseName);"))
                            .WithArgumentsOnNewLines(),
                        stmt => stmt.AddMetadata("application-mongodb-context", true));
            });
            
            application.EventDispatcher.Publish(new ConnectionStringRegistrationRequest("MongoDbConnection", $"mongodb://localhost/{application.Name}", string.Empty));
            
            application.EventDispatcher.Publish(ServiceConfigurationRequest
                .ToRegister("AddMongoDbUnitOfWork")
                .ForConcern("Infrastructure")
                .RequiresUsingNamespaces("MongoDB.UnitOfWork.Abstractions.Extensions"));
            application.EventDispatcher.Publish(ServiceConfigurationRequest
                .ToRegister($"AddMongoDbUnitOfWork<{dbContext.ClassName}>")
                .ForConcern("Infrastructure")
                .RequiresUsingNamespaces("MongoDB.UnitOfWork.Abstractions.Extensions"));
            application.EventDispatcher.Publish(ContainerRegistrationRequest
                .ToRegister(dbContext)
                .ForInterface(dbContext.GetTemplate<IClassProvider>(MongoDbUnitOfWorkInterfaceTemplate.TemplateId))
                .ForConcern("Infrastructure")
                .WithResolveFromContainer());
            application.EventDispatcher.Publish(ContainerRegistrationRequest
                .ToRegister(dbContext)
                .ForInterface("IMongoDbContext")
                .RequiresUsingNamespaces("MongoDB.Infrastructure")
                .ForConcern("Infrastructure")
                .WithResolveFromContainer());
        }
    }
}