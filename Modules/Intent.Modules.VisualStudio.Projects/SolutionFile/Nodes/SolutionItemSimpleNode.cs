using System;
using System.Collections.Generic;
using System.Linq;

namespace Intent.Modules.VisualStudio.Projects.SolutionFile
{
    /// <summary>
    /// Always at the root, sometimes contains <see cref="SolutionItemSectionNode"/>s, e.g.:
    /// <code>
    /// Global
    ///    ...
    /// EndGlobal
    /// </code>
    /// </summary>
    internal class SolutionItemSimpleNode : Node
    {
        public SolutionItemSimpleNode(string name, List<Node> childNodes)
        {
            Name = name;
            ChildNodes = childNodes;
        }

        public string Name { get; set; }

        /// <remarks>
        /// TODO: This only ever contains <see cref="SolutionItemSectionNode"/>.
        /// </remarks>
        public List<Node> ChildNodes { get; set; }

        public override void Visit(Writer writer)
        {
            writer.WriteLine($"{Name}");
            writer.WriteChildren(ChildNodes);
            writer.WriteLine($"End{Name}");
        }

        public bool GetOrCreateSection(string name, string value, List<Node> childNodes, out SolutionItemSectionNode sectionNode)
        {
            var identifier = $"{Name}Section";

            sectionNode = ChildNodes
                .OfType<SolutionItemSectionNode>()
                .SingleOrDefault(x => x.Name == identifier && x.SectionName == name);

            if (sectionNode != null)
            {
                return true;
            }

            sectionNode = new SolutionItemSectionNode(
                name: identifier,
                sectionName: name,
                value: value,
                childNodes: childNodes);

            ChildNodes.Add(sectionNode);
            return false;
        }

        public class Parser : INodeParser
        {
            public bool TryParse(string line, Queue<string> queue, Func<List<Node>> getChildNodes, out Node node)
            {
                node = default;

                if (line.Contains(" = ") ||
                    !line.IsHierarchicalStart(queue, out var name))
                {
                    return false;
                }

                node = new SolutionItemSimpleNode(name, getChildNodes());
                return true;
            }
        }
    }
}