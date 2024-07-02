using System.Collections.Generic;
using Intent.Modules.AspNetCore.DistributedCaching.Templates.DistributedCacheWithUnitOfWork;
using Intent.Modules.AspNetCore.DistributedCaching.Templates.DistributedCacheWithUnitOfWorkInterface;
using Intent.Modules.AspNetCore.DistributedCaching.Templates.IDistributedCacheExtensions;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateExtensions", Version = "1.0")]

namespace Intent.Modules.AspNetCore.DistributedCaching.Templates
{
    public static class TemplateExtensions
    {
        public static string GetDistributedCacheWithUnitOfWorkName(this IIntentTemplate template)
        {
            return template.GetTypeName(DistributedCacheWithUnitOfWorkTemplate.TemplateId);
        }

        public static string GetDistributedCacheWithUnitOfWorkInterfaceName(this IIntentTemplate template)
        {
            return template.GetTypeName(DistributedCacheWithUnitOfWorkInterfaceTemplate.TemplateId);
        }

        public static string GetIDistributedCacheExtensionsName(this IIntentTemplate template)
        {
            return template.GetTypeName(IDistributedCacheExtensionsTemplate.TemplateId);
        }

    }
}