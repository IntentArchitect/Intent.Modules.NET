using Intent.Modules.Common.DriftEngine.Code;

namespace Intent.Modules.Common.DriftEngine.Stacks.CSharp;

/// <summary>
/// What a well-known library's method gives, which the code can't say since its source isn't in scope: a deserializer's or
/// a mapper's type argument (<c>JsonConvert.DeserializeObject&lt;Order&gt;(json)</c>, <c>mapper.Map&lt;OrderDto&gt;(order)</c>),
/// <c>ToString()</c>'s string, and the same type from a <c>DateTime</c>'s or a string's methods that give another of it
/// (<c>date.ToLocalTime()</c>, <c>name.Trim()</c>). Any other library method is unknown, never guessed.
/// </summary>
internal static class LibraryResults
{
    /// <summary>The deserializers, by the type they're called on, whose one type argument is what they give.</summary>
    private static readonly HashSet<string> Deserializers = ["JsonConvert", "JsonSerializer"];

    private static readonly HashSet<string> DeserializeMethods = ["DeserializeObject", "Deserialize"];

    /// <summary>A <c>DateTime</c>'s or <c>DateTimeOffset</c>'s methods that give another of it.</summary>
    private static readonly HashSet<string> DateMethods =
    [
        "ToLocalTime", "ToUniversalTime", "AddDays", "AddHours", "AddMinutes", "AddSeconds", "AddMilliseconds", "AddMonths", "AddYears", "AddTicks", "Add", "Subtract"
    ];

    /// <summary>A string's methods that give another string.</summary>
    private static readonly HashSet<string> StringMethods =
    [
        "Trim", "TrimStart", "TrimEnd", "ToUpper", "ToLower", "ToUpperInvariant", "ToLowerInvariant", "Substring", "Replace", "PadLeft", "PadRight", "Insert",
        "Remove", "Normalize"
    ];

    public static TypeRefNode? Of(string name, string? receiver, TypeRefNode? receiverType, IReadOnlyList<TypeRefNode> typeArguments, int arguments)
    {
        var on = receiverType?.Name ?? receiver?.Split('.')[^1];
        if (DeserializeMethods.Contains(name) && on is not null && Deserializers.Contains(on) && typeArguments is [var deserialized])
        {
            return deserialized;
        }

        // AutoMapper's Map<TDestination>(source) and Map<TSource, TDestination>(source[, destination]), on IMapper or a mapper.
        if (name == "Map" && typeArguments.Count is 1 or 2 && on is not null && (on is "IMapper" || on.EndsWith("Mapper", StringComparison.OrdinalIgnoreCase)))
        {
            return typeArguments[^1];
        }

        if (name == "ToString" && arguments <= 2)
        {
            return Keyword("string");
        }

        return receiverType switch
        {
            { Name: "DateTime" or "DateTimeOffset", IsArray: false } when DateMethods.Contains(name) => receiverType,
            { Name: "string" or "String", IsArray: false } when StringMethods.Contains(name) => Keyword("string"),
            _ => null
        };
    }

    private static TypeRefNode Keyword(string keyword) =>
        new() { Text = keyword, Name = keyword, QualifiedName = keyword, Language = CSharpLanguage.LanguageName };
}
