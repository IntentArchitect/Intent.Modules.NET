using System.IO;
using System.Text;
using Microsoft.Build.Construction;

namespace Intent.Modules.VisualStudio.Projects.Templates;

internal static class ProjectRootElementExtensions
{
    public static string ToUtf8String(this ProjectRootElement projectRootElement)
    {
        using var stringWriter = new Utf8EncodingStringWriter();
        projectRootElement.Save(stringWriter);
        return stringWriter.ToString();
    }

    private class Utf8EncodingStringWriter : StringWriter
    {
        public override Encoding Encoding => Encoding.UTF8;
    }
}