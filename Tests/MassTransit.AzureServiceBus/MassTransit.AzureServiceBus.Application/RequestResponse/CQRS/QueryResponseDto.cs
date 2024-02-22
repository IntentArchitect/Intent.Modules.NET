using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace MassTransit.AzureServiceBus.Application.RequestResponse.CQRS
{
    public class QueryResponseDto
    {
        public QueryResponseDto()
        {
            Result = null!;
        }

        public string Result { get; set; }

        public static QueryResponseDto Create(string result)
        {
            return new QueryResponseDto
            {
                Result = result
            };
        }
    }
}