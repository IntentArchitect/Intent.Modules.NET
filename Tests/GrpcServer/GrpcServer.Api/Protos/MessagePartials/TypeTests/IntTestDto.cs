using System.Diagnostics.CodeAnalysis;
using System.Linq;
using GrpcServer.Application.TypeTests;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.Grpc.MessagePartial", Version = "1.0")]

namespace GrpcServer.Api.Protos.Messages.TypeTests
{
    public partial class IntTestDto
    {
        public Application.TypeTests.IntTestDto ToContract()
        {
            return new Application.TypeTests.IntTestDto
            {
                IntField = IntField,
                IntFieldCollection = IntFieldCollection.ToList(),
                IntFieldNullable = IntFieldNullable,
                IntFieldNullableCollection = IntFieldNullableCollection?.Items.ToList()
            };
        }

        [return: NotNullIfNotNull(nameof(contract))]
        public static IntTestDto? Create(Application.TypeTests.IntTestDto? contract)
        {
            if (contract == null)
            {
                return null;
            }

            var message = new IntTestDto
            {
                IntField = contract.IntField,
                IntFieldNullable = contract.IntFieldNullable,
                IntFieldNullableCollection = ListOfInt32.Create(contract.IntFieldNullableCollection)
            };

            message.IntFieldCollection.AddRange(contract.IntFieldCollection);
            return message;
        }
    }
}