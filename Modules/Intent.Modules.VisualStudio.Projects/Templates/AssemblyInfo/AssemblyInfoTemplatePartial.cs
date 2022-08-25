using Intent.Modules.Common.Templates;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Templates;

namespace Intent.Modules.VisualStudio.Projects.Templates.AssemblyInfo
{
    partial class AssemblyInfoTemplate : IntentFileTemplateBase<object>, ITemplate
    {
        public const string Identifier = "Intent.VisualStudio.Projects.AssemblyInfo";

        public AssemblyInfoTemplate(IOutputTarget outputTarget)
            : base (Identifier, outputTarget, null)
        {
        }

        public override string GetCorrelationId()
        {
            return $"{Identifier}#{OutputTarget.Id}";
        }

        public override ITemplateFileConfig GetTemplateFileConfig()
        {
            return new TemplateFileConfig(
                overwriteBehaviour: OverwriteBehaviour.OverwriteDisabled,
                codeGenType: CodeGenType.Basic,
                fileName: "AssemblyInfo",
                fileExtension: "cs",
                relativeLocation: "Properties"
                );
        }
    }
}
