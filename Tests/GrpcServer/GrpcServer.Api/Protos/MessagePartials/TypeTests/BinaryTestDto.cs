using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Google.Protobuf;
using GrpcServer.Application.TypeTests;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.Grpc.MessagePartial", Version = "1.0")]

namespace GrpcServer.Api.Protos.Messages.TypeTests
{
    public partial class BinaryTestDto
    {
        public Application.TypeTests.BinaryTestDto ToContract()
        {
            return new Application.TypeTests.BinaryTestDto
            {
                BinaryField = BinaryField.ToByteArray(),
                BinaryFieldCollection = BinaryFieldCollection.Select(x => x.ToByteArray()).ToList(),
                BinaryFieldNullable = BinaryFieldNullable?.ToByteArray(),
                BinaryFieldNullableCollection = BinaryFieldNullableCollection?.Items.Select(x => x.ToByteArray()).ToList()
            };
        }

        [return: NotNullIfNotNull(nameof(contract))]
        public static BinaryTestDto? Create(Application.TypeTests.BinaryTestDto? contract)
        {
            if (contract == null)
            {
                return null;
            }

            var message = new BinaryTestDto
            {
                BinaryField = ByteString.CopyFrom(contract.BinaryField),
                BinaryFieldNullable = contract.BinaryFieldNullable != null ? ByteString.CopyFrom(contract.BinaryFieldNullable) : null,
                BinaryFieldNullableCollection = ListOfBytes.Create(contract.BinaryFieldNullableCollection)
            };

            message.BinaryFieldCollection.AddRange(contract.BinaryFieldCollection.Select(ByteString.CopyFrom));
            return message;
        }
    }
}