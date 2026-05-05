using System;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace DtoSettings.Class.Internal.Domain.Entities
{
    public class DefaultValueEntity
    {
        public Guid Id { get; set; }

        public string StringDefault { get; set; } = "NoQuoteDefault";

        public string IntDefault { get; set; } = "0";

        public DefaultValueEnum EnumDefault { get; set; } = DefaultValueEnum.Two;
    }
}