using System;
using System.Collections.Generic;

namespace Intent.Modules.VisualStudio.Projects.SolutionFile
{
    internal class CommentNode : Node
    {
        public CommentNode(string value)
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
                if (!line.StartsWith("#"))
                {
                    node = default;
                    return false;
                }

                node = new CommentNode(line);
                return true;
            }
        }
    }
}