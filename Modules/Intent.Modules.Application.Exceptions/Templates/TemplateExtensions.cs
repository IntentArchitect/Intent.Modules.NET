using System.Collections.Generic;
using Intent.Modules.Application.Exceptions.Templates.NotFoundException;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateExtensions", Version = "1.0")]

namespace Intent.Modules.Application.Exceptions.Templates
{
    public static class TemplateExtensions
    {
        public static string GetNotFoundExceptionName(this IIntentTemplate template)
        {
            return template.GetTypeName(NotFoundExceptionTemplate.TemplateId);
        }

    }
}