using System.Collections.Generic;
using Intent.Engine;
using Intent.Modules.Common.Templates.StaticContent;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.StaticContentTemplateRegistration", Version = "1.0")]

namespace Intent.Modules.Blazor.Templates.Templates.Client.StaticContentTemplateRegistrations
{
    public class ClientStaticStaticContentTemplateRegistration : StaticContentTemplateRegistration
    {
        public new const string TemplateId = "Intent.Modules.Blazor.Templates.Templates.Client.StaticContentTemplateRegistrations.ClientStaticStaticContentTemplateRegistration";

        public ClientStaticStaticContentTemplateRegistration() : base(TemplateId)
        {
        }

        public override string ContentSubFolder => "static-content";


        public override string[] BinaryFileGlobbingPatterns => new string[] { "*.jpg", "*.png", "*.xlsx", "*.ico", "*.pdf" };


        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public override IReadOnlyDictionary<string, string> Replacements(IOutputTarget outputTarget) => new Dictionary<string, string>
        {
        };
    }
}