using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.Application.Contracts.Clients.DtoContract", Version = "2.0")]

namespace CleanArchitecture.Comprehensive.Application.IntegrationServices.Services.QueryDtoParameter
{
    public class QueryDtoParameterCriteria
    {
        public QueryDtoParameterCriteria()
        {
            Field1 = null!;
            Field2 = null!;
        }

        public string Field1 { get; set; }
        public string Field2 { get; set; }

        public static QueryDtoParameterCriteria Create(string field1, string field2)
        {
            return new QueryDtoParameterCriteria
            {
                Field1 = field1,
                Field2 = field2
            };
        }
    }
}