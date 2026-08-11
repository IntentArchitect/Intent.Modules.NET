using System;
using System.Collections.Generic;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FileTemplateStringInterpolation", Version = "1.0")]

namespace Intent.Modules.Blazor.Templates.Templates.Client.RazorComponentStyle
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class RazorComponentStyleTemplate
    {
        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public override string TransformText()
        {
            if (!string.IsNullOrWhiteSpace(StyleContentOverride))
            {
                return StyleContentOverride;
            }

            var filePath = GetMetadata().GetFilePath();
            return System.IO.File.Exists(filePath) ? System.IO.File.ReadAllText(filePath) : "";
        }

        [IntentManaged(Mode.Ignore)]
        public override bool CanRunTemplate()
        {
            return !string.IsNullOrWhiteSpace(StyleContentOverride) || System.IO.File.Exists(GetMetadata().GetFilePath());
        }
    }
}
