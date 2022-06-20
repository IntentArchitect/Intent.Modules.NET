using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.VisualStudio.Projects.Api;

namespace Intent.Modules.VisualStudio.Projects
{
    internal static class VisualStudioProjectStereotypeExtensions
    {
        public static NETCoreSettings GetNETCoreSettings(this IVisualStudioProject project)
        {
            var stereotype = project.GetStereotype(".NET Core Settings");
            return stereotype != null ? new NETCoreSettings(stereotype) : null;
        }

        public static bool HasNETCoreSettings(this IVisualStudioProject project)
        {
            return project.HasStereotype(".NET Core Settings");
        }

        public static CSharpProjectOptions GetCSharpProjectOptions(this IVisualStudioProject project)
        {
            var stereotype = project.GetStereotype("C# Project Options");
            return stereotype != null ? new CSharpProjectOptions(stereotype) : null;
        }

        public static bool HasCSharpProjectOptions(this IVisualStudioProject project)
        {
            return project.HasStereotype("C# Project Options");
        }
    }

    /// <summary>
    /// An alias of one of the generated stereotype extensions, Intent generates one
    /// for each target element type and they are always the same. No one was chosen
    /// in particular to extend for this alias.
    /// </summary>
    internal class NETCoreSettings : ASPNETCoreWebApplicationModelStereotypeExtensions.NETCoreSettings
    {
        public NETCoreSettings(IStereotype stereotype) : base(stereotype)
        {
        }
    }

    /// <summary>
    /// An alias of one of the generated stereotype extensions, Intent generates one
    /// for each target element type and they are always the same. No one was chosen
    /// in particular to extend for this alias.
    /// </summary>
    internal class CSharpProjectOptions : ASPNETCoreWebApplicationModelStereotypeExtensions.CSharpProjectOptions
    {
        public CSharpProjectOptions(IStereotype stereotype) : base(stereotype)
        {
        }
    }
}