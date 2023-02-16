using System.Collections.Generic;
using Intent.Modules.AspNetCore.Identity.AccountController.Templates.AccountController;
using Intent.Modules.AspNetCore.Identity.AccountController.Templates.AccountEmailSender;
using Intent.Modules.AspNetCore.Identity.AccountController.Templates.AccountEmailSenderInterface;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateExtensions", Version = "1.0")]

namespace Intent.Modules.AspNetCore.Identity.AccountController.Templates
{
    public static class TemplateExtensions
    {
        public static string GetAccountControllerName<T>(this IntentTemplateBase<T> template)
        {
            return template.GetTypeName(AccountControllerTemplate.TemplateId);
        }

        public static string GetAccountEmailSenderName<T>(this IntentTemplateBase<T> template)
        {
            return template.GetTypeName(AccountEmailSenderTemplate.TemplateId);
        }

        public static string GetAccountEmailSenderInterfaceName<T>(this IntentTemplateBase<T> template)
        {
            return template.GetTypeName(AccountEmailSenderInterfaceTemplate.TemplateId);
        }

    }
}