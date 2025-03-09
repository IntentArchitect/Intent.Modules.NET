﻿using System.Collections.Generic;
using Intent.Modules.Common.CSharp.VisualStudio;

namespace Intent.Modules.VisualStudio.Projects.Sync
{
    internal class FileAddedData
    {
        public Dictionary<string, string> Elements { get; set; } = new();

        public Dictionary<string, string> Attributes { get; set; } = new();

        public string ItemType { get; set; }

        public string RemoveItemType { get; set; }

        public bool WasAddedImplicitly { get; set; } = true;

        public MsBuildFileItemGenerationBehaviour? MsBuildFileItemGenerationBehaviour { get; set; }
    }
}