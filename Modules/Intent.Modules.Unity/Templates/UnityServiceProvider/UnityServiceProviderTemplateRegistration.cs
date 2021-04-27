using System.ComponentModel;
using Intent.Engine;
using Intent.Modules.Common.Registrations;
using Intent.Templates;

namespace Intent.Modules.Unity.Templates.UnityServiceProvider
{
    [Description(UnityServiceProviderTemplate.Identifier)]
    public class IdentityGeneratorTemplateRegistration : SingleFileTemplateRegistration
    {
        public override string TemplateId => UnityServiceProviderTemplate.Identifier;

        public override ITemplate CreateTemplateInstance(IOutputTarget outputTarget)
        {
            return new UnityServiceProviderTemplate(outputTarget);
        }
    }
}
