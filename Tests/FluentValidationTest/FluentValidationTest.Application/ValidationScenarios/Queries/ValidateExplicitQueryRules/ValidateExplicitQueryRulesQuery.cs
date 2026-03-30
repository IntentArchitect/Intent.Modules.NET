using FluentValidationTest.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryModels", Version = "1.0")]

namespace FluentValidationTest.Application.ValidationScenarios.Queries.ValidateExplicitQueryRules
{
    public class ValidateExplicitQueryRulesQuery : IRequest<int>, IQuery
    {
        public ValidateExplicitQueryRulesQuery(string requiredFilter, string? optionalFilter, int pageNo, int pageSize)
        {
            RequiredFilter = requiredFilter;
            OptionalFilter = optionalFilter;
            PageNo = pageNo;
            PageSize = pageSize;
        }

        public string RequiredFilter { get; set; }
        public string? OptionalFilter { get; set; }
        public int PageNo { get; set; }
        public int PageSize { get; set; }
    }
}