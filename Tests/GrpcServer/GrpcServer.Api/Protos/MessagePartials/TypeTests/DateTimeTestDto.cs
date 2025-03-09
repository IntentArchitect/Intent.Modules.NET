using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Google.Protobuf.WellKnownTypes;
using GrpcServer.Application.TypeTests;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.Grpc.MessagePartial", Version = "1.0")]

namespace GrpcServer.Api.Protos.Messages.TypeTests
{
    public partial class DateTimeTestDto
    {
        public Application.TypeTests.DateTimeTestDto ToContract()
        {
            return new Application.TypeTests.DateTimeTestDto
            {
                DateTimeField = DateTimeField.ToDateTime(),
                DateTimeFieldCollection = DateTimeFieldCollection.Select(x => x.ToDateTime()).ToList(),
                DateTimeFieldNullable = DateTimeFieldNullable != null ? DateTimeFieldNullable.ToDateTime() : null,
                DateTimeFieldNullableCollection = DateTimeFieldNullableCollection?.Items.Select(x => x.ToDateTime()).ToList()
            };
        }

        [return: NotNullIfNotNull(nameof(contract))]
        public static DateTimeTestDto? Create(Application.TypeTests.DateTimeTestDto? contract)
        {
            if (contract == null)
            {
                return null;
            }

            var message = new DateTimeTestDto
            {
                DateTimeField = Timestamp.FromDateTime(contract.DateTimeField),
                DateTimeFieldNullable = contract.DateTimeFieldNullable != null ? Timestamp.FromDateTime(contract.DateTimeFieldNullable.Value) : null,
                DateTimeFieldNullableCollection = ListOfTimestamp.Create(contract.DateTimeFieldNullableCollection?.Select(Timestamp.FromDateTime))
            };

            message.DateTimeFieldCollection.AddRange(contract.DateTimeFieldCollection.Select(Timestamp.FromDateTime));
            return message;
        }
    }
}