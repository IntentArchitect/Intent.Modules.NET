using System.ComponentModel;
using Intent.Engine;
using Intent.Modules.Common.Registrations;
using Intent.Templates;

namespace Intent.Modules.EntityFramework.Repositories.Templates.PagedList
{
    [Description(PagedListTemplate.Identifier)]
    public class PagedListTemplateRegistration : SingleFileTemplateRegistration
    {
        public override string TemplateId => PagedListTemplate.Identifier;

        public override ITemplate CreateTemplateInstance(IOutputTarget outputTarget)
        {
            return new PagedListTemplate(outputTarget);
        }
    }
}
