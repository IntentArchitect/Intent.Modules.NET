namespace Intent.Modules.EntityFrameworkCore.Templates.EntityTypeConfiguration;

public static class EscapeHelper
{
    public static string EscapeName(string original)
    {
        var result = original;
        return result.Replace("\"", "\\\"");
    }
}