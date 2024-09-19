using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.Application.Contracts.Clients.DtoContract", Version = "2.0")]

namespace AzureFunctions.TestApplication.Application.IntegrationServices.Contracts.Services.Validation
{
    public class InboundComValCommand
    {
        public InboundComValCommand()
        {
            RangeStr = null!;
            MinStr = null!;
            MaxStr = null!;
            IsRequired = null!;
            IsRequiredEmpty = null!;
            RegexField = null!;
        }

        public string RangeStr { get; set; }
        public string MinStr { get; set; }
        public string MaxStr { get; set; }
        public int RangeInt { get; set; }
        public int MinInt { get; set; }
        public int MaxInt { get; set; }
        public string IsRequired { get; set; }
        public string IsRequiredEmpty { get; set; }
        public decimal DecimalRange { get; set; }
        public decimal DecimalMin { get; set; }
        public decimal DecimalMax { get; set; }
        public string StringOption { get; set; }
        public string StringOptionNonEmpty { get; set; }
        public EnumDescriptions MyEnum { get; set; }
        public string RegexField { get; set; }

        public static InboundComValCommand Create(
            string rangeStr,
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
            string stringOption,
            string stringOptionNonEmpty,
            EnumDescriptions myEnum,
            string regexField)
        {
            return new InboundComValCommand
            {
                RangeStr = rangeStr,
                MinStr = minStr,
                MaxStr = maxStr,
                RangeInt = rangeInt,
                MinInt = minInt,
                MaxInt = maxInt,
                IsRequired = isRequired,
                IsRequiredEmpty = isRequiredEmpty,
                DecimalRange = decimalRange,
                DecimalMin = decimalMin,
                DecimalMax = decimalMax,
                StringOption = stringOption,
                StringOptionNonEmpty = stringOptionNonEmpty,
                MyEnum = myEnum,
                RegexField = regexField
            };
        }
    }
}