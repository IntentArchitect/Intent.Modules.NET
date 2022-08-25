﻿using System;
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

        protected override FileAddedData GetFileAddedDataP(IDictionary<string, string> input)
        {
            if (!input.ContainsKey(CustomMetadataKeys.ItemType) && 
                input.ContainsKey("ExcludeFromProject"))
            {
                input[CustomMetadataKeys.ItemType] = "None";
            }

            if (!input.ContainsKey(CustomMetadataKeys.ItemType) &&
                ".sql".Equals(Path.GetExtension(input["Path"]), StringComparison.OrdinalIgnoreCase))
            {
                input[CustomMetadataKeys.ItemType] = "Build";
            }

            return base.GetFileAddedDataP(input);
        }

        protected override void AddProjectItem(ProjectFileXml xml, string path, FileAddedData fileAddedData)
        {
            if (".sqlproj".Equals(Path.GetExtension(path), StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            if (fileAddedData.ItemType == "None")
            {
                RemoveProjectItem(xml, path);
                return;
            }

            base.AddProjectItem(xml, path, fileAddedData);
        }
    }
}
