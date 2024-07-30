using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.DtoContract", Version = "2.0")]

namespace AdvancedMappingCrud.Repositories.Tests.IntegrationTests.Services.Basics
{
    public class GetBasicsNullableQuery
    {
        public int PageNo { get; set; }
        public int PageSize { get; set; }
        public string? OrderBy { get; set; }

        public static GetBasicsNullableQuery Create(int pageNo, int pageSize, string? orderBy)
        {
            return new GetBasicsNullableQuery
            {
                PageNo = pageNo,
                PageSize = pageSize,
                OrderBy = orderBy
            };
        }
    }
}