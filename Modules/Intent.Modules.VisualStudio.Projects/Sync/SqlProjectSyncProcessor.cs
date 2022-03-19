using System;
using System.Collections.Generic;
using System.IO;
using Intent.Engine;
using Intent.Eventing;
using Intent.Utils;

namespace Intent.Modules.VisualStudio.Projects.Sync
{
    internal class SqlProjectSyncProcessor : ProjectSyncProcessorBase
    {
        private static readonly Dictionary<string, string> Children = new();
        private static readonly IReadOnlyCollection<(string FileExtension, string ItemType)> Fallbacks = new[]
        {
            (".sql", "Build")
        };

        public SqlProjectSyncProcessor(
            string projectPath,
            ISoftwareFactoryEventDispatcher sfEventDispatcher,
            IXmlFileCache xmlFileCache,
            IChanges changeManager) : base(
                projectPath,
                sfEventDispatcher,
                xmlFileCache,
                changeManager)
        {
        }

        protected override void AddProjectItem(string path, Dictionary<string, string> additionalData)
        {
            if (Path.GetExtension(path).Equals(".sqlproj", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            AddProjectItem(
                path: path,
                itemType: DetermineItemType(path, additionalData, Fallbacks),
                children: Children);
        }
    }
}
