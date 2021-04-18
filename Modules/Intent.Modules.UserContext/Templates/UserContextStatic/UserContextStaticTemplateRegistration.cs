using System.ComponentModel;
using Intent.Engine;
using Intent.Modules.Common.Registrations;
using Intent.Templates;

namespace Intent.Modules.UserContext.Templates.UserContextStatic
{
    [Description(UserContextStaticTemplate.Identifier)]
    public class UserContextStaticTemplateRegistration : SingleFileTemplateRegistration
    {
        public override string TemplateId => UserContextStaticTemplate.Identifier;

        public override ITemplate CreateTemplateInstance(IOutputTarget project)
        {
            return new UserContextStaticTemplate(project);
        }
    }
}
