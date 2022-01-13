using System;
using System.Collections.Generic;

namespace Intent.Modules.VisualStudio.Projects.SolutionFile
{
    /// <summary>
    /// For blank lines, or lines with whitespace only.
    /// </summary>
    internal class WhitespaceNode : Node
    {
        public WhitespaceNode(string value)
        {
            Value = value;
        }

        public string Value { get; set; }

        public override void Visit(Writer writer)
        {
            writer.WriteLine(Value);
        }

        public class Parser : INodeParser
        {
            public bool TryParse(string line, Queue<string> queue, Func<List<Node>> getChildNodes, out Node node)
            {
                if (!string.IsNullOrWhiteSpace(line))
                {
                    node = default;
                    return false;
                }

                node = new WhitespaceNode(line);
                return true;
            }
        }
    }
}