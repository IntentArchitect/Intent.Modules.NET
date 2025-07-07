using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.Blazor.HttpClients.DtoContract", Version = "2.0")]

namespace MudBlazor.ExampleApp.Client.HttpClients.Contracts.Services.Customers
{
    public class GetCustomersQuery
    {
        public int PageNo { get; set; }
        public int PageSize { get; set; }
        public string? OrderBy { get; set; }
        public string? SearchText { get; set; }

        public static GetCustomersQuery Create(int pageNo, int pageSize, string? orderBy, string? searchText)
        {
            return new GetCustomersQuery
            {
                PageNo = pageNo,
                PageSize = pageSize,
                OrderBy = orderBy,
                SearchText = searchText
            };
        }
    }
}