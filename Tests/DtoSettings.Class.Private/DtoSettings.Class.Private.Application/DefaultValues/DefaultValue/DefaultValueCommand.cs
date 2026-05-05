using System.ComponentModel;
using DtoSettings.Class.Private.Application.Common.Interfaces;
using DtoSettings.Class.Private.Domain;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace DtoSettings.Class.Private.Application.DefaultValues.DefaultValue
{
    public class DefaultValueCommand : IRequest, ICommand
    {
        public DefaultValueCommand(string stringDefault = "NoQuoteDefault",
            int intDefault = 0,
            DefaultValueEnum enumDefault = DefaultValueEnum.Three)
        {
            StringDefault = stringDefault;
            IntDefault = intDefault;
            EnumDefault = enumDefault;
        }

        [System.ComponentModel.DefaultValue("NoQuoteDefault")]
        public string StringDefault { get; set; }
        [System.ComponentModel.DefaultValue(0)]
        public int IntDefault { get; set; }
        [System.ComponentModel.DefaultValue(DefaultValueEnum.Three)]
        public DefaultValueEnum EnumDefault { get; set; }
    }
}