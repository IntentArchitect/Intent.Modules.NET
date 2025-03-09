using System.Diagnostics.CodeAnalysis;
using System.Linq;
using GrpcServer.Application.TypeTests;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.Grpc.MessagePartial", Version = "1.0")]

namespace GrpcServer.Api.Protos.Messages.TypeTests
{
    public partial class DoubleTestDto
    {
        public Application.TypeTests.DoubleTestDto ToContract()
        {
            return new Application.TypeTests.DoubleTestDto
            {
                DoubleField = DoubleField,
                DoubleFieldCollection = DoubleFieldCollection.ToList(),
                DoubleFieldNullable = DoubleFieldNullable,
                DoubleFieldNullableCollection = DoubleFieldNullableCollection?.Items.ToList()
            };
        }

        [return: NotNullIfNotNull(nameof(contract))]
        public static DoubleTestDto? Create(Application.TypeTests.DoubleTestDto? contract)
        {
            if (contract == null)
            {
                return null;
            }

            var message = new DoubleTestDto
            {
                DoubleField = contract.DoubleField,
                DoubleFieldNullable = contract.DoubleFieldNullable,
                DoubleFieldNullableCollection = ListOfDouble.Create(contract.DoubleFieldNullableCollection)
            };

            message.DoubleFieldCollection.AddRange(contract.DoubleFieldCollection);
            return message;
        }
    }
}