using System.Linq;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Common.Templates;
using Intent.Modules.Eventing.GoogleCloud.PubSub.Templates.ControllerTemplates.PubSubController;
using Intent.Modules.Eventing.GoogleCloud.PubSub.Templates.EventingTemplates.GoogleSubscriberBackgroundService;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.Eventing.GoogleCloud.PubSub.FactoryExtensions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class MongoDbUnitOfWorkInstaller : FactoryExtensionBase
    {
        public override string Id => "Intent.Eventing.GoogleCloud.PubSub.MongoDbUnitOfWorkInstaller";

        [IntentManaged(Mode.Ignore)]
        public override int Order => 0;

        protected override void OnAfterTemplateRegistrations(IApplication application)
        {
            InstallGoogleCloudPubSubBackgroundServiceInterop(application);
            InstallGoogleCloudPubSubWebhookInterop(application);
        }

        private void InstallGoogleCloudPubSubWebhookInterop(IApplication application)
        {
            if (!InteropCoordinator.ShouldInstallMongoDbUnitOfWork(application))
            {
                return;
            }

            var controllerTemplates =
                application.FindTemplateInstances<ICSharpFileBuilderTemplate>(TemplateDependency.OnTemplate(PubSubControllerTemplate.TemplateId));
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

        private void InstallGoogleCloudPubSubBackgroundServiceInterop(IApplication application)
        {
            if (!InteropCoordinator.ShouldInstallMongoDbUnitOfWork(application))
            {
                return;
            }

            var controllerTemplates =
                application.FindTemplateInstances<ICSharpFileBuilderTemplate>(TemplateDependency.OnTemplate(GoogleSubscriberBackgroundServiceTemplate.TemplateId));
            foreach (var template in controllerTemplates)
            {
                template.CSharpFile.AfterBuild(file =>
                {
                    var @class = file.Classes.First();
                    var method = @class.FindMethod("RequestHandler");
                    var tryBlock = (CSharpStatementBlock)method.Statements.First();
                    tryBlock.FindStatement(p => p.HasMetadata("create-scope"))
                        .InsertBelow($"var mongoDbUnitOfWork = scope.ServiceProvider.GetService<{GetAppDbContext(template)}>();");
                    tryBlock.FindStatement(p => p.HasMetadata("return"))
                        .InsertAbove($"await mongoDbUnitOfWork.SaveChangesAsync(cancellationToken);");
                }, -120);
            }
        }

        private string GetAppDbContext(ICSharpFileBuilderTemplate template)
        {
            return template.GetTypeName("Infrastructure.Data.DbContext.MongoDb");
        }
    }
}