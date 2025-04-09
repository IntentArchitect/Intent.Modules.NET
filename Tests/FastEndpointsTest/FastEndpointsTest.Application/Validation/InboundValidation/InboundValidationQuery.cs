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

        [FromQuery]
        public string RangeStr { get; set; }
        [FromQuery]
        public string MinStr { get; set; }
        [FromQuery]
        public string MaxStr { get; set; }
        [FromQuery]
        public int RangeInt { get; set; }
        [FromQuery]
        public int MinInt { get; set; }
        [FromQuery]
        public int MaxInt { get; set; }
        [FromQuery]
        public string IsRequired { get; set; }
        [FromQuery]
        public string IsRequiredEmpty { get; set; }
        [FromQuery]
        public decimal DecimalRange { get; set; }
        [FromQuery]
        public decimal DecimalMin { get; set; }
        [FromQuery]
        public decimal DecimalMax { get; set; }
        [FromQuery]
        public string? StringOption { get; set; }
        [FromQuery]
        public string? StringOptionNonEmpty { get; set; }
        [FromQuery]
        public EnumDescriptions MyEnum { get; set; }
        [FromQuery]
        public string RegexField { get; set; }
    }
}