using System;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTagModeImplicit]

namespace MongoDb.TestApplication.Domain.Entities.Indexes
{
    [DefaultIntentManaged(Mode.Fully, Targets = Targets.Methods, Body = Mode.Ignore, AccessModifiers = AccessModifiers.Public)]
    public class CompoundIndexEntitySingleChild
    {
        private string? _id;

        public string CompoundOne { get; set; }

        public string CompoundTwo { get; set; }
    }
}