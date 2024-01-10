using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.VisualStudio.Projects.Api;
using Intent.Templates;

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

        public static NETSettings GetNETSettings(this IVisualStudioProject project)
        {
            var stereotype = project.GetStereotype(".NET Settings");
            return stereotype != null ? new NETSettings(stereotype) : null;
        }

        public static bool HasNETSettings(this IVisualStudioProject project)
        {
            return project.HasStereotype(".NET Settings");
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

        public static bool NullableIsEnabled(this WCFServiceApplicationModel model) => model.InternalElement.NullableIsEnabled();
        public static bool NullableIsEnabled(this ASPNETWebApplicationNETFrameworkModel model) => model.InternalElement.NullableIsEnabled();
        public static bool NullableIsEnabled(this ClassLibraryNETCoreModel model) => model.InternalElement.NullableIsEnabled();
        public static bool NullableIsEnabled(this ConsoleAppNETFrameworkModel model) => model.InternalElement.NullableIsEnabled();
        public static bool NullableIsEnabled(this ClassLibraryNETFrameworkModel model) => model.InternalElement.NullableIsEnabled();
        public static bool NullableIsEnabled(this AzureFunctionsProjectModel model) => model.InternalElement.NullableIsEnabled();
        public static bool NullableIsEnabled(this ConsoleAppNETCoreModel model) => model.InternalElement.NullableIsEnabled();
        public static bool NullableIsEnabled(this ASPNETCoreWebApplicationModel model) => model.InternalElement.NullableIsEnabled();
        public static bool NullableIsEnabled(this CSharpProjectNETModel model) => model.InternalElement.NullableIsEnabled();

        private static bool NullableIsEnabled(this IHasStereotypes element)
        {
            var stereotype = element.GetStereotype("C# Project Options");

            return stereotype?.GetProperty<string>("Nullable") == "enable" ||
                   stereotype?.GetProperty<string>("Nullable") == "warnings" ||
                   stereotype?.GetProperty<string>("Nullable") == "annotations" ||
                   stereotype?.GetProperty<bool>("Nullable Enabled") == true;
        }
    }

    /// <summary>
    /// An alias of one of the generated stereotype extensions, Intent generates one
    /// for each target element type and they are always the same. No one was chosen
    /// in particular to extend for this alias.
    /// </summary>
    internal class NETSettings : CSharpProjectNETModelStereotypeExtensions.NETSettings
    {
        public NETSettings(IStereotype stereotype) : base(stereotype)
        {
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