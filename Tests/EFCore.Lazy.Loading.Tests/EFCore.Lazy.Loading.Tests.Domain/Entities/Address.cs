using System;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTagModeImplicit]

namespace EFCore.Lazy.Loading.Tests.Domain.Entities
{
    public class Address
    {
        public Guid Id { get; set; }
    }
}