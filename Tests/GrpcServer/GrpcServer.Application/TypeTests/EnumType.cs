using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.ContractEnumModel", Version = "1.0")]

namespace GrpcServer.Application.TypeTests
{
    public enum EnumType
    {

    }
}