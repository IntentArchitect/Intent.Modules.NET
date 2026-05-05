using System.ComponentModel;
using DtoSettings.Record.Public.Domain;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace DtoSettings.Record.Public.Application.DefaultValues
{
    public record DefaultValueDto
    {
        public DefaultValueDto()
        {
        }

        [System.ComponentModel.DefaultValue("NoQuoteDefault")]
        public string StringDefault { get; set; } = "NoQuoteDefault";
        [System.ComponentModel.DefaultValue(0)]
        public int IntDefault { get; set; } = 0;
        [System.ComponentModel.DefaultValue(DefaultValueEnum.Two)]
        public DefaultValueEnum EnumDefault { get; set; } = DefaultValueEnum.Two;

        public static DefaultValueDto Create(
            string stringDefault = "NoQuoteDefault",
            int intDefault = 0,
            DefaultValueEnum enumDefault = DefaultValueEnum.Two)
        {
            return new DefaultValueDto
            {
                StringDefault = stringDefault,
                IntDefault = intDefault,
                EnumDefault = enumDefault
            };
        }
    }
}