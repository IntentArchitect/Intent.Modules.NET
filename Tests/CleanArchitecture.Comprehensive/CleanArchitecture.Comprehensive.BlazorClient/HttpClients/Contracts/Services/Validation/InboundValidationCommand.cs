using System.ComponentModel.DataAnnotations;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.Blazor.HttpClients.DtoContract", Version = "2.0")]

namespace CleanArchitecture.Comprehensive.BlazorClient.HttpClients.Contracts.Services.Validation
{
    public class InboundValidationCommand
    {
        public InboundValidationCommand()
        {
            RangeStr = null!;
            MinStr = null!;
            MaxStr = null!;
            IsRequired = null!;
            IsRequiredEmpty = null!;
            RegexField = null!;
        }

        [Required(ErrorMessage = "Range str is required.")]
        [StringLength(5, MinimumLength = 3, ErrorMessage = "Range str must be between 3 and 5 characters.")]
        public string RangeStr { get; set; }
        [Required(ErrorMessage = "Min str is required.")]
        [MinLength(3, ErrorMessage = "Min str must be 3 or more characters.")]
        public string MinStr { get; set; }
        [Required(ErrorMessage = "Max str is required.")]
        [MaxLength(10, ErrorMessage = "Max str must be 10 or less characters.")]
        public string MaxStr { get; set; }
        [Range(0, 10, ErrorMessage = "Value for Range int must be between 0 and 10.")]
        public int RangeInt { get; set; }
        [Range(0, int.MaxValue, ErrorMessage = "Value for Min int must be more than 0.")]
        public int MinInt { get; set; }
        [Range(int.MinValue, 5, ErrorMessage = "Value for Max int must be less than 5.")]
        public int MaxInt { get; set; }
        [Required(ErrorMessage = "Is required is required.")]
        public string IsRequired { get; set; }
        [Required(ErrorMessage = "Is required empty is required.")]
        public string IsRequiredEmpty { get; set; }
        [Range(10, 20, ErrorMessage = "Value for Decimal range must be between 10 and 20.")]
        public decimal DecimalRange { get; set; }
        [Range(0, double.MaxValue, ErrorMessage = "Value for Decimal min must be more than 0.")]
        public decimal DecimalMin { get; set; }
        [Range(double.MinValue, 1000, ErrorMessage = "Value for Decimal max must be less than 1000.")]
        public decimal DecimalMax { get; set; }
        public string? StringOption { get; set; }
        [Required(ErrorMessage = "String option non empty is required.")]
        public string? StringOptionNonEmpty { get; set; }
        [Required(ErrorMessage = "My enum is required.")]
        public EnumDescriptions MyEnum { get; set; }
        [Required(ErrorMessage = "Regex field is required.")]
        public string RegexField { get; set; }

        public static InboundValidationCommand Create(
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
            string? stringOption,
            string? stringOptionNonEmpty,
            EnumDescriptions myEnum,
            string regexField)
        {
            return new InboundValidationCommand
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