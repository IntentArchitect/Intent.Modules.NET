using System.ComponentModel;
using DtoSettings.Record.Public.Application.Common.Interfaces;
using DtoSettings.Record.Public.Domain;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryModels", Version = "1.0")]

namespace DtoSettings.Record.Public.Application.DefaultValues.DefaultValue
{
    public class DefaultValueQuery : IRequest<int>, IQuery
    {
        public DefaultValueQuery(string stringDefault = "NoQuoteDefault",
            int intDefault = 0,
            DefaultValueEnum enumDefault = DefaultValueEnum.One)
        {
            StringDefault = stringDefault;
            IntDefault = intDefault;
            EnumDefault = enumDefault;
        }

        [System.ComponentModel.DefaultValue("NoQuoteDefault")]
        public string StringDefault { get; set; }
        [System.ComponentModel.DefaultValue(0)]
        public int IntDefault { get; set; }
        [System.ComponentModel.DefaultValue(DefaultValueEnum.One)]
        public DefaultValueEnum EnumDefault { get; set; }
    }
}