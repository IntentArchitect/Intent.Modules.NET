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

namespace Intent.Modules.Application.MediatR.FluentValidation.FactoryExtentions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class UnhandledExceptionBahaviorExtension : FactoryExtensionBase
    {
        public override string Id => "Intent.Application.MediatR.FluentValidation.UnhandledExceptionBahaviorExtension";

        [IntentManaged(Mode.Ignore)]
        public override int Order => 0;

        protected override void OnAfterTemplateRegistrations(IApplication application)
        {
            var template = application.FindTemplateInstance<ICSharpFileBuilderTemplate>("Intent.Application.MediatR.Behaviours.UnhandledExceptionBehaviour");
            template?.CSharpFile.OnBuild(file =>
            {
                file.Classes.First()
                    .FindMethod("Handle")
                    ?.FindStatement(s => s is CSharpCatchBlock { ExceptionType: "Exception" })
                    ?.InsertAbove(new CSharpCatchBlock(template.UseType("FluentValidation.ValidationException"), "ex")
                        .AddStatement("// Do not log Fluent Validation Exceptions")
                        .AddStatement("throw;"));
            });
        }
    }
}