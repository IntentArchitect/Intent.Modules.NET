using Blazor.InteractiveAuto.Oidc.Client.Common;
using Intent.RoslynWeaver.Attributes;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Blazor.Templates.Client.ProgramTemplate", Version = "1.0")]

namespace Blazor.InteractiveAuto.Oidc.Client
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            await LoadAppSettings(builder);
            builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
            builder.Services.AddClientServices(builder.Configuration);
            builder.Services.AddAuthorizationCore();

            builder.Services.AddCascadingAuthenticationState();

            builder.Services.AddSingleton<AuthenticationStateProvider, PersistentAuthenticationStateProvider>();

            builder.Services.AddApiAuthorization(); ;

            builder.Services.AddMudServices();

            await builder.Build().RunAsync();
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