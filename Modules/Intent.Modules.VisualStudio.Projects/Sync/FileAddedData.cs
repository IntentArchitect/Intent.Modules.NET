using System.Collections.Generic;
using Intent.Modules.Common.CSharp.VisualStudio;

namespace Intent.Modules.VisualStudio.Projects.Sync
{
    internal class FileAddedData
    {
        /// <summary>
        /// Template file metadata key: when <c>"true"</c>, the project sync leaves the file's
        /// <c>.csproj</c> items exactly as they are. Set by hands-off templates (e.g.
        /// <c>Generate AppSettings File</c> turned off), whose file and its items are the developer's.
        /// </summary>
        public const string LeaveProjectItemsUnchangedKey = "VisualStudio.Projects.LeaveProjectItemsUnchanged";

        public bool LeaveProjectItemsUnchanged { get; set; }

        public Dictionary<string, string> Elements { get; set; } = new();

        public Dictionary<string, string> Attributes { get; set; } = new();

        public string ItemType { get; set; }

        public string RemoveItemType { get; set; }

        public bool WasAddedImplicitly { get; set; } = true;

        public MsBuildFileItemGenerationBehaviour? MsBuildFileItemGenerationBehaviour { get; set; }
    }
}