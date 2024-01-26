using System.Collections.Generic;
using System.Linq;
using Intent.Modules.Enums.Shared;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly:IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpStringInterpolationTemplate",Version= "1.0")]

namespace Intent.Modules.IntegrationTesting.Templates.EnumContract
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class EnumContractTemplate
    {
        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public override string TransformText()
        {
            return EnumGenerator.Generate(this);
        }
    }
}