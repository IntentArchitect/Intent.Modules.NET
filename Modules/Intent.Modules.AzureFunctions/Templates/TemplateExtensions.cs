using System.Collections.Generic;
using Intent.Modules.AzureFunctions.Templates.AzureFunctionClass;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateExtensions", Version = "1.0")]

namespace Intent.Modules.AzureFunctions.Templates
{
    public static class TemplateExtensions
    {
        public static string GetAzureFunctionClassName<T>(this IntentTemplateBase<T> template) where T : Intent.Modelers.Services.Api.OperationModel
        {
            return template.GetTypeName(AzureFunctionClassTemplate.TemplateId, template.Model);
        }

        public static string GetAzureFunctionClassName(this IntentTemplateBase template, Intent.Modelers.Services.Api.OperationModel model)
        {
            return template.GetTypeName(AzureFunctionClassTemplate.TemplateId, model);
        }

    }
}