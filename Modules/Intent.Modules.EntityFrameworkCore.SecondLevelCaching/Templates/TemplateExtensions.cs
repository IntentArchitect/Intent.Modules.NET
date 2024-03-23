using System.Collections.Generic;
using Intent.Modules.Common.Templates;
using Intent.Modules.EntityFrameworkCore.SecondLevelCaching.Templates.DistributedCacheServiceProvider;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateExtensions", Version = "1.0")]

namespace Intent.Modules.EntityFrameworkCore.SecondLevelCaching.Templates
{
    public static class TemplateExtensions
    {
        public static string GetDistributedCacheServiceProviderName(this IIntentTemplate template)
        {
            return template.GetTypeName(DistributedCacheServiceProviderTemplate.TemplateId);
        }

    }
}