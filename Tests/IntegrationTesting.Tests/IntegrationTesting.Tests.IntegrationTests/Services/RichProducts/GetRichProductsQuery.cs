using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.DtoContract", Version = "2.0")]

namespace IntegrationTesting.Tests.IntegrationTests.Services.RichProducts
{
    public class GetRichProductsQuery
    {
        public static GetRichProductsQuery Create()
        {
            return new GetRichProductsQuery
            {
            };
        }
    }
}