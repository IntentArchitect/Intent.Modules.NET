using System.Collections.Generic;
using System.Text;

namespace Intent.Modules.VisualStudio.Projects.SolutionFile
{
    internal class Writer
    {
        private int _depth;
        private readonly StringBuilder _sb = new StringBuilder();

        private void PushIndentation()
        {
            _depth++;
        }

        private void PopIndentation()
        {
            _depth--;
        }

        public void WriteLine(string value)
        {
            for (var i = 0; i < _depth; i++)
            {
                _sb.Append('\t');
            }

            _sb.AppendLine(value);
        }

        public void WriteChildren(List<Node> childNodes, bool increaseIndentation = true)
        {
            if (increaseIndentation) PushIndentation();

            foreach (var node in childNodes)
            {
                node.Visit(this);
            }

            if (increaseIndentation) PopIndentation();
        }

        public override string ToString()
        {
            return _sb.ToString();
        }
    }
}