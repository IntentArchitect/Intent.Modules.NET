using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Google.Protobuf.WellKnownTypes;
using GrpcServer.Application.TypeTests;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.Grpc.MessagePartial", Version = "1.0")]

namespace GrpcServer.Api.Protos.Messages.TypeTests
{
    public partial class TimeSpanTestDto
    {
        public Application.TypeTests.TimeSpanTestDto ToContract()
        {
            return new Application.TypeTests.TimeSpanTestDto
            {
                TimeSpanField = TimeSpanField.ToTimeSpan(),
                TimeSpanFieldCollection = TimeSpanFieldCollection.Select(x => x.ToTimeSpan()).ToList(),
                TimeSpanFieldNullable = TimeSpanFieldNullable?.ToTimeSpan(),
                TimeSpanFieldNullableCollection = TimeSpanFieldNullableCollection?.Items.Select(x => x.ToTimeSpan()).ToList()
            };
        }

        [return: NotNullIfNotNull(nameof(contract))]
        public static TimeSpanTestDto? Create(Application.TypeTests.TimeSpanTestDto? contract)
        {
            if (contract == null)
            {
                return null;
            }

            var message = new TimeSpanTestDto
            {
                TimeSpanField = Duration.FromTimeSpan(contract.TimeSpanField),
                TimeSpanFieldNullable = contract.TimeSpanFieldNullable != null ? Duration.FromTimeSpan(contract.TimeSpanFieldNullable.Value) : null,
                TimeSpanFieldNullableCollection = ListOfDuration.Create(contract.TimeSpanFieldNullableCollection)
            };

            message.TimeSpanFieldCollection.AddRange(contract.TimeSpanFieldCollection.Select(Duration.FromTimeSpan));
            return message;
        }
    }
}