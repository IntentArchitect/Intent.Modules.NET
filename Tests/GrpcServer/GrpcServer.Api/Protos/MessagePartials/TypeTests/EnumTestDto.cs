using System.Diagnostics.CodeAnalysis;
using System.Linq;
using GrpcServer.Application.TypeTests;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.Grpc.MessagePartial", Version = "1.0")]

namespace GrpcServer.Api.Protos.Messages.TypeTests
{
    public partial class EnumTestDto
    {
        public Application.TypeTests.EnumTestDto ToContract()
        {
            return new Application.TypeTests.EnumTestDto
            {
                EnumField = (EnumType)EnumField,
                EnumFieldCollection = EnumFieldCollection.Cast<EnumType>().ToList(),
                EnumFieldNullable = (EnumType?)EnumFieldNullable,
                EnumFieldNullableCollection = EnumFieldNullableCollection?.Items.Cast<EnumType>().ToList()
            };
        }

        [return: NotNullIfNotNull(nameof(contract))]
        public static EnumTestDto? Create(Application.TypeTests.EnumTestDto? contract)
        {
            if (contract == null)
            {
                return null;
            }

            var message = new EnumTestDto
            {
                EnumField = (int)contract.EnumField,
                EnumFieldNullable = (int?)contract.EnumFieldNullable,
                EnumFieldNullableCollection = ListOfInt32.Create(contract.EnumFieldNullableCollection?.Cast<int>())
            };

            message.EnumFieldCollection.AddRange(contract.EnumFieldCollection.Cast<int>());
            return message;
        }
    }
}