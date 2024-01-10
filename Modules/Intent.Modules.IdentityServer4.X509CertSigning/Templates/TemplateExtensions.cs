using System.Collections.Generic;
using Intent.Modules.Common.Templates;
using Intent.Modules.IdentityServer4.X509CertSigning.Templates.CertificateRepo;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateExtensions", Version = "1.0")]

namespace Intent.Modules.IdentityServer4.X509CertSigning.Templates
{
    public static class TemplateExtensions
    {
        public static string GetCertificateRepoName(this IIntentTemplate template)
        {
            return template.GetTypeName(CertificateRepoTemplate.TemplateId);
        }

    }
}