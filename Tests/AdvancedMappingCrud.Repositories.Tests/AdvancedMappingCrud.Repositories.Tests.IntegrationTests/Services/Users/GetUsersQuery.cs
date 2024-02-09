using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.DtoContract", Version = "2.0")]

namespace AdvancedMappingCrud.Repositories.Tests.IntegrationTests.Services.Users
{
    public class GetUsersQuery
    {
        public string? Name { get; set; }
        public string? Surname { get; set; }
        public int PageNo { get; set; }
        public int PageSize { get; set; }

        public static GetUsersQuery Create(string? name, string? surname, int pageNo, int pageSize)
        {
            return new GetUsersQuery
            {
                Name = name,
                Surname = surname,
                PageNo = pageNo,
                PageSize = pageSize
            };
        }
    }
}