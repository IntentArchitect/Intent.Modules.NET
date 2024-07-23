using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.DtoContract", Version = "2.0")]

namespace AdvancedMappingCrud.Cosmos.Tests.IntegrationTests.Services.BasicOrderBies
{
    public class GetBasicOrderByQuery
    {
        public GetBasicOrderByQuery()
        {
            OrderBy = null!;
        }

        public int PageNo { get; set; }
        public int PageSize { get; set; }
        public string OrderBy { get; set; }

        public static GetBasicOrderByQuery Create(int pageNo, int pageSize, string orderBy)
        {
            return new GetBasicOrderByQuery
            {
                PageNo = pageNo,
                PageSize = pageSize,
                OrderBy = orderBy
            };
        }
    }
}