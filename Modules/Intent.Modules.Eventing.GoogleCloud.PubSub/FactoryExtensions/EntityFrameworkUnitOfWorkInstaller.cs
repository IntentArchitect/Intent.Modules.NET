using System.Linq;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.Eventing.GoogleCloud.PubSub.FactoryExtensions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class EntityFrameworkUnitOfWorkInstaller : FactoryExtensionBase
    {
        public override string Id => "Intent.Eventing.GoogleCloud.PubSub.EntityFrameworkUnitOfWorkInstaller";

        [IntentManaged(Mode.Ignore)]
        public override int Order => 0;

        protected override void OnAfterTemplateRegistrations(IApplication application)
        {
            InstallGoogleCloudPubSubBackgroundServiceInterop(application);
            InstallGoogleCloudPubSubWebhookInterop(application);
        }

        private void InstallGoogleCloudPubSubWebhookInterop(IApplication application)
        {
            var templates = application.FindTemplateInstances<ICSharpFileBuilderTemplate>(TemplateDependency.OnTemplate("Distribution.Controller.PubSub"));
            foreach (var template in templates)
            {
                template.AddUsing("System.Transactions");
                template.AddUsing("Microsoft.Extensions.DependencyInjection");
                template.CSharpFile.AfterBuild(file =>
                {
                    var priClass = file.Classes.First();
                    var method = priClass.FindMethod("RequestHandler");
                    var body = method.Statements.ToList();
                    body.ForEach(x => method.Statements.Remove(x));
                    method.InsertStatement(0,
                        new CSharpStatementBlock(
                                $"using (var transaction = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions() {{ IsolationLevel = IsolationLevel.ReadCommitted }}, TransactionScopeAsyncFlowOption.Enabled))")
                            .AddStatement($"var unitOfWork = _serviceProvider.GetService<{GetUnitOfWorkName(template)}>();")
                            .AddStatements(body)
                            .AddStatement($"await unitOfWork.SaveChangesAsync(cancellationToken);")
                            .AddStatement($"transaction.Complete();"));
                }, -200);
            }
        }

        private void InstallGoogleCloudPubSubBackgroundServiceInterop(IApplication application)
        {
            var templates = application.FindTemplateInstances<ICSharpFileBuilderTemplate>(TemplateDependency.OnTemplate("Infrastructure.Eventing.GoogleBackgroundService"));
            foreach (var template in templates)
            {
                template.AddUsing("System.Transactions");
                template.AddUsing("Microsoft.Extensions.DependencyInjection");
                template.CSharpFile.AfterBuild(file =>
                {
                    var priClass = file.Classes.First();
                    var method = priClass.FindMethod("RequestHandler");
                    var tryBlock = (CSharpStatementBlock)method.Statements.First();
                    var body = tryBlock.Statements.Where(p => !p.HasMetadata("create-scope") && !p.HasMetadata("return")).ToList();
                    body.ForEach(x => tryBlock.Statements.Remove(x));
                    tryBlock.InsertStatement(1,
                        new CSharpStatementBlock(
                                $"using (var transaction = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions() {{ IsolationLevel = IsolationLevel.ReadCommitted }}, TransactionScopeAsyncFlowOption.Enabled))")
                            .AddStatement($"var unitOfWork = scope.ServiceProvider.GetService<{GetUnitOfWorkName(template)}>();")
                            .AddStatements(body)
                            .AddStatement($"await unitOfWork.SaveChangesAsync(cancellationToken);")
                            .AddStatement($"transaction.Complete();"));
                }, -200);
            }
        }

        private string GetUnitOfWorkName(ICSharpFileBuilderTemplate template)
        {
            if (template.TryGetTypeName(TemplateRoles.Domain.UnitOfWork, out var unitOfWorkTypeName) ||
                template.TryGetTypeName(TemplateRoles.Application.Common.DbContextInterface, out unitOfWorkTypeName))
            {
                return unitOfWorkTypeName;
            }

            return null;
        }
    }
}