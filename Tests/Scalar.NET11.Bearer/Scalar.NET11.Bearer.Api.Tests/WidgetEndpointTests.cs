using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using Microsoft.AspNetCore.Mvc.Testing;
using Scalar.NET11.Bearer.Api;
using Scalar.NET11.Bearer.Application;
using Xunit;

namespace Scalar.NET11.Bearer.Api.Tests
{
    /// <summary>
    /// Calls each endpoint behind the documented operations once.
    /// </summary>
    public class WidgetEndpointTests(WebApplicationFactory<Program> factory) : IClassFixture<WebApplicationFactory<Program>>
    {
        [Fact]
        public async Task Update_ReturnsNoContent()
        {
            var client = factory.CreateClient();
            var id = await CreateWidgetAsync(client, "Before update");

            var response = await client.PutAsJsonAsync($"api/widgets/{id}", new { name = "After update" }, TestContext.Current.CancellationToken);

            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        }

        [Fact]
        public async Task Query_ReturnsMatchingWidgets()
        {
            var client = factory.CreateClient();
            var name = $"Queried {Guid.NewGuid():N}";
            var id = await CreateWidgetAsync(client, name);

            var request = new HttpRequestMessage(HttpMethod.Query, "api/widgets/search")
            {
                Content = JsonContent.Create(new { name })
            };
            var response = await client.SendAsync(request, TestContext.Current.CancellationToken);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var widgets = await response.Content.ReadFromJsonAsync<List<WidgetDto>>(TestContext.Current.CancellationToken);
            Assert.Contains(widgets!, w => w.Id == id);
        }

        [Fact]
        public async Task Upload_ReturnsCreated()
        {
            var client = factory.CreateClient();
            var content = new ByteArrayContent([1, 2, 3, 4]);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
            content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment") { FileName = "widget.bin" };

            var response = await client.PostAsync("api/widgets/images", content, TestContext.Current.CancellationToken);

            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            Assert.NotEqual(Guid.Empty, await response.Content.ReadFromJsonAsync<Guid>(TestContext.Current.CancellationToken));
        }

        [Fact]
        public async Task MergePatch_ReturnsPatchedWidget()
        {
            var client = factory.CreateClient();
            var id = await CreateWidgetAsync(client, "Before patch");

            var content = new StringContent("""{ "name": "After patch" }""", Encoding.UTF8, "application/merge-patch+json");
            var response = await client.PatchAsync($"api/widgets/{id}", content, TestContext.Current.CancellationToken);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var widget = await response.Content.ReadFromJsonAsync<WidgetDto>(TestContext.Current.CancellationToken);
            Assert.Equal("After patch", widget!.Name);
        }

        private static async Task<Guid> CreateWidgetAsync(HttpClient client, string name)
        {
            var response = await client.PostAsJsonAsync("api/widgets", new { name }, TestContext.Current.CancellationToken);
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            return await response.Content.ReadFromJsonAsync<Guid>(TestContext.Current.CancellationToken);
        }
    }
}
