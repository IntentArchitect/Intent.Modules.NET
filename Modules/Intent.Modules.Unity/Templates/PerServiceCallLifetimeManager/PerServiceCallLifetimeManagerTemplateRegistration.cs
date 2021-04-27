using System.ComponentModel;
using Intent.Engine;
using Intent.Modules.Common.Registrations;
using Intent.Templates;

namespace Intent.Modules.Unity.Templates.PerServiceCallLifetimeManager
{
    [Description(PerServiceCallLifetimeManagerTemplate.Identifier)]
    public class PerServiceCallLifetimeManagerTemplateRegistration : SingleFileTemplateRegistration
    {
        public override string TemplateId => PerServiceCallLifetimeManagerTemplate.Identifier;

        public override ITemplate CreateTemplateInstance(IOutputTarget outputTarget)
        {
            return new PerServiceCallLifetimeManagerTemplate(outputTarget);
        }
    }
}

