using System.Linq;
using System.Reflection;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using Intent.Modules.Entities.Repositories.Api.Templates.EntityRepositoryInterface;
using Intent.Modules.Entities.Repositories.Api.Templates.RepositoryInterface;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.Entities.Repositories.Api.FactoryExtensions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class CustomDbProviderFactoryExtension : FactoryExtensionBase
    {
        public override string Id => "Intent.Entities.Repositories.Api.CustomDbProviderFactoryExtension";

        [IntentManaged(Mode.Ignore)]
        public override int Order => 0;

        protected override void OnAfterTemplateRegistrations(IApplication application)
        {
            var templates = application.FindTemplateInstances<ICSharpFileBuilderTemplate>(TemplateDependency.OnTemplate(EntityRepositoryInterfaceTemplate.TemplateId));
            foreach (var template in templates)
            {
                var templateModel = ((CSharpTemplateBase<ClassModel>)template).Model;
                if (!FilterCustomDbProvider(templateModel))
                {
                    continue;
                }
                template.CSharpFile.OnBuild(file =>
                {
                    var @interface = file.Interfaces.First();
                    //Strip the first generic parameter off the IRepository<IDomainInterface, IDomain> to IRepository<IDomain> as this is more in-line with what a generic repo looks like
                    @interface.Interfaces[0] = $"{template.GetTypeName(RepositoryInterfaceTemplate.TemplateId)}<{template.GetTypeName("Domain.Entity", ((CSharpTemplateBase<ClassModel>)template).Model)}>";
                });
            }
        }

        public const string CustomDbProviderId = "3b436f4c-554e-4d97-8968-5167a725e5cb";

        public static bool FilterCustomDbProvider(ClassModel x)
        {
            if (!x.InternalElement.Package.HasStereotype("Document Database"))
                return false;
            var setting = x.InternalElement.Package.GetStereotypeProperty<IElement>("Document Database", "Provider");
            return setting?.Id == CustomDbProviderId;
        }
    }
}