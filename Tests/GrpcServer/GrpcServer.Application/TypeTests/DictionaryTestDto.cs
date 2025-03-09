using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace GrpcServer.Application.TypeTests
{
    public class DictionaryTestDto
    {
        public DictionaryTestDto()
        {
            DictionaryField = null!;
            DictionaryFieldCollection = null!;
        }

        public Dictionary<string, string> DictionaryField { get; set; }
        public List<Dictionary<string, string>> DictionaryFieldCollection { get; set; }
        public Dictionary<string, string>? DictionaryFieldNullable { get; set; }
        public List<Dictionary<string, string>>? DictionaryFieldNullableCollection { get; set; }

        public static DictionaryTestDto Create(
            Dictionary<string, string> dictionaryField,
            List<Dictionary<string, string>> dictionaryFieldCollection,
            Dictionary<string, string>? dictionaryFieldNullable,
            List<Dictionary<string, string>>? dictionaryFieldNullableCollection)
        {
            return new DictionaryTestDto
            {
                DictionaryField = dictionaryField,
                DictionaryFieldCollection = dictionaryFieldCollection,
                DictionaryFieldNullable = dictionaryFieldNullable,
                DictionaryFieldNullableCollection = dictionaryFieldNullableCollection
            };
        }
    }
}