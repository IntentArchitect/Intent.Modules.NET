using System;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTagModeImplicit]

namespace AzureFunctions.TestApplication.Domain.Entities
{
    [DefaultIntentManaged(Mode.Fully, Targets = Targets.Methods, Body = Mode.Ignore, AccessModifiers = AccessModifiers.Public)]
    public class SampleDomain
    {

        public Guid Id { get; set; }

        public string Attribute { get; set; }
    }
}