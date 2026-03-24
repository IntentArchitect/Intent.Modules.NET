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

namespace Intent.Modules.Application.MediatR.FluentValidation.Templates.BypassPipelineValidationInterface
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class BypassPipelineValidationInterfaceTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.Application.MediatR.FluentValidation.BypassPipelineValidationInterface";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public BypassPipelineValidationInterfaceTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddInterface($"IBypassPipelineValidation", @interface =>
                {
                    @interface.WithComments(
                        """
                        /// <summary>
                        /// Defines a marker interface that, when implemented by a request, instructs the 
                        /// <see cref="ValidationBehaviour{TRequest, TResponse}"/> to skip the execution 
                        /// of all registered validators.
                        /// </summary>
                        /// <remarks>
                        /// Use this interface for specialized requests where standard pipeline validation 
                        /// is redundant or must be deferred to a later stage of processing.
                        /// </remarks>
                        """);
                });
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