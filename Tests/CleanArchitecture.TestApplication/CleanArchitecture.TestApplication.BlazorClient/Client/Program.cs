using CleanArchitecture.TestApplication.BlazorClient;
using CleanArchitecture.TestApplication.BlazorClient.HttpClients;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration.Memory;

namespace CleanArchitecture.TestApplication.BlazorClient
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");
            builder.RootComponents.Add<HeadOutlet>("head::after");

            builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
            builder.Configuration.Add(new MemoryConfigurationSource
            {
                InitialData = new Dictionary<string, string>
                {
                    ["Urls:CleanArchitectureTestApplication"] = "https://localhost:44341/"
                }
            });
            builder.Services.AddHttpClients(builder.Configuration);

            await builder.Build().RunAsync();
        }
    }
}