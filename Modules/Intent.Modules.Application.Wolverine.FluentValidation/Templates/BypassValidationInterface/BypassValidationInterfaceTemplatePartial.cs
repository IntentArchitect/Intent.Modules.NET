using System;
using System.Collections.Generic;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Application.Wolverine.FluentValidation.Templates.BypassValidationInterface
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class BypassValidationInterfaceTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.Application.Wolverine.FluentValidation.BypassValidationInterface";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public BypassValidationInterfaceTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddInterface("IBypassValidation", @interface =>
                {
                    @interface.WithComments(
                        "/// <summary>" + Environment.NewLine +
                        "/// Defines a marker interface that, when implemented by a message, instructs the" + Environment.NewLine +
                        "/// <c>ValidationMiddleware</c> to skip the execution of all registered validators." + Environment.NewLine +
                        "/// </summary>" + Environment.NewLine +
                        "/// <remarks>" + Environment.NewLine +
                        "/// Use this interface for specialized messages where standard validation" + Environment.NewLine +
                        "/// is redundant or must be deferred to a later stage of processing." + Environment.NewLine +
                        "/// </remarks>");
                });

            FulfillsRole("Application.Common.BypassValidationInterface");
        }

        [IntentManaged(Mode.Fully)]
        public CSharpFile CSharpFile { get; }

        [IntentManaged(Mode.Fully)]
        protected override CSharpFileConfig DefineFileConfig()
        {
            return CSharpFile.GetConfig();
        }

        [IntentManaged(Mode.Fully)]
        public override string TransformText()
        {
            return CSharpFile.ToString();
        }
    }
}
