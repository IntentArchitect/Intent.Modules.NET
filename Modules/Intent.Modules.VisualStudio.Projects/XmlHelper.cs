using System.Xml.Linq;

namespace Intent.Modules.VisualStudio.Projects
{
    internal class XmlHelper
    {
        public static bool IsSemanticallyTheSame(string original, string updated)
        {
            return XDocument.Parse(original).ToString() == XDocument.Parse(updated).ToString();
        }

        public static bool IsSemanticallyTheSame(string original, XDocument updated)
        {
            return XDocument.Parse(original).ToString() == updated.ToString();
        }
    }
}
