using System.Linq;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.Plugins;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.Application.Wolverine.FactoryExtensions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class WolverineControllerDispatchExtension : FactoryExtensionBase
    {
        public override string Id => "Intent.Application.Wolverine.WolverineControllerDispatchExtension";

        [IntentManaged(Mode.Ignore)]
        public override int Order => 0;

        /// <summary>
        /// This is an example override which would extend the
        /// <see cref="ExecutionLifeCycleSteps.AfterTemplateRegistrations"/> phase of the Software Factory execution.
        /// See <see cref="FactoryExtensionBase"/> for all available overrides.
        /// </summary>
        /// <remarks>
        /// It is safe to update or delete this method.
        /// </remarks>
        protected override void OnAfterTemplateRegistrations(IApplication application)
        {
            Intent.Utils.Logging.Log.Info("WolverineControllerDispatchExtension running!");
            var controllerTemplates = application.FindTemplateInstances<Intent.Modules.Common.Templates.IntentTemplateBase>(Intent.Modules.Constants.TemplateRoles.Distribution.WebApi.Controller);
            Intent.Utils.Logging.Log.Info($"WolverineControllerDispatchExtension: found {controllerTemplates.Count()} controller templates.");
            foreach (var controllerTemplate in controllerTemplates)
            {
                controllerTemplate.AddTypeSource(Templates.CommandModels.CommandModelsTemplate.TemplateId);
                controllerTemplate.AddTypeSource(Templates.QueryModels.QueryModelsTemplate.TemplateId);
            }
        }

        protected override void OnBeforeTemplateExecution(IApplication application)
        {
        }
    }
}