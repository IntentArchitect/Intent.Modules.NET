using System.Diagnostics.CodeAnalysis;
using System.Linq;
using GrpcServer.Application.TypeTests;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.Grpc.MessagePartial", Version = "1.0")]

namespace GrpcServer.Api.Protos.Messages.TypeTests
{
    public partial class DecimalTestDto
    {
        public Application.TypeTests.DecimalTestDto ToContract()
        {
            return new Application.TypeTests.DecimalTestDto
            {
                DecimalField = DecimalField,
                DecimalFieldCollection = DecimalFieldCollection.Select(x => (decimal)x).ToList(),
                DecimalFieldNullable = DecimalFieldNullable,
                DecimalFieldNullableCollection = DecimalFieldNullableCollection?.Items.Select(x => (decimal)x).ToList()
            };
        }

        [return: NotNullIfNotNull(nameof(contract))]
        public static DecimalTestDto? Create(Application.TypeTests.DecimalTestDto? contract)
        {
            if (contract == null)
            {
                return null;
            }

            var message = new DecimalTestDto
            {
                DecimalField = contract.DecimalField,
                DecimalFieldNullable = contract.DecimalFieldNullable,
                DecimalFieldNullableCollection = ListOfDecimalValue.Create(contract.DecimalFieldNullableCollection)
            };

            message.DecimalFieldCollection.AddRange(contract.DecimalFieldCollection?.Select(x => (DecimalValue)x));
            return message;
        }
    }
}