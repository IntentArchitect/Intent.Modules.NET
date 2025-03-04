using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace OcelotTest.Application.Cqrs
{
    public class GetAggregateRootLongsQuery
    {
        public GetAggregateRootLongsQuery()
        {
        }

        public static GetAggregateRootLongsQuery Create()
        {
            return new GetAggregateRootLongsQuery
            {
            };
        }
    }
}