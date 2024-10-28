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

        [FromQueryParams]
        public string RangeStr { get; set; }
        [FromQueryParams]
        public string MinStr { get; set; }
        [FromQueryParams]
        public string MaxStr { get; set; }
        [FromQueryParams]
        public int RangeInt { get; set; }
        [FromQueryParams]
        public int MinInt { get; set; }
        [FromQueryParams]
        public int MaxInt { get; set; }
        [FromQueryParams]
        public string IsRequired { get; set; }
        [FromQueryParams]
        public string IsRequiredEmpty { get; set; }
        [FromQueryParams]
        public decimal DecimalRange { get; set; }
        [FromQueryParams]
        public decimal DecimalMin { get; set; }
        [FromQueryParams]
        public decimal DecimalMax { get; set; }
        [FromQueryParams]
        public string? StringOption { get; set; }
        [FromQueryParams]
        public string? StringOptionNonEmpty { get; set; }
        [FromQueryParams]
        public EnumDescriptions MyEnum { get; set; }
        [FromQueryParams]
        public string RegexField { get; set; }
    }
}