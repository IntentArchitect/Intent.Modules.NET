using System;
using System.Collections.Generic;
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
            return StyleContentOverride ?? "";
        }

        [IntentManaged(Mode.Ignore)]
        public override bool CanRunTemplate()
        {
            return !string.IsNullOrWhiteSpace(StyleContentOverride);
        }
    }
}
