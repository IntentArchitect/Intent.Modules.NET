using Intent.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intent.Modules.AI.Blazor.Tasks.Config
{
    public class PromptFile : ICodebaseFile
    {
        public PromptFile(string path, string content)
        {
            FilePath = path;
            Content = content;
        }
        public PromptFile(string content)
        {
            FilePath = null;
            Content = content;
        }

        public string FilePath { get; }

        public string Content { get; }
    }
}
