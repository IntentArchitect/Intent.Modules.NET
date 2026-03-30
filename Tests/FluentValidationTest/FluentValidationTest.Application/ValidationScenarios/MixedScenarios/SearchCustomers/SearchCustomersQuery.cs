using FluentValidationTest.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryModels", Version = "1.0")]

namespace FluentValidationTest.Application.ValidationScenarios.MixedScenarios.SearchCustomers
{
    public class SearchCustomersQuery : IRequest<int>, IQuery
    {
        public SearchCustomersQuery(string? searchText, int pageNo, int pageSize)
        {
            SearchText = searchText;
            PageNo = pageNo;
            PageSize = pageSize;
        }

        public string? SearchText { get; set; }
        public int PageNo { get; set; }
        public int PageSize { get; set; }
    }
}