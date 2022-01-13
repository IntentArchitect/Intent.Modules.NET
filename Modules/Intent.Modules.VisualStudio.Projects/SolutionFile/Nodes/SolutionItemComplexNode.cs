using System;
using System.Collections.Generic;
using System.Linq;

namespace Intent.Modules.VisualStudio.Projects.SolutionFile
{
    /// <summary>
    /// Always at the root, sometimes contains <see cref="SolutionItemSectionNode"/>s, e.g.:
    /// <code>
    /// Project("{2150E333-8FDC-42A3-9474-1A3956D46DE8}") = "Release", "Release", "{0E743CD7-5B62-4275-9263-3BEFE9B26696}"
    ///    ...
    /// EndProject
    /// </code>
    /// </summary>
    internal class SolutionItemComplexNode : Node
    {
        public SolutionItemComplexNode(string name, string parameter, List<string> values, List<Node> childNodes)
        {
            Name = name;
            Parameter = parameter;
            Values = values;
            ChildNodes = childNodes;
        }

        public string Name { get; set; }

        public string Parameter { get; set; }

        public List<string> Values { get; set; }

        public string Id
        {
            get => Values[2];
            set => Values[2] = value;
        }

        /// <remarks>
        /// TODO: This only ever contains <see cref="SolutionItemSectionNode"/>.
        /// </remarks>
        public List<Node> ChildNodes { get; set; }

        public override void Visit(Writer writer)
        {
            var values = Values
                .Select(value => $"\"{value}\"")
                .Aggregate((x, y) => x + ", " + y);

            writer.WriteLine($"{Name}(\"{Parameter}\") = {values}");
            writer.WriteChildren(ChildNodes);
            writer.WriteLine($"End{Name}");
        }
        public class Parser : INodeParser
        {
            public bool TryParse(string line, Queue<string> queue, Func<List<Node>> getChildNodes, out Node node)
            {
                node = default;

                if (!line.Contains(" = ") ||
                    !line.Contains('\"') ||
                    !line.IsHierarchicalStart(queue, out var name))
                {
                    return false;
                }

                var parameterStart = line.IndexOf('"') + 1;
                var parameterEnd = line.IndexOf('"', parameterStart + 1);
                var parameter = line.Substring(parameterStart, parameterEnd - parameterStart);

                var values = line
                    .Substring(parameterEnd + "\") = ".Length)
                    .Split("\", \"")
                    .Select(x => x.Trim('\"'))
                    .ToList();

                node = new SolutionItemComplexNode(name, parameter, values, getChildNodes());
                return true;
            }
        }
    }
}