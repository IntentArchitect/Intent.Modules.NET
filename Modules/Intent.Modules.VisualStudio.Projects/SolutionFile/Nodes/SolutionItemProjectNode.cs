using System.Collections.Generic;
using System.Linq;

namespace Intent.Modules.VisualStudio.Projects.SolutionFile
{
    internal class SolutionItemProjectNode
    {
        public SolutionItemProjectNode(string typeId, string name, string path, string id)
        {
            UnderlyingNode = new SolutionItemComplexNode(
                name: "Project",
                parameter: $"{{{typeId}}}".ToUpperInvariant(),
                values: new List<string> { name, Normalize(path), id },
                childNodes: new List<Node>());
        }

        public SolutionItemProjectNode(SolutionItemComplexNode underlyingNode)
        {
            UnderlyingNode = underlyingNode;
        }

        public SolutionItemComplexNode UnderlyingNode { get; }

        public string Name
        {
            get => UnderlyingNode.Values[0];
            set => UnderlyingNode.Values[0] = value;
        }

        public string Path
        {
            get => UnderlyingNode.Values[1];
            set => UnderlyingNode.Values[1] = Normalize(value);
        }

        public string Id
        {
            get => UnderlyingNode.Values[2];
            set => UnderlyingNode.Values[2] = value;
        }

        public bool HasPath(string path)
        {
            return Path == Normalize(path);
        }

        private static string Normalize(string path)
        {
            // .sln files always use back-slashes
            path = path.Replace("/", "\\");

            // Remove folders in path which are just "."
            path = path
                .Split("\\")
                .Where(x => !string.IsNullOrWhiteSpace(x) && x != ".")
                .Aggregate((x, y) => $"{x}\\{y}");

            return path;
        }
    }
}