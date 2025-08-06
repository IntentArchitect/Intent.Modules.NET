using Intent.RoslynWeaver.Attributes;
using MudBlazor.ExampleApp.Client.HttpClients.Implementations;
using MudBlazor.ExampleApp.Client.HttpClients.Implementations.Customers;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.Blazor.HttpClients.HttpClientConfiguration", Version = "2.0")]

namespace MudBlazor.ExampleApp.Client.HttpClients
{
    public static class HttpClientConfiguration
    {
        public static void AddHttpClients(this IServiceCollection services, IConfiguration configuration)
        {
            services
                .AddHttpClient<ICollectionsService, CollectionsServiceHttpClient>(http =>
                {
                    http.BaseAddress = GetUrl(configuration, "MudBlazorExampleApp");
                });

            services
                .AddHttpClient<ICustomersService, CustomersServiceHttpClient>(http =>
                {
                    http.BaseAddress = GetUrl(configuration, "MudBlazorExampleApp");
                });

            services
                .AddHttpClient<IDummyService, DummyServiceHttpClient>(http =>
                {
                    http.BaseAddress = GetUrl(configuration, "MudBlazorExampleApp");
                });

            services
                .AddHttpClient<IInvoiceService, InvoiceServiceHttpClient>(http =>
                {
                    http.BaseAddress = GetUrl(configuration, "MudBlazorExampleApp");
                });

            services
                .AddHttpClient<IProductsService, ProductsServiceHttpClient>(http =>
                {
                    http.BaseAddress = GetUrl(configuration, "MudBlazorExampleApp");
                });

        }

        private static Uri GetUrl(IConfiguration configuration, string applicationName)
        {
            var url = configuration.GetValue<Uri?>($"Urls:{applicationName}");

            return url ?? throw new Exception($"Configuration key \"Urls:{applicationName}\" is not set");
        }
    }
}