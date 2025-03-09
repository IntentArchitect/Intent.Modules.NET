using System.Diagnostics.CodeAnalysis;
using System.Linq;
using GrpcServer.Application.TypeTests;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.Grpc.MessagePartial", Version = "1.0")]

namespace GrpcServer.Api.Protos.Messages.TypeTests
{
    public partial class LongTestDto
    {
        public Application.TypeTests.LongTestDto ToContract()
        {
            return new Application.TypeTests.LongTestDto
            {
                LongField = LongField,
                LongFieldCollection = LongFieldCollection.ToList(),
                LongFieldNullable = LongFieldNullable,
                LongFieldNullableCollection = LongFieldNullableCollection?.Items.ToList()
            };
        }

        [return: NotNullIfNotNull(nameof(contract))]
        public static LongTestDto? Create(Application.TypeTests.LongTestDto? contract)
        {
            if (contract == null)
            {
                return null;
            }

            var message = new LongTestDto
            {
                LongField = contract.LongField,
                LongFieldNullable = contract.LongFieldNullable,
                LongFieldNullableCollection = ListOfInt64.Create(contract.LongFieldNullableCollection)
            };

            message.LongFieldCollection.AddRange(contract.LongFieldCollection);
            return message;
        }
    }
}