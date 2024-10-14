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
    public class UpdateParentWithAnemicChildTests : BaseIntegrationTest
    {
        public UpdateParentWithAnemicChildTests(IntegrationTestWebAppFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task UpdateParentWithAnemicChild_ShouldUpdateParentWithAnemicChild()
        {
            // Arrange
            var client = new ParentWithAnemicChildrenHttpClient(CreateClient());

            var dataFactory = new TestDataFactory(WebAppFactory);
            var parentWithAnemicChildId = await dataFactory.CreateParentWithAnemicChild();

            var command = dataFactory.CreateCommand<UpdateParentWithAnemicChildCommand>();
            command.Id = parentWithAnemicChildId;

            // Act
            await client.UpdateParentWithAnemicChildAsync(parentWithAnemicChildId, command);

            // Assert
            var parentWithAnemicChild = await client.GetParentWithAnemicChildByIdAsync(parentWithAnemicChildId);
            Assert.NotNull(parentWithAnemicChild);
            Assert.Equal(command.Name, parentWithAnemicChild.Name);
        }
    }
}