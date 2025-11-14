using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]

namespace Intent.Modules.UnitTesting.Templates
{
    /// <summary>
    /// Generic extension methods for IElement to replace model-specific operations.
    /// </summary>
    public static class ElementExtensions
    {
        /// <summary>
        /// Gets parameter child elements for Operations.
        /// </summary>
        public static IEnumerable<IElement> GetParameters(this IElement element)
        {
            return element.ChildElements
                .Where(c => c.SpecializationTypeId == SpecializationTypeIds.Parameter);
        }

        /// <summary>
        /// Gets property child elements for Commands/Queries.
        /// </summary>
        public static IEnumerable<IElement> GetProperties(this IElement element)
        {
            return element.ChildElements
                .Where(c => c.SpecializationTypeId == SpecializationTypeIds.Property);
        }

        /// <summary>
        /// Gets operation child elements for Services.
        /// </summary>
        public static IEnumerable<IElement> GetOperations(this IElement element)
        {
            return element.ChildElements
                .Where(c => c.SpecializationTypeId == SpecializationTypeIds.Operation);
        }

        /// <summary>
        /// Checks if element has the Unit Test stereotype applied.
        /// </summary>
        public static bool HasUnitTestStereotype(this IElement element)
        {
            return element.HasStereotype(SpecializationTypeIds.UnitTestStereotype);
        }
    }
}
