using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.ProjectItemTemplate.Partial", Version = "1.0")]

namespace Intent.Modules.IdentityServer4.UI.Templates.QuickStartContent
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    partial class QuickStartContentTemplate : IntentTemplateBase<object>
    {
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "Intent.IdentityServer4.UI.QuickStartContent";

        private ZipEntry _zipEntry;

        [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
        public QuickStartContentTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            _zipEntry = (ZipEntry)model;
        }

        [IntentManaged(Mode.Ignore)]
        public override ITemplateFileConfig GetTemplateFileConfig()
        {
            string filename;
            var extension = Path.GetExtension(_zipEntry.FullFileNamePath);
            if (!string.IsNullOrEmpty(extension))
            {
                filename = _zipEntry.FullFileNamePath.Replace(extension, string.Empty);
            }
            else
            {
                filename = _zipEntry.FullFileNamePath;
            }
            return new TemplateFileConfig(
                fileName: filename,
                fileExtension: extension.Replace(".", string.Empty)
            );
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public override string TransformText()
        {
            return _zipEntry.Content;
        }
    }
}