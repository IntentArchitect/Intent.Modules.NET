using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using GrpcServer.Application.TypeTests;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.Grpc.MessagePartial", Version = "1.0")]

namespace GrpcServer.Api.Protos.Messages.TypeTests
{
    public partial class GuidTestDto
    {
        public Application.TypeTests.GuidTestDto ToContract()
        {
            return new Application.TypeTests.GuidTestDto
            {
                GuidField = Guid.Parse(GuidField),
                GuidFieldCollection = GuidFieldCollection.Select(Guid.Parse).ToList(),
                GuidFieldNullable = GuidFieldNullable != null ? Guid.Parse(GuidFieldNullable) : null,
                GuidFieldNullableCollection = GuidFieldNullableCollection?.Items.Select(Guid.Parse).ToList()
            };
        }

        [return: NotNullIfNotNull(nameof(contract))]
        public static GuidTestDto? Create(Application.TypeTests.GuidTestDto? contract)
        {
            if (contract == null)
            {
                return null;
            }

            var message = new GuidTestDto
            {
                GuidField = contract.GuidField.ToString(),
                GuidFieldNullable = contract.GuidFieldNullable?.ToString(),
                GuidFieldNullableCollection = ListOfString.Create(contract.GuidFieldNullableCollection?.Select(x => x.ToString()))
            };

            message.GuidFieldCollection.AddRange(contract.GuidFieldCollection.Select(x => x.ToString()));
            return message;
        }
    }
}