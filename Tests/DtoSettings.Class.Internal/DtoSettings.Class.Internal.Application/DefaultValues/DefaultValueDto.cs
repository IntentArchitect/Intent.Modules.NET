using System.ComponentModel;
using DtoSettings.Class.Internal.Domain;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace DtoSettings.Class.Internal.Application.DefaultValues
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
        public string StringDefault { get; internal set; } = "NoQuoteDefault";
        [System.ComponentModel.DefaultValue(0)]
        public int IntDefault { get; internal set; } = 0;
        [System.ComponentModel.DefaultValue(DefaultValueEnum.Two)]
        public DefaultValueEnum EnumDefault { get; internal set; } = DefaultValueEnum.Two;
    }
}