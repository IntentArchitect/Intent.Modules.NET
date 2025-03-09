using System.Diagnostics.CodeAnalysis;
using System.Linq;
using GrpcServer.Application.TypeTests;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.Grpc.MessagePartial", Version = "1.0")]

namespace GrpcServer.Api.Protos.Messages.TypeTests
{
    public partial class BoolTestDto
    {
        public Application.TypeTests.BoolTestDto ToContract()
        {
            return new Application.TypeTests.BoolTestDto
            {
                BoolField = BoolField,
                BoolFieldCollection = BoolFieldCollection.ToList(),
                BoolFieldNullable = BoolFieldNullable,
                BoolFieldNullableCollection = BoolFieldNullableCollection?.Items.ToList()
            };
        }

        [return: NotNullIfNotNull(nameof(contract))]
        public static BoolTestDto? Create(Application.TypeTests.BoolTestDto? contract)
        {
            if (contract == null)
            {
                return null;
            }

            var message = new BoolTestDto
            {
                BoolField = contract.BoolField,
                BoolFieldNullable = contract.BoolFieldNullable,
                BoolFieldNullableCollection = ListOfBool.Create(contract.BoolFieldNullableCollection)
            };

            message.BoolFieldCollection.AddRange(contract.BoolFieldCollection);
            return message;
        }
    }
}