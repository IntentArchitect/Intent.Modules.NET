using System;
using System.Collections.Generic;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.CSharp.VisualStudio;
using Intent.Modules.Common.Templates;
using Intent.Modules.VisualStudio.Projects.Api;
using Intent.Modules.WindowsServiceHost.Events;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.WindowsServiceHost.Templates.ProgramPartial
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class ProgramPartialTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.WindowsServiceHost.ProgramPartial";

        private bool _exposeProgramClass = false;

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public ProgramPartialTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            ExecutionContext.EventDispatcher.Subscribe<ExposeProgramClassRequest>(Handle);

            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddClass($"Program", @class =>
                {
                    @class.Partial();
                    @class.WithComments("""
                                        // Expose the Program class as public for use cases such as WebApplicationFactories
                                        // to reference when Top Level Statements are used.
                                        """);
                });
        }

        private void Handle(ExposeProgramClassRequest message)
        {
            _exposeProgramClass = true;
        }

        public override bool CanRunTemplate()
        {
            var topLevelStatements = OutputTarget.GetProject()?.InternalElement?.AsCSharpProjectNETModel()?.GetNETSettings()?.UseTopLevelStatements() == true;
            return topLevelStatements && _exposeProgramClass;
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