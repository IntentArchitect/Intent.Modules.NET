using Intent.RoslynWeaver.Attributes;
using MudBlazor.Sample.Client.HttpClients.Implementations;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.Blazor.HttpClients.HttpClientConfiguration", Version = "2.0")]

namespace MudBlazor.Sample.Client.HttpClients
{
    public static class HttpClientConfiguration
    {
        public static void AddHttpClients(this IServiceCollection services, IConfiguration configuration)
        {
            services
                .AddHttpClient<ICustomersService, CustomersServiceHttpClient>(http =>
                {
                    http.BaseAddress = GetUrl(configuration, "MudBlazorSample");
                });

            services
                .AddHttpClient<IInvoiceService, InvoiceServiceHttpClient>(http =>
                {
                    http.BaseAddress = GetUrl(configuration, "MudBlazorSample");
                });

            services
                .AddHttpClient<IProductsService, ProductsServiceHttpClient>(http =>
                {
                    http.BaseAddress = GetUrl(configuration, "MudBlazorSample");
                });
        }

        private static Uri GetUrl(IConfiguration configuration, string applicationName)
        {
            var url = configuration.GetValue<Uri?>($"Urls:{applicationName}");

            return url ?? throw new Exception($"Configuration key \"Urls:{applicationName}\" is not set");
        }
    }
}