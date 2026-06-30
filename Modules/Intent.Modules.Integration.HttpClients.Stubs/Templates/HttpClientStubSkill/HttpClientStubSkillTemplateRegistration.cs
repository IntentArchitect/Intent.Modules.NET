using Intent.Engine;
using Intent.Modules.Common.Registrations;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.TemplateRegistration.SingleFileNoModel", Version = "1.0")]

namespace Intent.Modules.Integration.HttpClients.Stubs.Templates.HttpClientStubSkill
{
    [IntentManaged(Mode.Merge, Body = Mode.Merge, Signature = Mode.Fully)]
    public class HttpClientStubSkillTemplateRegistration : SingleFileTemplateRegistration
    {
        public override string TemplateId => HttpClientStubSkillTemplate.TemplateId;

        [IntentManaged(Mode.Fully)]
        public override ITemplate CreateTemplateInstance(IOutputTarget outputTarget)
        {
            return new HttpClientStubSkillTemplate(outputTarget);
        }
    }
}
