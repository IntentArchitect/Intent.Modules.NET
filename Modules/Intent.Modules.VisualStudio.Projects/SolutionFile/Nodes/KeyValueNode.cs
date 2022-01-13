using System;
using System.Collections.Generic;

namespace Intent.Modules.VisualStudio.Projects.SolutionFile
{
    internal class KeyValueNode : Node
    {
        public KeyValueNode(string key, string value)
        {
            Key = key;
            Value = value;
        }

        public string Key { get; set; }
        public string Value { get; set; }

        public override void Visit(Writer writer)
        {
            writer.WriteLine($"{Key} = {Value}");
        }

        public class Parser : INodeParser
        {
            public bool TryParse(string line, Queue<string> queue, Func<List<Node>> getChildNodes, out Node node)
            {
                node = default;

                if (!line.Contains(" = ") ||
                    line.IsHierarchicalStart(queue, out _))
                {
                    return false;
                }

                var split = line.Trim().Split(" = ");

                node = new KeyValueNode(split[0], split[1]);
                return true;
            }
        }
    }
}