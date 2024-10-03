using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.DtoContract", Version = "2.0")]

namespace Ardalis.IntegrationTests.Services.Clients
{
    public class GetClientsPaginatedQuery
    {
        public int PageNo { get; set; }
        public int PageSize { get; set; }

        public static GetClientsPaginatedQuery Create(int pageNo, int pageSize)
        {
            return new GetClientsPaginatedQuery
            {
                PageNo = pageNo,
                PageSize = pageSize
            };
        }
    }
}