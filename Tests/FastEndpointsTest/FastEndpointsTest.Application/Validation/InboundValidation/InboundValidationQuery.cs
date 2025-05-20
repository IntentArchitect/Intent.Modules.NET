using FastEndpoints;
using FastEndpointsTest.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryModels", Version = "1.0")]

namespace FastEndpointsTest.Application.Validation.InboundValidation
{
    public class InboundValidationQuery : IRequest<DummyResultDto>, IQuery
    {
        public InboundValidationQuery(string rangeStr,
            string minStr,
            string maxStr,
            int rangeInt,
            int minInt,
            int maxInt,
            string isRequired,
            string isRequiredEmpty,
            decimal decimalRange,
            decimal decimalMin,
            decimal decimalMax,
            string? stringOption,
            string? stringOptionNonEmpty,
            EnumDescriptions myEnum,
            string regexField)
        {
            RangeStr = rangeStr;
            MinStr = minStr;
            MaxStr = maxStr;
            RangeInt = rangeInt;
            MinInt = minInt;
            MaxInt = maxInt;
            IsRequired = isRequired;
            IsRequiredEmpty = isRequiredEmpty;
            DecimalRange = decimalRange;
            DecimalMin = decimalMin;
            DecimalMax = decimalMax;
            StringOption = stringOption;
            StringOptionNonEmpty = stringOptionNonEmpty;
            MyEnum = myEnum;
            RegexField = regexField;
        }

        [QueryParam]
        public string RangeStr { get; set; }
        [QueryParam]
        public string MinStr { get; set; }
        [QueryParam]
        public string MaxStr { get; set; }
        [QueryParam]
        public int RangeInt { get; set; }
        [QueryParam]
        public int MinInt { get; set; }
        [QueryParam]
        public int MaxInt { get; set; }
        [QueryParam]
        public string IsRequired { get; set; }
        [QueryParam]
        public string IsRequiredEmpty { get; set; }
        [QueryParam]
        public decimal DecimalRange { get; set; }
        [QueryParam]
        public decimal DecimalMin { get; set; }
        [QueryParam]
        public decimal DecimalMax { get; set; }
        [QueryParam]
        public string? StringOption { get; set; }
        [QueryParam]
        public string? StringOptionNonEmpty { get; set; }
        [QueryParam]
        public EnumDescriptions MyEnum { get; set; }
        [QueryParam]
        public string RegexField { get; set; }
    }
}