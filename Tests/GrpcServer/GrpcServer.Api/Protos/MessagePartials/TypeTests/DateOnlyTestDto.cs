using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Google.Protobuf.WellKnownTypes;
using GrpcServer.Application.TypeTests;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.Grpc.MessagePartial", Version = "1.0")]

namespace GrpcServer.Api.Protos.Messages.TypeTests
{
    public partial class DateOnlyTestDto
    {
        public Application.TypeTests.DateOnlyTestDto ToContract()
        {
            return new Application.TypeTests.DateOnlyTestDto
            {
                DateOnlyField = DateOnly.FromDateTime(DateOnlyField.ToDateTime()),
                DateOnlyFieldCollection = DateOnlyFieldCollection.Select(x => DateOnly.FromDateTime(x.ToDateTime())).ToList(),
                DateOnlyFieldNullable = DateOnlyFieldNullable != null ? DateOnly.FromDateTime(DateOnlyFieldNullable.ToDateTime()) : null,
                DateOnlyFieldNullableCollection = DateOnlyFieldNullableCollection?.Items.Select(x => DateOnly.FromDateTime(x.ToDateTime())).ToList()
            };
        }

        [return: NotNullIfNotNull(nameof(contract))]
        public static DateOnlyTestDto? Create(Application.TypeTests.DateOnlyTestDto? contract)
        {
            if (contract == null)
            {
                return null;
            }

            var message = new DateOnlyTestDto
            {
                DateOnlyField = Timestamp.FromDateTime(contract.DateOnlyField.ToDateTime(TimeOnly.MinValue)),
                DateOnlyFieldNullable = contract.DateOnlyFieldNullable != null ? Timestamp.FromDateTime(contract.DateOnlyFieldNullable.Value.ToDateTime(TimeOnly.MinValue)) : null,
                DateOnlyFieldNullableCollection = ListOfTimestamp.Create(contract.DateOnlyFieldNullableCollection?.Select(x => Timestamp.FromDateTime(x.ToDateTime(TimeOnly.MinValue))))
            };

            message.DateOnlyFieldCollection.AddRange(contract.DateOnlyFieldCollection.Select(x => Timestamp.FromDateTime(x.ToDateTime(TimeOnly.MinValue))));
            return message;
        }
    }
}