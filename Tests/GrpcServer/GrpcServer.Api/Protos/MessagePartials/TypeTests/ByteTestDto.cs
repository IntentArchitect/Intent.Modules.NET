using System.Diagnostics.CodeAnalysis;
using System.Linq;
using GrpcServer.Application.TypeTests;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.Grpc.MessagePartial", Version = "1.0")]

namespace GrpcServer.Api.Protos.Messages.TypeTests
{
    public partial class ByteTestDto
    {
        public Application.TypeTests.ByteTestDto ToContract()
        {
            return new Application.TypeTests.ByteTestDto
            {
                ByteField = (byte)ByteField,
                ByteFieldCollection = ByteFieldCollection.Select(x => (byte)x).ToList(),
                ByteFieldNullable = (byte?)ByteFieldNullable,
                ByteFieldNullableCollection = ByteFieldNullableCollection?.Items.Select(x => (byte)x).ToList()
            };
        }

        [return: NotNullIfNotNull(nameof(contract))]
        public static ByteTestDto? Create(Application.TypeTests.ByteTestDto? contract)
        {
            if (contract == null)
            {
                return null;
            }

            var message = new ByteTestDto
            {
                ByteField = contract.ByteField,
                ByteFieldNullable = contract.ByteFieldNullable,
                ByteFieldNullableCollection = ListOfUInt32.Create(contract.ByteFieldNullableCollection?.Select(x => (uint)x))
            };

            message.ByteFieldCollection.AddRange(contract.ByteFieldCollection.Select(x => (uint)x));
            return message;
        }
    }
}