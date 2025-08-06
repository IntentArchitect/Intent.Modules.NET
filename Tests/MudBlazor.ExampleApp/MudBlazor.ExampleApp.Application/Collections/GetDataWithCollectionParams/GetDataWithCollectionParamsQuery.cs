using Intent.RoslynWeaver.Attributes;
using MediatR;
using MudBlazor.ExampleApp.Application.Common.Interfaces;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryModels", Version = "1.0")]

namespace MudBlazor.ExampleApp.Application.Collections.GetDataWithCollectionParams
{
    public class GetDataWithCollectionParamsQuery : IRequest<List<ResponseDto>>, IQuery
    {
        public GetDataWithCollectionParamsQuery(List<int> intCollection, List<string> stringCollection, int intValue)
        {
            IntCollection = intCollection;
            StringCollection = stringCollection;
            IntValue = intValue;
        }

        public List<int> IntCollection { get; set; }
        public List<string> StringCollection { get; set; }
        public int IntValue { get; set; }
    }
}