using System;
using System.IO;
using System.Text;
using System.Xml;

namespace Intent.Modules.VisualStudio.Projects.Templates
{
    public static class XmlDocumentExtensions
    {
        public static string ToUtf8String(this XmlDocument doc)
        {
            if (doc == null)
            {
                throw new ArgumentNullException(nameof(doc));
            }

            var sb = new StringBuilder();
            using (var textWriter = new Utf8StringWriter(sb))
            {
                doc.Save(textWriter);
            }

            return sb.ToString();
        }

        private class Utf8StringWriter(StringBuilder sb) : StringWriter(sb)
        {
            public override Encoding Encoding => Encoding.UTF8;
        }
    }
}
