using System.ComponentModel;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.ContractEnumModel", Version = "1.0")]

namespace CleanArchitecture.TestApplication.Application.Validation
{
    public enum EnumDescriptions
    {
        Option1,
        Option2 = 2
    }
}