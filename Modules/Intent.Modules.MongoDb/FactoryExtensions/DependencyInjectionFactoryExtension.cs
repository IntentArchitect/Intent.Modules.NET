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

            var dependencyInjection = application.FindTemplateInstance<ICSharpFileBuilderTemplate>(TemplateRoles.Infrastructure.DependencyInjection);
            if (dependencyInjection == null)
            {
                return;
            }

            dependencyInjection.CSharpFile.OnBuild(file =>
            {
                file.AddUsing("MongoFramework");
                file.Classes.First().FindMethod("AddInfrastructure")
                    .AddStatement($"services.AddScoped<{dependencyInjection.GetTypeName(dbContext.Id)}>();")
                    .AddStatement("services.AddSingleton<IMongoDbConnection>((c) => MongoDbConnection.FromConnectionString(configuration.GetConnectionString(\"MongoDbConnection\")));")
                    ;
            });

            application.EventDispatcher.Publish(new ConnectionStringRegistrationRequest("MongoDbConnection", $"mongodb://localhost/{application.Name.ToCSharpIdentifier()}", string.Empty));

            application.EventDispatcher.Publish(new InfrastructureRegisteredEvent(Infrastructure.MongoDb.Name)
                .WithProperty(Infrastructure.MongoDb.Property.ConnectionStringName, "MongoDbConnection"));

            application.EventDispatcher.Publish(ContainerRegistrationRequest
                .ToRegister(dbContext)
                .ForInterface(dbContext.GetTemplate<IClassProvider>(MongoDbUnitOfWorkInterfaceTemplate.TemplateId))
                .ForConcern("Infrastructure")
                .WithResolveFromContainer());
        }
    }
}