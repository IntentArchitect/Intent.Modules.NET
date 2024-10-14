using AdvancedMappingCrud.Repositories.Tests.IntegrationTests.HttpClients.ParentWithAnemicChildren;
using AutoFixture;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.ServiceEndpointTest", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.IntegrationTests.Tests
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    [Collection("SharedContainer")]
    public class GetParentWithAnemicChildrenTests : BaseIntegrationTest
    {
        public GetParentWithAnemicChildrenTests(IntegrationTestWebAppFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task GetParentWithAnemicChildren_ShouldGetParentWithAnemicChildren()
        {
            // Arrange
            var client = new ParentWithAnemicChildrenHttpClient(CreateClient());

            var dataFactory = new TestDataFactory(WebAppFactory);
            await dataFactory.CreateParentWithAnemicChild();

            // Act
            var parentWithAnemicChildren = await client.GetParentWithAnemicChildrenAsync();

            // Assert
            Assert.True(parentWithAnemicChildren.Count > 0);
        }
    }
}