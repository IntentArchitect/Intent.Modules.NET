using System.Collections.Generic;
using Intent.Modules.Application.Dtos.Templates.ContractEnumModel;
using Intent.Modules.Application.Dtos.Templates.DtoModel;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateExtensions", Version = "1.0")]

namespace Intent.Modules.Application.Dtos.Templates
{
    public static class TemplateExtensions
    {
        public static string GetContractEnumModelName<T>(this IntentTemplateBase<T> template) where T : Intent.Modules.Common.Types.Api.EnumModel
        {
            return template.GetTypeName(ContractEnumModelTemplate.TemplateId, template.Model);
        }

        public static string GetContractEnumModelName(this IntentTemplateBase template, Intent.Modules.Common.Types.Api.EnumModel model)
        {
            return template.GetTypeName(ContractEnumModelTemplate.TemplateId, model);
        }
        public static string GetDtoModelName<T>(this IntentTemplateBase<T> template) where T : Intent.Modelers.Services.Api.DTOModel
        {
            return template.GetTypeName(DtoModelTemplate.TemplateId, template.Model);
        }

        public static string GetDtoModelName(this IntentTemplateBase template, Intent.Modelers.Services.Api.DTOModel model)
        {
            return template.GetTypeName(DtoModelTemplate.TemplateId, model);
        }

    }
}