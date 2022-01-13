using System;
using System.Collections.Generic;

namespace Intent.Modules.VisualStudio.Projects.SolutionFile
{
    internal interface INodeParser
    {
        bool TryParse(string line, Queue<string> queue, Func<List<Node>> getChildNodes, out Node node);
    }
}