using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Modelers.Domain.ValueObjects.Api;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly:IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpStringInterpolationTemplate",Version= "1.0")]

namespace Intent.Modules.ValueObjects.Templates.ValueObject
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class ValueObjectTemplate
    {
        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public override string TransformText()
        {
            return $@"
[assembly: DefaultIntentManaged(Mode.Fully)]

namespace {Namespace}
{{
{GetClass()}
}}";
        }
    }
}