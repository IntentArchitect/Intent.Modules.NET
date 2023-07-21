using System.Collections.Generic;
using Intent.Modules.Blazor.HttpClients.AccountController.Templates.AccountServiceHttpClient;
using Intent.Modules.Blazor.HttpClients.AccountController.Templates.AccountServiceInterface;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateExtensions", Version = "1.0")]

namespace Intent.Modules.Blazor.HttpClients.AccountController.Templates
{
    public static class TemplateExtensions
    {
        public static string GetAccountServiceHttpClientTemplateName(this IIntentTemplate template)
        {
            return template.GetTypeName(AccountServiceHttpClientTemplate.TemplateId);
        }

        public static string GetAccountServiceInterfaceTemplateName(this IIntentTemplate template)
        {
            return template.GetTypeName(AccountServiceInterfaceTemplate.TemplateId);
        }

    }
}