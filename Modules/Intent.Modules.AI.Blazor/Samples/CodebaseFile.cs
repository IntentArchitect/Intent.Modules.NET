using Intent.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intent.Modules.AI.Blazor.Samples
{
    public class CodebaseFile : ICodebaseFile
    {
        public CodebaseFile(string path, string content)
        {
            FilePath = path;
            Content = content;
        }
        public string FilePath { get; } 

        public string Content { get; }
    }
}
