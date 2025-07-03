using System.Net;
using AdvancedMappingCrud.Repositories.Tests.IntegrationTests.HttpClients;
using AdvancedMappingCrud.Repositories.Tests.IntegrationTests.HttpClients.ParentWithAnemicChildren;
using AutoFixture;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.ServiceEndpointTest", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.IntegrationTests.Tests.ParentWithAnemicChildren
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    [Collection("SharedContainer")]
    public class DeleteParentWithAnemicChildTests : BaseIntegrationTest
    {
        public DeleteParentWithAnemicChildTests(IntegrationTestWebAppFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task DeleteParentWithAnemicChild_ShouldDeleteParentWithAnemicChild()
        {
            // Arrange
            var client = new ParentWithAnemicChildrenHttpClient(CreateClient());

            var dataFactory = new TestDataFactory(WebAppFactory);
            var parentWithAnemicChildId = await dataFactory.CreateParentWithAnemicChild();

            // Act
            await client.DeleteParentWithAnemicChildAsync(parentWithAnemicChildId, TestContext.Current.CancellationToken);

            // Assert
            var exception = await Assert.ThrowsAsync<HttpClientRequestException>(() => client.GetParentWithAnemicChildByIdAsync(parentWithAnemicChildId, TestContext.Current.CancellationToken));
            Assert.Equal(HttpStatusCode.NotFound, exception.StatusCode);
        }
    }
}