using System.Collections.Generic;
using Intent.Modules.AzureFunctions.Templates.AzureFunctionClass;
using Intent.Modules.AzureFunctions.Templates.AzureFunctionClassHelper;
using Intent.Modules.AzureFunctions.Templates.ReturnTypes.ResourceLocationClass;
using Intent.Modules.AzureFunctions.Templates.Startup;
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

        public static string GetAzureFunctionClassHelperName<T>(this IntentTemplateBase<T> template)
        {
            return template.GetTypeName(AzureFunctionClassHelperTemplate.TemplateId);
        }

        public static string GetResourceLocationClassName<T>(this IntentTemplateBase<T> template)
        {
            return template.GetTypeName(ResourceLocationClassTemplate.TemplateId);
        }

        public static string GetStartupName<T>(this IntentTemplateBase<T> template)
        {
            return template.GetTypeName(StartupTemplate.TemplateId);
        }

    }
}