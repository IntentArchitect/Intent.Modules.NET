using Intent.Exceptions;
using Intent.Metadata.Models;

namespace Intent.Modules.Common.DriftEngine.Extensions;

/// <summary>
/// Reading a stereotype's properties wherever its element comes from. Intent types a stereotype's properties from its
/// definition, which only an application that installs the stereotype's module has, and <see cref="IStereotype.Properties"/>
/// throws without it: a front end's application calling the backend application's Operations needn't install the module
/// their Http Settings come from. Their stored values are still there, as text.
/// </summary>
public static class StereotypeProperties
{
    /// <summary>
    /// The stereotype's property of the name, typed as its definition says (an element lookup gives the element); its stored
    /// text when the application doesn't have the definition; null when it has no such property.
    /// </summary>
    public static IStereotypeProperty? Property(this IStereotype stereotype, string name)
    {
        try
        {
            return stereotype.Properties?.FirstOrDefault(x => x.Key == name);
        }
        catch (ElementException)
        {
            // The definition isn't in this application: TryGetProperty gives the stored value.
            return stereotype.TryGetProperty(name, out var stored) ? stored : null;
        }
    }
}
