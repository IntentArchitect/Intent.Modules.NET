using System.ComponentModel;
using Intent.Engine;
using Intent.Modules.Common.Registrations;
using Intent.Templates;

namespace Intent.Modules.UserContext.Templates.UserContextProvider
{
    [Description(UserContextProviderTemplate.Identifier)]
    public class UserContextProviderTemplateRegistration : SingleFileTemplateRegistration
    {
        public override string TemplateId => UserContextProviderTemplate.Identifier;

        public override ITemplate CreateTemplateInstance(IOutputTarget project)
        {
            return new UserContextProviderTemplate(project);
        }
    }
}
