using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using Intent.Modules.Eventing.Contracts.Templates.EventBusInterface;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.Eventing.GoogleCloud.PubSub.FactoryExtensions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class ControllerMessageBusPublishInstaller : FactoryExtensionBase
    {
        public override string Id => "Intent.Eventing.GoogleCloud.PubSub.ControllerMessageBusPublishInstaller";

        [IntentManaged(Mode.Ignore)]
        public override int Order => 0;

        protected override void OnAfterTemplateRegistrations(IApplication application)
        {
            if (!IntegrationCoordinator.ShouldInstallStandardIntegration(application))
            {
                return;
            }

            var controllerTemplates = application.FindTemplateInstances<ICSharpFileBuilderTemplate>(TemplateDependency.OnTemplate(TemplateFulfillingRoles.Application.Services.Controllers));
            foreach (var template in controllerTemplates)
            {
                template.CSharpFile.AfterBuild(file =>
                {
                    var @class = file.Classes.First();
                    var ctor = @class.Constructors.First();
                    ctor.AddParameter(template.GetTypeName(EventBusInterfaceTemplate.TemplateId), "eventBus", p =>
                    {
                        p.IntroduceReadonlyField((_, assignment) => assignment.ThrowArgumentNullException());
                    });

                    foreach (var method in @class.Methods)
                    {
                        method.Statements.LastOrDefault(x => x.ToString().Trim().StartsWith("return "))?
                            .InsertAbove("await _eventBus.FlushAllAsync(cancellationToken);");
                    }
                }, order: -100);
            }
        }
    }
}