using System.ComponentModel;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.ContractEnumModel", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Application.Validation
{
    public enum EnumDescriptions
    {
        [Description("My First Option")]
        Option1,
        [Description("My Second Option")]
        Option2 = 2
    }
}