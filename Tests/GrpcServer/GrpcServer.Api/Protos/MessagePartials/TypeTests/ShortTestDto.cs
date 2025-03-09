using System.Diagnostics.CodeAnalysis;
using System.Linq;
using GrpcServer.Application.TypeTests;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.Grpc.MessagePartial", Version = "1.0")]

namespace GrpcServer.Api.Protos.Messages.TypeTests
{
    public partial class ShortTestDto
    {
        public Application.TypeTests.ShortTestDto ToContract()
        {
            return new Application.TypeTests.ShortTestDto
            {
                ShortField = (short)ShortField,
                ShortFieldCollection = ShortFieldCollection.Select(x => (short)x).ToList(),
                ShortFieldNullable = (short?)ShortFieldNullable,
                ShortFieldNullableCollection = ShortFieldNullableCollection?.Items.Select(x => (short)x).ToList()
            };
        }

        [return: NotNullIfNotNull(nameof(contract))]
        public static ShortTestDto? Create(Application.TypeTests.ShortTestDto? contract)
        {
            if (contract == null)
            {
                return null;
            }

            var message = new ShortTestDto
            {
                ShortField = contract.ShortField,
                ShortFieldNullable = contract.ShortFieldNullable,
                ShortFieldNullableCollection = ListOfInt32.Create(contract.ShortFieldNullableCollection?.Select(x => (int)x))
            };

            message.ShortFieldCollection.AddRange(contract.ShortFieldCollection.Select(x => (int)x));
            return message;
        }
    }
}