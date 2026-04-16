namespace Intent.Modules.FluentValidation.Shared;

internal static class RuleSpaces
{
    public const string Required = "required";
    public const string LengthMax = "length.max";
    public const string LengthMin = "length.min";
    public const string Length = "length";
    public const string NumericMin = "numeric.min";
    public const string NumericMax = "numeric.max";
    public const string Numeric = "numeric";
    public const string CollectionMin = "collection.min";
    public const string CollectionMax = "collection.max";
    public const string Collection = "collection";
    public const string Regex = "pattern.regex";
    public const string Email = "format.email";
    public const string Base64 = "format.base64";
    public const string Url = "format.url";
}

internal record RuleData(string RuleSpace, params string[] Statements);