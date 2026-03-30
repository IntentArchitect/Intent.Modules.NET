using FluentValidationTest.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryModels", Version = "1.0")]

namespace FluentValidationTest.Application.ValidationScenarios.Nullability.SearchNullabilityCases
{
    public class SearchNullabilityCasesQuery : IRequest<int>, IQuery
    {
        public SearchNullabilityCasesQuery(Guid tenantId, string? category, int? minValue, int pageNo, int pageSize)
        {
            TenantId = tenantId;
            Category = category;
            MinValue = minValue;
            PageNo = pageNo;
            PageSize = pageSize;
        }

        public Guid TenantId { get; set; }
        public string? Category { get; set; }
        public int? MinValue { get; set; }
        public int PageNo { get; set; }
        public int PageSize { get; set; }
    }
}