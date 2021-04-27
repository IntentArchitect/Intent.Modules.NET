using System.ComponentModel;
using Intent.Engine;
using Intent.Modules.Common.Registrations;
using Intent.Templates;

namespace Intent.Modules.Unity.Templates.UnityConfig
{
    [Description(UnityConfigTemplate.Identifier)]
    public class UnityConfigTemplateRegistration : SingleFileTemplateRegistration
    {
        public override string TemplateId => UnityConfigTemplate.Identifier;

        public override ITemplate CreateTemplateInstance(IOutputTarget outputTarget)
        {
            return new UnityConfigTemplate(outputTarget, outputTarget.Application.EventDispatcher);
        }
    }
}
