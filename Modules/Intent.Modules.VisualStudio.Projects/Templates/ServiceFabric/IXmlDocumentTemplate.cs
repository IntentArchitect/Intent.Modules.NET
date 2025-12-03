using System.Xml;

namespace Intent.Modules.VisualStudio.Projects.Templates.ServiceFabric
{
    public interface IXmlDocumentTemplate
    {
        public XmlDocument Document { get; }

        public XmlNamespaceManager NamespaceManager { get; }

        public string Namespace { get; }
    }
}
