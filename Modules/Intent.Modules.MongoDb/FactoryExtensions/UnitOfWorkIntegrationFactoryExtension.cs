using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
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
    public class UnitOfWorkIntegrationFactoryExtension : FactoryExtensionBase
    {
        public override string Id => "Intent.MongoDb.UnitOfWorkIntegrationFactoryExtension";

        [IntentManaged(Mode.Ignore)]
        public override int Order => 0;

        protected override void OnAfterTemplateRegistrations(IApplication application)
        {
            InstallMongoDbUnitOfWorkForStandardIntegration(application);
            InstallMongoDbUnitOfWorkForGoogleCloudPubSubBackgroundServiceIntegration(application);
            InstallMongoDbUnitOfWorkForGoogleCloudPubSubWebhookIntegration(application);
        }

        private void InstallMongoDbUnitOfWorkForGoogleCloudPubSubWebhookIntegration(IApplication application)
        {
            if (!IntegrationCoordinator.ShouldInstallGoogleCloudPubSubIntegration(application))
            {
                return;
            }

            var controllerTemplates =
                application.FindTemplateInstances<ICSharpFileBuilderTemplate>(TemplateDependency.OnTemplate("Distribution.Controller.PubSub"));
            foreach (var template in controllerTemplates)
            {
                template.CSharpFile.AfterBuild(file =>
                {
                    var @class = file.Classes.First();
                    var method = @class.FindMethod("RequestHandler");
                    method.Statements.First()
                        .InsertAbove($"var mongoDbUnitOfWork = _serviceProvider.GetService<{GetAppDbContext(template)}>();");
                    method.Statements.Last()
                        .InsertBelow($"await mongoDbUnitOfWork.SaveChangesAsync(cancellationToken);");
                }, -120);
            }
        }

        private void InstallMongoDbUnitOfWorkForGoogleCloudPubSubBackgroundServiceIntegration(IApplication application)
        {
            if (!IntegrationCoordinator.ShouldInstallGoogleCloudPubSubIntegration(application))
            {
                return;
            }

            var controllerTemplates =
                application.FindTemplateInstances<ICSharpFileBuilderTemplate>(TemplateDependency.OnTemplate("Infrastructure.Eventing.GoogleBackgroundService"));
            foreach (var template in controllerTemplates)
            {
                template.CSharpFile.AfterBuild(file =>
                {
                    var @class = file.Classes.First();
                    var method = @class.FindMethod("RequestHandler");
                    method.FindStatement(p => p.HasMetadata("create-scope"))
                        .InsertBelow($"var mongoDbUnitOfWork = scope.ServiceProvider.GetService<{GetAppDbContext(template)}>();");
                    method.FindStatement(p => p.HasMetadata("return"))
                        .InsertAbove($"await mongoDbUnitOfWork.SaveChangesAsync(cancellationToken);");
                }, -120);
            }
        }

        private void InstallMongoDbUnitOfWorkForStandardIntegration(IApplication application)
        {
            if (!IntegrationCoordinator.ShouldInstallStandardIntegration(application))
            {
                return;
            }

            var controllerTemplates =
                application.FindTemplateInstances<ICSharpFileBuilderTemplate>(TemplateDependency.OnTemplate(TemplateFulfillingRoles.Application.Services.Controllers));
            foreach (var template in controllerTemplates)
            {
                template.CSharpFile.AfterBuild(file =>
                {
                    var @class = file.Classes.First();
                    var ctor = @class.Constructors.First();
                    ctor.AddParameter(template.GetTypeName(MongoDbUnitOfWorkInterfaceTemplate.TemplateId), "mongoDbUnitOfWork", p => { p.IntroduceReadonlyField((_, assignment) => assignment.ThrowArgumentNullException()); });

                    foreach (var method in @class.Methods)
                    {
                        if (method.TryGetMetadata<IHasStereotypes>("model", out var operation) &&
                            operation.HasStereotype("Http Settings") && operation.GetStereotype("Http Settings").GetProperty<string>("Verb") != "GET")
                        {
                            method.Statements.LastOrDefault(x => x.ToString().Contains("return "))
                                ?.InsertAbove($"await _mongoDbUnitOfWork.SaveChangesAsync(cancellationToken);");
                        }
                    }
                }, -150);
            }
        }

        private string GetAppDbContext(ICSharpFileBuilderTemplate template)
        {
            return template.GetTypeName(ApplicationMongoDbContextTemplate.TemplateId);
        }
    }
}