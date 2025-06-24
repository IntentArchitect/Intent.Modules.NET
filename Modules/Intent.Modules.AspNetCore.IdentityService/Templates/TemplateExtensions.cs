using System.Collections.Generic;
using Intent.Modules.AspNetCore.IdentityService.Templates.EmailSenderOptions;
using Intent.Modules.AspNetCore.IdentityService.Templates.IdentityEmailSender;
using Intent.Modules.AspNetCore.IdentityService.Templates.IdentityEmailSenderInterface;
using Intent.Modules.AspNetCore.IdentityService.Templates.IdentityServiceEmailSenderCollectionExtensions;
using Intent.Modules.AspNetCore.IdentityService.Templates.IdentityServiceManager;
using Intent.Modules.AspNetCore.IdentityService.Templates.IdentityServiceManagerInterface;
using Intent.Modules.AspNetCore.IdentityService.Templates.TokenService;
using Intent.Modules.AspNetCore.IdentityService.Templates.TokenServiceInterface;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateExtensions", Version = "1.0")]

namespace Intent.Modules.AspNetCore.IdentityService.Templates
{
    public static class TemplateExtensions
    {

        public static string GetEmailSenderOptionsName(this IIntentTemplate template)
        {
            return template.GetTypeName(EmailSenderOptionsTemplate.TemplateId);
        }

        public static string GetIdentityEmailSenderName(this IIntentTemplate template)
        {
            return template.GetTypeName(IdentityEmailSenderTemplate.TemplateId);
        }

        public static string GetIdentityEmailSenderInterfaceName(this IIntentTemplate template)
        {
            return template.GetTypeName(IdentityEmailSenderInterfaceTemplate.TemplateId);
        }

        public static string GetIdentityServiceEmailSenderCollectionExtensionsName(this IIntentTemplate template)
        {
            return template.GetTypeName(IdentityServiceEmailSenderCollectionExtensionsTemplate.TemplateId);
        }
        public static string GetIdentityServiceManagerName(this IIntentTemplate template)
        {
            return template.GetTypeName(IdentityServiceManagerTemplate.TemplateId);
        }

        public static string GetIdentityServiceManagerInterfaceName(this IIntentTemplate template)
        {
            return template.GetTypeName(IdentityServiceManagerInterfaceTemplate.TemplateId);
        }

        public static string GetTokenServiceName(this IIntentTemplate template)
        {
            return template.GetTypeName(TokenServiceTemplate.TemplateId);
        }

        public static string GetTokenServiceInterfaceName(this IIntentTemplate template)
        {
            return template.GetTypeName(TokenServiceInterfaceTemplate.TemplateId);
        }

    }
}