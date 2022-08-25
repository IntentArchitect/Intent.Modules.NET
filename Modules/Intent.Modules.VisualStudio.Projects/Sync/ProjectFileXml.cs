using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;

namespace Intent.Modules.VisualStudio.Projects.Sync;

internal class ProjectFileXml
{
    public ProjectFileXml(string xmlContent)
    {
        Document = XDocument.Parse(xmlContent, LoadOptions.PreserveWhitespace);
        Namespace = Document.Root!.GetDefaultNamespace();
        Namespaces = new XmlNamespaceManager(new NameTable());
        Namespaces.AddNamespace("ns", Namespace.NamespaceName);
        ProjectElement = Document.XPathSelectElement("/ns:Project", Namespaces);
    }

    public XDocument Document { get; }
    public XNamespace Namespace { get; }
    public XmlNamespaceManager Namespaces { get; }
    public XElement ProjectElement { get; }
}