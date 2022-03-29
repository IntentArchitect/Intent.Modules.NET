using Intent.Modules.Common.CSharp.Configuration;
using Intent.Modules.Common.CSharp.VisualStudio;
using Intent.Modules.Common.Templates;

namespace Intent.Modules.VisualStudio.Projects.Templates
{
    internal static class ForProjectWithRoleRequestExtensions
    {
        public static bool IsApplicableTo(this IForProjectWithRoleRequest request, IntentTemplateBase template)
        {
            if (!string.IsNullOrWhiteSpace(request.ForProjectWithRole) &&
                !template.OutputTarget.GetProject().HasRole(request.ForProjectWithRole))
            {
                return false;
            }

            request.MarkHandled();

            return true;
        }
    }
}
