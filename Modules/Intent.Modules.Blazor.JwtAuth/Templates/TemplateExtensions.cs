using System.Collections.Generic;
using Intent.Modules.Blazor.JwtAuth.Templates.AccessTokenProvider;
using Intent.Modules.Blazor.JwtAuth.Templates.AuthServiceImplementation;
using Intent.Modules.Blazor.JwtAuth.Templates.AuthServiceInterface;
using Intent.Modules.Blazor.JwtAuth.Templates.JwtAuthenticationStateProvider;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateExtensions", Version = "1.0")]

namespace Intent.Modules.Blazor.JwtAuth.Templates
{
    public static class TemplateExtensions
    {
        public static string GetAccessTokenProviderName(this IIntentTemplate template)
        {
            return template.GetTypeName(AccessTokenProviderTemplate.TemplateId);
        }

        public static string GetAuthServiceImplementationName(this IIntentTemplate template)
        {
            return template.GetTypeName(AuthServiceImplementationTemplate.TemplateId);
        }

        public static string GetAuthServiceInterfaceName(this IIntentTemplate template)
        {
            return template.GetTypeName(AuthServiceInterfaceTemplate.TemplateId);
        }

        public static string GetJwtAuthenticationStateProviderName(this IIntentTemplate template)
        {
            return template.GetTypeName(JwtAuthenticationStateProviderTemplate.TemplateId);
        }

    }
}