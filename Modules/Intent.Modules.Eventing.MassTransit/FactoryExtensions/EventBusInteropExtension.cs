using System.Linq;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using Intent.Modules.EntityFrameworkCore.Shared;
using Intent.Modules.Eventing.Contracts.Settings;
using Intent.Modules.Eventing.Contracts.Templates;
using Intent.Modules.Eventing.MassTransit.Settings;
using Intent.Modules.Eventing.MassTransit.Templates.FinbucklePublishingFilter;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.Eventing.MassTransit.FactoryExtensions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class EventBusInteropExtension : FactoryExtensionBase
    {
        public override string Id => "Intent.Eventing.MassTransit.EventBusInteropExtension";

        [IntentManaged(Mode.Ignore)]
        public override int Order => 0;

        protected override void OnBeforeTemplateExecution(IApplication application)
        {
            InstallMessageBusForServiceContractDispatch(application);
            InstallMessageBusForMediatRDispatch(application);
        }

        protected override void OnAfterTemplateRegistrations(IApplication application)
        {
            InstallMessageBusForDbContextForTransactionalOutboxPattern(application);
        }

        private void InstallMessageBusForMediatRDispatch(IApplication application)
        {
            if (!IsTransactionalOutboxPatternSelected(application))
            {
                return;
            }

            var template = application.FindTemplateInstance<ICSharpFileBuilderTemplate>("Intent.Application.DependencyInjection.DependencyInjection"); // Replace with Role later.
            if (template == null)
            {
                return;
            }

            template.CSharpFile.AfterBuild(file =>
            {
                var priClass = file.Classes.First();
                var method = priClass.FindMethod("AddApplication");
                var meditarConfigLambda = (CSharpInvocationStatement)method.FindStatement(stmt => stmt.HasMetadata("mediatr-config"));
                if (meditarConfigLambda == null)
                {
                    return;
                }
                var mediatorConfig = (CSharpLambdaBlock)(meditarConfigLambda.Statements.FirstOrDefault());
                var statementToMove = mediatorConfig.Statements.FirstOrDefault(stmt => stmt.GetText("").Contains("EventBusPublishBehaviour"));
                if (statementToMove == null)
                {
                    return;
                }
                statementToMove.Remove();
            }, 1000);
        }

        private void InstallMessageBusForServiceContractDispatch(IApplication application)
        {
            if (!IsTransactionalOutboxPatternSelected(application))
            {
                return;
            }

            var templates = application.FindTemplateInstances<ICSharpFileBuilderTemplate>(TemplateDependency.OnTemplate(TemplateRoles.Application.Services.Controllers));
            foreach (var template in templates)
            {
                template.CSharpFile.AfterBuild(file =>
                {
                    var priClass = file.Classes.First();
                    foreach (var method in priClass.Methods)
                    {
                        var statementToMove = method.FindStatement(stmt => stmt.HasMetadata("eventbus-flush"));
                        if (statementToMove == null)
                        {
                            continue;
                        }
                        statementToMove.Remove();
                    }
                }, 1000);
            }
        }

        private void InstallMessageBusForDbContextForTransactionalOutboxPattern(IApplication application)
        {
            if (!IsTransactionalOutboxPatternSelected(application))
            {
                return;
            }

            var template = application.FindTemplateInstance<ICSharpFileBuilderTemplate>(TemplateRoles.Infrastructure.Data.DbContext);
            if (template is null)
            {
                return;
            }

            template.CSharpFile.OnBuild(file =>
            {
                var @class = file.Classes.First();
                var busInterface = template.GetBusInterfaceName();
                var busParameterName = template.ExecutionContext.Settings.GetEventingSettings().UseLegacyInterfaceName() 
                    ? "eventBus" 
                    : "messageBus";
                var constructor = @class.Constructors.First();
                if (!constructor.Parameters.Any(p => p.Type == busInterface))
                {
                    constructor.AddParameter(busInterface, busParameterName, p => p.IntroduceReadonlyField());
                }

                var busFieldName = template.ExecutionContext.Settings.GetEventingSettings().UseLegacyInterfaceName() 
                    ? "_eventBus" 
                    : "_messageBus";

                var method = template.GetSaveChangesMethod();
                var statement = method.FindStatement(stmt => stmt.GetText("") == "DispatchEventsAsync().GetAwaiter().GetResult();");
                if (statement != null)
                {
                    statement.InsertBelow((CSharpStatement)$"{busFieldName}.FlushAllAsync().GetAwaiter().GetResult();");
                }
                else
                {
                    statement = method.FindStatement(stmt => stmt.GetText("").Contains("base.SaveChanges"));
                    if (statement != null)
                    {
                        statement.InsertAbove($"{busFieldName}.FlushAllAsync().GetAwaiter().GetResult();");
                    }
                }

                method = template.GetSaveChangesAsyncMethod();
                method = @class.FindMethod("SaveChangesAsync");
                statement = method.FindStatement(stmt => stmt.GetText("") == "await DispatchEventsAsync(cancellationToken);");
                if (statement != null)
                {
                    statement.InsertBelow((CSharpStatement)$"await {busFieldName}.FlushAllAsync(cancellationToken);");
                }
                else
                {
                    statement = method.FindStatement(stmt => stmt.GetText("").Contains("base.SaveChanges"));
                    if (statement != null)
                    {
                        statement.InsertAbove($"await {busFieldName}.FlushAllAsync(cancellationToken);");
                    }
                }

            }, 10);

        }

        private static bool IsTransactionalOutboxPatternSelected(IApplication application)
        {
            return application.Settings.GetMassTransitMessageBusSettings()?.OutboxPattern()?.IsEntityFramework() == true;
        }
    }
}