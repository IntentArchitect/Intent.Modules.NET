using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Application.QueryDtoParameter
{
    public class QueryDtoParameterCriteria
    {
        public QueryDtoParameterCriteria()
        {
            Field1 = null!;
            Field2 = null!;
            Nested = null!;
        }

        public string Field1 { get; set; }
        public string Field2 { get; set; }
        public NestedQueryDto Nested { get; set; }

        public static QueryDtoParameterCriteria Create(string field1, string field2, NestedQueryDto nested)
        {
            return new QueryDtoParameterCriteria
            {
                Field1 = field1,
                Field2 = field2,
                Nested = nested
            };
        }
    }
}