using System;
using System.Collections.Generic;

namespace Intent.Modules.VisualStudio.Projects.SolutionFile
{
    /// <summary>
    /// Contains version, like:
    /// <code>
    /// Microsoft Visual Studio Solution File, Format Version 12.00
    /// </code>
    /// </summary>
    internal class HeaderNode : Node
    {
        public HeaderNode(string value)
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
                node = default;

                if (!line.StartsWith("Microsoft Visual Studio Solution File, "))
                {
                    return false;
                }

                node = new HeaderNode(line);
                return true;
            }
        }
    }
}