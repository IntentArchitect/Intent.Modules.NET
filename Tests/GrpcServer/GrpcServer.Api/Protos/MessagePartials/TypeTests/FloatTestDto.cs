using System.Diagnostics.CodeAnalysis;
using System.Linq;
using GrpcServer.Application.TypeTests;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.Grpc.MessagePartial", Version = "1.0")]

namespace GrpcServer.Api.Protos.Messages.TypeTests
{
    public partial class FloatTestDto
    {
        public Application.TypeTests.FloatTestDto ToContract()
        {
            return new Application.TypeTests.FloatTestDto
            {
                FloatField = FloatField,
                FloatFieldCollection = FloatFieldCollection.ToList(),
                FloatFieldNullable = FloatFieldNullable,
                FloatFieldNullableCollection = FloatFieldNullableCollection?.Items.ToList()
            };
        }

        [return: NotNullIfNotNull(nameof(contract))]
        public static FloatTestDto? Create(Application.TypeTests.FloatTestDto? contract)
        {
            if (contract == null)
            {
                return null;
            }

            var message = new FloatTestDto
            {
                FloatField = contract.FloatField,
                FloatFieldNullable = contract.FloatFieldNullable,
                FloatFieldNullableCollection = ListOfFloat.Create(contract.FloatFieldNullableCollection)
            };

            message.FloatFieldCollection.AddRange(contract.FloatFieldCollection);
            return message;
        }
    }
}