using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;

namespace Intent.Modules.Application.DomainInteractions;

public static class TemplateExtensions
{
    public static string GetNotFoundExceptionName(this ICSharpTemplate template)
    {
        var exceptionName = template
            .GetTypeName("Domain.NotFoundException", TemplateDiscoveryOptions.DoNotThrow);
        return exceptionName ?? "NotFoundException";
    }
}