using System.Linq;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Plugins;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.Application.Mediator.FluentValidation.FactoryExtensions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class UnhandledExceptionBehaviourExtension : FactoryExtensionBase
    {
        // The id of the core's UnhandledExceptionBehaviour template. A string rather than the core's
        // template class: the core project is referenced with PrivateAssets=All, so only constants are safe.
        private const string UnhandledExceptionBehaviourTemplateId = "Intent.Application.Mediator.UnhandledExceptionBehaviour";

        public override string Id => "Intent.Application.Mediator.FluentValidation.UnhandledExceptionBehaviourExtension";

        [IntentManaged(Mode.Ignore)]
        public override int Order => 0;

        protected override void OnAfterTemplateRegistrations(IApplication application)
        {
            // The core only generates this template when its opt-in setting is on.
            var template = application.FindTemplateInstance<ICSharpFileBuilderTemplate>(UnhandledExceptionBehaviourTemplateId);
            template?.CSharpFile.OnBuild(file =>
            {
                file.Classes.First()
                    .FindMethod("Handle")
                    ?.FindStatement(s => s is CSharpCatchBlock { ExceptionType: "Exception" or "System.Exception" })
                    ?.InsertAbove(new CSharpCatchBlock(template.UseType("FluentValidation.ValidationException"), null)
                        .AddStatement("// Do not log FluentValidation exceptions: they are expected request failures, not unhandled errors.")
                        .AddStatement("throw;"));
            });
        }
    }
}
