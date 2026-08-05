using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]

namespace Intent.Modules.AspNetCore.IntegrationTesting.Templates
{
    /// <summary>
    /// Extension methods for reading this module's designer metadata off of arbitrary elements.
    /// </summary>
    internal static class ElementExtensions
    {
        /// <summary>
        /// The "Integration Test" stereotype definition owned by this module. It is applicable to
        /// Commands, Queries, Services and Operations and marks them for integration test scaffolding
        /// when the "Integration Test Generation Mode" setting is set to <c>explicit</c>.
        /// </summary>
        private const string IntegrationTestStereotypeId = "90142c0a-268e-4316-a9f1-fcf1e40d5f47";

        /// <summary>
        /// Checks if the element (or, for an endpoint hosted on a Service, its parent Service) has the
        /// Integration Test stereotype applied.
        /// </summary>
        public static bool HasIntegrationTestStereotype(this IElement element)
        {
            return element != null &&
                (element.HasStereotype(IntegrationTestStereotypeId) ||
                element.ParentElement?.HasStereotype(IntegrationTestStereotypeId) == true);
        }
    }
}
