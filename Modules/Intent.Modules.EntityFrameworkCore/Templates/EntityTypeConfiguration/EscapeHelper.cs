namespace Intent.Modules.EntityFrameworkCore.Templates.EntityTypeConfiguration;

public static class EscapeHelper
{
    public static string EscapeName(string original)
    {
        var result = original;
        return result.Replace("\"", "\\\"");
    }
    
    public static string EscapeValueForCSharpStringLiteral(string value, bool treatAsSqlExpression, bool isStringType)
    {
        value = value.Trim();
        
        // Check if the value already starts with a double-quote
        var alreadyQuoted = value.StartsWith("\"") && value.EndsWith("\"");

        if (treatAsSqlExpression)
        {
            // For SQL expressions: always wrap with quotes if not there, and escape any embedded quotes
            if (alreadyQuoted)
            {
                value = value.Remove(value.Length - 1);
                value = value.Substring(1);
            }
            return $@"""{EscapeName(value)}""";
        }

        // For literal values: only wrap if it's a string type and not already quoted
        if (isStringType && !alreadyQuoted)
        {
            return $@"""{value}""";
        }

        return value;
    }
}