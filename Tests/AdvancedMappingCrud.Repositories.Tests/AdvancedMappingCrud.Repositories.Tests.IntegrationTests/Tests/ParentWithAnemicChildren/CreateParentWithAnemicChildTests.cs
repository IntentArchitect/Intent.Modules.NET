using AdvancedMappingCrud.Repositories.Tests.IntegrationTests.HttpClients.ParentWithAnemicChildren;
using AdvancedMappingCrud.Repositories.Tests.IntegrationTests.Services.ParentWithAnemicChildren;
using AutoFixture;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.ServiceEndpointTest", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.IntegrationTests.Tests
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    [Collection("SharedContainer")]
    public class CreateParentWithAnemicChildTests : BaseIntegrationTest
    {
        public CreateParentWithAnemicChildTests(IntegrationTestWebAppFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task CreateParentWithAnemicChild_ShouldCreateParentWithAnemicChild()
        {
            // Arrange
            var client = new ParentWithAnemicChildrenHttpClient(CreateClient());

            var dataFactory = new TestDataFactory(WebAppFactory);

            var command = dataFactory.CreateCommand<CreateParentWithAnemicChildCommand>();

            // Act
            var parentWithAnemicChildId = await client.CreateParentWithAnemicChildAsync(command);

            // Assert
            var parentWithAnemicChild = await client.GetParentWithAnemicChildByIdAsync(parentWithAnemicChildId);
            Assert.NotNull(parentWithAnemicChild);
        }
    }
}