using AdvancedMappingCrud.Repositories.Tests.IntegrationTests.HttpClients.ParentWithAnemicChildren;
using AutoFixture;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.ServiceEndpointTest", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.IntegrationTests.Tests
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    [Collection("SharedContainer")]
    public class GetParentWithAnemicChildByIdTests : BaseIntegrationTest
    {
        public GetParentWithAnemicChildByIdTests(IntegrationTestWebAppFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task GetParentWithAnemicChildById_ShouldGetParentWithAnemicChildById()
        {
            // Arrange
            var client = new ParentWithAnemicChildrenHttpClient(CreateClient());

            var dataFactory = new TestDataFactory(WebAppFactory);
            var parentWithAnemicChildId = await dataFactory.CreateParentWithAnemicChild();

            // Act
            var parentWithAnemicChild = await client.GetParentWithAnemicChildByIdAsync(parentWithAnemicChildId);

            // Assert
            Assert.NotNull(parentWithAnemicChild);
        }
    }
}