using System.ComponentModel;
using DtoSettings.Class.Init.Domain;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace DtoSettings.Class.Init.Application.DefaultValues
{
    public class DefaultValueDto
    {
        public DefaultValueDto()
        {
        }

        [System.ComponentModel.DefaultValue("NoQuoteDefault")]
        public string StringDefault { get; init; } = "NoQuoteDefault";
        [System.ComponentModel.DefaultValue(0)]
        public int IntDefault { get; init; } = 0;
        [System.ComponentModel.DefaultValue(DefaultValueEnum.Two)]
        public DefaultValueEnum EnumDefault { get; init; } = DefaultValueEnum.Two;

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