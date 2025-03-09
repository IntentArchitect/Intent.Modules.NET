using System.Diagnostics.CodeAnalysis;
using System.Linq;
using GrpcServer.Application.TypeTests;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.Grpc.MessagePartial", Version = "1.0")]

namespace GrpcServer.Api.Protos.Messages.TypeTests
{
    public partial class StringTestDto
    {
        public Application.TypeTests.StringTestDto ToContract()
        {
            return new Application.TypeTests.StringTestDto
            {
                StringField = StringField,
                StringFieldCollection = StringFieldCollection.ToList(),
                StringFieldNullable = StringFieldNullable,
                StringFieldNullableCollection = StringFieldNullableCollection?.Items.ToList()
            };
        }

        [return: NotNullIfNotNull(nameof(contract))]
        public static StringTestDto? Create(Application.TypeTests.StringTestDto? contract)
        {
            if (contract == null)
            {
                return null;
            }

            var message = new StringTestDto
            {
                StringField = contract.StringField,
                StringFieldNullable = contract.StringFieldNullable,
                StringFieldNullableCollection = ListOfString.Create(contract.StringFieldNullableCollection)
            };

            message.StringFieldCollection.AddRange(contract.StringFieldCollection);
            return message;
        }
    }
}