using System;
using System.Collections.Generic;
using System.IO;
using Intent.Engine;
using Intent.Eventing;
using Intent.Modules.Common.CSharp.VisualStudio;
using Intent.Modules.VisualStudio.Projects.Templates;

namespace Intent.Modules.VisualStudio.Projects.Sync
{
    internal class SqlProjectSyncProcessor : ProjectSyncProcessorBase
    {
        public SqlProjectSyncProcessor(
            IVisualStudioProjectTemplate template,
            ISoftwareFactoryEventDispatcher sfEventDispatcher,
            IChanges changes) : base(
                template: template,
                sfEventDispatcher: sfEventDispatcher,
                changes: changes,
                includeXmlDeclaration: true)
        {
        }

        protected override FileAddedData GetFileAddedDataProtected(IDictionary<string, string> input)
        {
            if (!input.ContainsKey(CustomMetadataKeys.ItemType) &&
                ".sql".Equals(Path.GetExtension(input["Path"]), StringComparison.OrdinalIgnoreCase))
            {
                input[CustomMetadataKeys.ItemType] = "Build";
            }

            return base.GetFileAddedDataProtected(input);
        }

        protected override void AddProjectItem(ProjectFileXml xml, string path, FileAddedData fileAddedData)
        {
            if (".sqlproj".Equals(Path.GetExtension(path), StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            base.AddProjectItem(xml, path, fileAddedData);
        }
    }
}
