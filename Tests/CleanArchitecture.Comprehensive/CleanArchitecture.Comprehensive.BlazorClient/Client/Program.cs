using CleanArchitecture.Comprehensive.BlazorClient.Client.Common.Validation;
using Intent.RoslynWeaver.Attributes;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Blazor.WebAssembly.ProgramTemplate", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.BlazorClient.Client
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");
            builder.RootComponents.Add<HeadOutlet>("head::after");
            await LoadAppSettings(builder);
            //IntentIgnore
            CustomRegistrations(builder);
            builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
            builder.Services.AddScoped<IValidatorProvider, ValidatorProvider>();
            await builder.Build().RunAsync();
        }

        [IntentManaged(Mode.Ignore)]
        private static void CustomRegistrations(WebAssemblyHostBuilder builder)
        {
            builder.Services.AddScoped<IAccessTokenProvider, AccessTokenProvider>();
            builder.Services.AddTransient<AuthorizationMessageHandler>();
        }

        public static async Task LoadAppSettings(WebAssemblyHostBuilder builder)
        {
            var configProxy = new HttpClient() { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) };
            using var response = await configProxy.GetAsync("appsettings.json");
            using var stream = await response.Content.ReadAsStreamAsync();
            builder.Configuration.AddJsonStream(stream);
        }
    }
}