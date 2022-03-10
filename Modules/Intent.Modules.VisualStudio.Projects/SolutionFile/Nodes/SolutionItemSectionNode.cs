using System;
using System.Collections.Generic;

namespace Intent.Modules.VisualStudio.Projects.SolutionFile
{
    /// <summary>
    /// Only ever within a <see cref="SolutionItemSimpleNode"/> or <see cref="SolutionItemComplexNode"/>, e.g.:
    /// <code>
    /// ProjectSection(SolutionItems) = preProject
    ///     ...
    /// EndProjectSection
    /// </code>
    /// </summary>
    internal class SolutionItemSectionNode : Node
    {
        public SolutionItemSectionNode(string name, string sectionName, string value, List<Node> childNodes)
        {
            Name = name;
            SectionName = sectionName;
            Value = value;
            ChildNodes = childNodes;
        }

        public string Name { get; set; }

        public string SectionName { get; set; }

        public string Value { get; set; }

        /// <remarks>
        /// TODO: This only ever contains <see cref="KeyValueNode"/>.
        /// </remarks>
        public List<Node> ChildNodes { get; set; }

        public override void Visit(Writer writer)
        {
            if (ChildNodes.Count == 0)
            {
                return;
            }

            writer.WriteLine($"{Name}({SectionName}) = {Value}");
            writer.WriteChildren(ChildNodes);
            writer.WriteLine($"End{Name}");
        }

        /// <remarks>
        /// TODO: I realized after making this work that these are only ever within a <see cref="SolutionItemSimpleNode"/>.
        /// </remarks>
        internal class Parser : INodeParser
        {
            public bool TryParse(string line, Queue<string> queue, Func<List<Node>> getChildNodes, out Node node)
            {
                node = default;

                if (!line.Contains(" = ") ||
                    line.Contains('\"') ||
                    !line.IsHierarchicalStart(queue, out var name))
                {
                    return false;
                }

                var parameterStart = line.IndexOf('(') + 1;
                var parameterEnd = line.IndexOf(')');
                var parameter = line.Substring(parameterStart, parameterEnd - parameterStart);

                var value = line.Substring(parameterEnd + ") = ".Length);

                node = new SolutionItemSectionNode(name, parameter, value, getChildNodes());
                return true;
            }
        }
    }
}