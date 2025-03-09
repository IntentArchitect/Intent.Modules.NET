using System.Diagnostics.CodeAnalysis;
using System.Linq;
using GrpcServer.Application.TypeTests;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.Grpc.MessagePartial", Version = "1.0")]

namespace GrpcServer.Api.Protos.Messages.TypeTests
{
    public partial class DictionaryTestDto
    {
        public Application.TypeTests.DictionaryTestDto ToContract()
        {
            return new Application.TypeTests.DictionaryTestDto
            {
                DictionaryField = DictionaryField.ToDictionary(),
                DictionaryFieldCollection = DictionaryFieldCollection.Select(x => x.Items.ToDictionary()).ToList(),
                DictionaryFieldNullable = DictionaryFieldNullable?.Items.ToDictionary(),
                DictionaryFieldNullableCollection = DictionaryFieldNullableCollection?.Items.Select(x => x.Items.ToDictionary()).ToList()
            };
        }

        [return: NotNullIfNotNull(nameof(contract))]
        public static DictionaryTestDto? Create(Application.TypeTests.DictionaryTestDto? contract)
        {
            if (contract == null)
            {
                return null;
            }

            var message = new DictionaryTestDto
            {
                DictionaryFieldNullable = MapOfStringAndString.Create(contract.DictionaryFieldNullable),
                DictionaryFieldNullableCollection = ListOfMapOfStringAndString.Create(contract.DictionaryFieldNullableCollection)
            };

            message.DictionaryField.Add(contract.DictionaryField);
            message.DictionaryFieldCollection.AddRange(contract.DictionaryFieldCollection.Select(MapOfStringAndString.Create));
            return message;
        }
    }
}