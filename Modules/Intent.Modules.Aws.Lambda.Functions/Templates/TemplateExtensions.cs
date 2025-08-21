using System.Collections.Generic;
using Intent.Modules.Aws.Lambda.Functions.Api;
using Intent.Modules.Aws.Lambda.Functions.Templates.FunctionClass;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateExtensions", Version = "1.0")]

namespace Intent.Modules.Aws.Lambda.Functions.Templates
{
    public static class TemplateExtensions
    {
        public static string GetFunctionClassTemplateName<T>(this IIntentTemplate<T> template) where T : ILambdaFunctionContainerModel
        {
            return template.GetTypeName(FunctionClassTemplate.TemplateId, template.Model);
        }

        public static string GetFunctionClassTemplateName(this IIntentTemplate template, ILambdaFunctionContainerModel model)
        {
            return template.GetTypeName(FunctionClassTemplate.TemplateId, model);
        }

    }
}