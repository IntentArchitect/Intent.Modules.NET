using System.Collections.Generic;
using Intent.Engine;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates.StaticContent;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.StaticContentTemplateRegistration", Version = "1.0")]

namespace Intent.Modules.Blazor.WebAssembly.Templates.StaticContentTemplateRegistrations
{
    public class StaticStaticContentTemplateRegistration : StaticContentTemplateRegistration
    {
        public new const string TemplateId = "Intent.Modules.Blazor.WebAssembly.Templates.StaticContentTemplateRegistrations.StaticStaticContentTemplateRegistration";

        public StaticStaticContentTemplateRegistration() : base(TemplateId)
        {
        }

        public override string ContentSubFolder => "static-content";


        public override string[] BinaryFileGlobbingPatterns => new string[] { "*.jpg", "*.png", "*.xlsx", "*.ico", "*.pdf" };

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public override IReadOnlyDictionary<string, string> Replacements(IOutputTarget outputTarget) => new Dictionary<string, string>
        {
            ["Namespace"] = outputTarget.GetNamespace()
        };

        [IntentManaged(Mode.Ignore)]
        protected override OverwriteBehaviour DefaultOverrideBehaviour => OverwriteBehaviour.OnceOff;
    }
}