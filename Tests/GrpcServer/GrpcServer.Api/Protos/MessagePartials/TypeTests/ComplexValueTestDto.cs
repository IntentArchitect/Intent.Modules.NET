using System.Diagnostics.CodeAnalysis;
using System.Linq;
using GrpcServer.Application.TypeTests;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.Grpc.MessagePartial", Version = "1.0")]

namespace GrpcServer.Api.Protos.Messages.TypeTests
{
    public partial class ComplexValueTestDto
    {
        public Application.TypeTests.ComplexValueTestDto ToContract()
        {
            return new Application.TypeTests.ComplexValueTestDto
            {
                ComplexTypeField = ComplexTypeField.ToContract(),
                ComplexTypeFieldCollection = ComplexTypeFieldCollection.Select(x => x.ToContract()).ToList(),
                ComplexTypeFieldNullable = ComplexTypeFieldNullable?.ToContract(),
                ComplexTypeFieldNullableCollection = ComplexTypeFieldNullableCollection?.Items.Select(x => x.ToContract()).ToList()
            };
        }

        [return: NotNullIfNotNull(nameof(contract))]
        public static ComplexValueTestDto? Create(Application.TypeTests.ComplexValueTestDto? contract)
        {
            if (contract == null)
            {
                return null;
            }

            var message = new ComplexValueTestDto
            {
                ComplexTypeField = ComplexTypeDto.Create(contract.ComplexTypeField),
                ComplexTypeFieldNullable = ComplexTypeDto.Create(contract.ComplexTypeFieldNullable),
                ComplexTypeFieldNullableCollection = ListOfComplexTypeDto.Create(contract.ComplexTypeFieldNullableCollection)
            };

            message.ComplexTypeFieldCollection.AddRange(contract.ComplexTypeFieldCollection.Select(ComplexTypeDto.Create));
            return message;
        }
    }
}