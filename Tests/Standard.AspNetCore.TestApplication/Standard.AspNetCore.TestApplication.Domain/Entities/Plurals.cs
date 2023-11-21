using System;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTagModeImplicit]

namespace Standard.AspNetCore.TestApplication.Domain.Entities
{
    public class Plurals
    {
        public Guid Id { get; set; }
    }
}