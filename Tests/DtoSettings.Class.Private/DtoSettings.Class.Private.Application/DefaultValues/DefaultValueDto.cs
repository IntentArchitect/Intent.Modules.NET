using System.ComponentModel;
using DtoSettings.Class.Private.Domain;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace DtoSettings.Class.Private.Application.DefaultValues
{
    public class DefaultValueDto
    {
        public DefaultValueDto(string stringDefault = "NoQuoteDefault", int intDefault = 0, DefaultValueEnum enumDefault = DefaultValueEnum.Two)
        {
            StringDefault = stringDefault;
            IntDefault = intDefault;
            EnumDefault = enumDefault;
        }

        protected DefaultValueDto()
        {
        }

        [System.ComponentModel.DefaultValue("NoQuoteDefault")]
        public string StringDefault { get; private set; } = "NoQuoteDefault";
        [System.ComponentModel.DefaultValue(0)]
        public int IntDefault { get; private set; } = 0;
        [System.ComponentModel.DefaultValue(DefaultValueEnum.Two)]
        public DefaultValueEnum EnumDefault { get; private set; } = DefaultValueEnum.Two;

        public static DefaultValueDto Create(
            string stringDefault = "NoQuoteDefault",
            int intDefault = 0,
            DefaultValueEnum enumDefault = DefaultValueEnum.Two)
        {
            return new DefaultValueDto(stringDefault, intDefault, enumDefault);
        }
    }
}