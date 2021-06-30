using System.ComponentModel;
using Intent.Engine;
using Intent.Modules.Common.Registrations;
using Intent.Templates;

namespace Intent.Modules.EntityFramework.Repositories.Templates.RepositoryBase
{
    [Description(RepositoryBaseTemplate.Identifier)]
    public class RepositoryBaseTemplateRegistration : SingleFileTemplateRegistration
    {
        public override string TemplateId => RepositoryBaseTemplate.Identifier;

        public override ITemplate CreateTemplateInstance(IOutputTarget project)
        {
            return new RepositoryBaseTemplate(project);
        }
    }
}
