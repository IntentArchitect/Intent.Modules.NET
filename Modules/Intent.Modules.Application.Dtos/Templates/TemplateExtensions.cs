using System.Collections.Generic;
using Intent.Modelers.Services.Api;
using Intent.Modules.Application.Dtos.Templates.ContractEnumModel;
using Intent.Modules.Application.Dtos.Templates.DtoModel;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.Types.Api;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateExtensions", Version = "1.0")]

namespace Intent.Modules.Application.Dtos.Templates
{
    public static class TemplateExtensions
    {
        public static string GetContractEnumModelName<T>(this IIntentTemplate<T> template) where T : EnumModel
        {
            return template.GetTypeName(ContractEnumModelTemplate.TemplateId, template.Model);
        }

        public static string GetContractEnumModelName(this IIntentTemplate template, EnumModel model)
        {
            return template.GetTypeName(ContractEnumModelTemplate.TemplateId, model);
        }

        public static string GetDtoModelName<T>(this IIntentTemplate<T> template) where T : DTOModel
        {
            return template.GetTypeName(DtoModelTemplate.TemplateId, template.Model);
        }

        public static string GetDtoModelName(this IIntentTemplate template, DTOModel model)
        {
            return template.GetTypeName(DtoModelTemplate.TemplateId, model);
        }

    }
}