using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;
using GrpcServer.Application.TypeTests;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.Grpc.MessagePartial", Version = "1.0")]

namespace GrpcServer.Api.Protos.Messages.TypeTests
{
    public partial class ObjectTestDto
    {
        public Application.TypeTests.ObjectTestDto ToContract()
        {
            return new Application.TypeTests.ObjectTestDto
            {
                ObjectField = ObjectField,
                ObjectFieldCollection = ObjectFieldCollection.Cast<object>().ToList(),
                ObjectFieldNullable = ObjectFieldNullable,
                ObjectFieldNullableCollection = ObjectFieldNullableCollection?.Items.Cast<object>().ToList()
            };
        }

        [return: NotNullIfNotNull(nameof(contract))]
        public static ObjectTestDto? Create(Application.TypeTests.ObjectTestDto? contract)
        {
            if (contract == null)
            {
                return null;
            }

            var message = new ObjectTestDto
            {
                ObjectField = Any.Pack((IMessage)contract.ObjectField),
                ObjectFieldNullable = contract.ObjectFieldNullable != null ? Any.Pack((IMessage)contract.ObjectFieldNullable) : null,
                ObjectFieldNullableCollection = ListOfAny.Create(contract.ObjectFieldNullableCollection)
            };

            message.ObjectFieldCollection.AddRange(contract.ObjectFieldCollection.Select(x => Any.Pack((IMessage)x)));
            return message;
        }
    }
}