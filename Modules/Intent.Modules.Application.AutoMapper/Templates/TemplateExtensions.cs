using System.Collections.Generic;
using Intent.Modules.Application.AutoMapper.Templates.MapFromInterface;
using Intent.Modules.Application.AutoMapper.Templates.MappingProfile;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateExtensions", Version = "1.0")]

namespace Intent.Modules.Application.AutoMapper.Templates
{
    public static class TemplateExtensions
    {
        public static string GetMapFromInterfaceName<T>(this IntentTemplateBase<T> template)
        {
            return template.GetTypeName(MapFromInterfaceTemplate.TemplateId);
        }

        public static string GetMappingProfileName<T>(this IntentTemplateBase<T> template)
        {
            return template.GetTypeName(MappingProfileTemplate.TemplateId);
        }

    }
}