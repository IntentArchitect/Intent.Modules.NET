using System.Collections.Generic;
using System.Linq;
using Intent.Templates;

namespace Intent.Modules.VisualStudio.Projects.Templates
{
    /// <summary>
    /// A template whose file the developer owns when a "Generate ... File" setting is off. It stays
    /// registered so that other modules resolving it keep working, but nothing is ever written on
    /// its account.
    /// </summary>
    internal interface IHandsOffTemplate
    {
        bool IsHandsOff { get; }
    }

    internal static class HandsOffTemplateExtensions
    {
        /// <summary>
        /// The templates whose dependencies (NuGet, assembly, framework and project references) go into
        /// their project. A hands-off template is left out, along with everything other modules attached
        /// to it for code it will never write.
        /// </summary>
        public static IEnumerable<ITemplate> ExceptHandsOff(this IEnumerable<ITemplate> templates)
        {
            return templates.Where(template => template is not IHandsOffTemplate { IsHandsOff: true });
        }
    }
}
