using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Google.Protobuf.WellKnownTypes;
using GrpcServer.Application.TypeTests;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.Grpc.MessagePartial", Version = "1.0")]

namespace GrpcServer.Api.Protos.Messages.TypeTests
{
    public partial class DateTimeOffsetTestDto
    {
        public Application.TypeTests.DateTimeOffsetTestDto ToContract()
        {
            return new Application.TypeTests.DateTimeOffsetTestDto
            {
                DateTimeOffsetField = DateTimeOffsetField.ToDateTimeOffset(),
                DateTimeOffsetFieldCollection = DateTimeOffsetFieldCollection.Select(x => x.ToDateTimeOffset()).ToList(),
                DateTimeOffsetFieldNullable = DateTimeOffsetFieldNullable != null ? DateTimeOffsetFieldNullable.ToDateTimeOffset() : null,
                DateTimeOffsetFieldNullableCollection = DateTimeOffsetFieldNullableCollection?.Items.Select(x => x.ToDateTimeOffset()).ToList()
            };
        }

        [return: NotNullIfNotNull(nameof(contract))]
        public static DateTimeOffsetTestDto? Create(Application.TypeTests.DateTimeOffsetTestDto? contract)
        {
            if (contract == null)
            {
                return null;
            }

            var message = new DateTimeOffsetTestDto
            {
                DateTimeOffsetField = Timestamp.FromDateTimeOffset(contract.DateTimeOffsetField),
                DateTimeOffsetFieldNullable = contract.DateTimeOffsetFieldNullable != null ? Timestamp.FromDateTimeOffset(contract.DateTimeOffsetFieldNullable.Value) : null,
                DateTimeOffsetFieldNullableCollection = ListOfTimestamp.Create(contract.DateTimeOffsetFieldNullableCollection?.Select(Timestamp.FromDateTimeOffset))
            };

            message.DateTimeOffsetFieldCollection.AddRange(contract.DateTimeOffsetFieldCollection.Select(Timestamp.FromDateTimeOffset));
            return message;
        }
    }
}