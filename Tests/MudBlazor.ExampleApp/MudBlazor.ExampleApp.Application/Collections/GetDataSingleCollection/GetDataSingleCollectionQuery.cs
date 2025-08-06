using Intent.RoslynWeaver.Attributes;
using MediatR;
using MudBlazor.ExampleApp.Application.Common.Interfaces;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryModels", Version = "1.0")]

namespace MudBlazor.ExampleApp.Application.Collections.GetDataSingleCollection
{
    public class GetDataSingleCollectionQuery : IRequest<List<ResponseDto>>, IQuery
    {
        public GetDataSingleCollectionQuery(List<int> intCollection, string stringValue)
        {
            IntCollection = intCollection;
            StringValue = stringValue;
        }

        public List<int> IntCollection { get; set; }
        public string StringValue { get; set; }
    }
}