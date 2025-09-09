using System.Collections.Generic;
using Intent.Modules.Aws.Lambda.Functions.Api;
using Intent.Modules.Aws.Lambda.Functions.Templates.JsonResponse;
using Intent.Modules.Aws.Lambda.Functions.Templates.LambdaFunctionClass;
using Intent.Modules.Aws.Lambda.Functions.Templates.Startup;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateExtensions", Version = "1.0")]

namespace Intent.Modules.Aws.Lambda.Functions.Templates
{
    public static class TemplateExtensions
    {
        public static string GetJsonResponseName(this IIntentTemplate template)
        {
            return template.GetTypeName(JsonResponseTemplate.TemplateId);
        }
        public static string GetLambdaFunctionClassTemplateName<T>(this IIntentTemplate<T> template) where T : ILambdaFunctionContainerModel
        {
            return template.GetTypeName(LambdaFunctionClassTemplate.TemplateId, template.Model);
        }

        public static string GetLambdaFunctionClassTemplateName(this IIntentTemplate template, ILambdaFunctionContainerModel model)
        {
            return template.GetTypeName(LambdaFunctionClassTemplate.TemplateId, model);
        }

        public static string GetStartupTemplateName(this IIntentTemplate template)
        {
            return template.GetTypeName(StartupTemplate.TemplateId);
        }

    }
}