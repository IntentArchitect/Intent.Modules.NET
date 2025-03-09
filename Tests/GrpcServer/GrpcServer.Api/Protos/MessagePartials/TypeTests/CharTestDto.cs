using System.Diagnostics.CodeAnalysis;
using System.Linq;
using GrpcServer.Application.TypeTests;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.Grpc.MessagePartial", Version = "1.0")]

namespace GrpcServer.Api.Protos.Messages.TypeTests
{
    public partial class CharTestDto
    {
        public Application.TypeTests.CharTestDto ToContract()
        {
            return new Application.TypeTests.CharTestDto
            {
                CharField = CharField[0],
                CharFieldCollection = CharFieldCollection.ToList(),
                CharFieldNullable = CharFieldNullable?[0],
                CharFieldNullableCollection = CharFieldNullableCollection?.ToList()
            };
        }

        [return: NotNullIfNotNull(nameof(contract))]
        public static CharTestDto? Create(Application.TypeTests.CharTestDto? contract)
        {
            if (contract == null)
            {
                return null;
            }

            var message = new CharTestDto
            {
                CharField = char.ToString(contract.CharField),
                CharFieldCollection = new string(contract.CharFieldCollection.ToArray()),
                CharFieldNullable = contract.CharFieldNullable != null ? char.ToString(contract.CharFieldNullable.Value) : null,
                CharFieldNullableCollection = contract.CharFieldNullableCollection != null ? new string(contract.CharFieldNullableCollection.ToArray()) : null
            };

            return message;
        }
    }
}