using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: DefaultIntentManaged(Mode.Merge, Signature = Mode.Fully, Attributes = Mode.Merge, Comments = Mode.Merge, Targets = Targets.Constructors | Targets.Methods)]
[assembly: IntentTemplate("Intent.Services.Guardrails.Contracts.ServiceProxyContract", Version = "1.0")]

namespace eShop.WebhookClient.Services;

public class WebhooksClient(HttpClient client)
{
    public Task<HttpResponseMessage> AddWebHookAsync(WebhookSubscriptionRequest payload)
    {
        return client.PostAsJsonAsync("/api/webhooks", payload);
    }

    public async Task<IEnumerable<WebhookResponse>> LoadWebhooks()
    {
        return await client.GetFromJsonAsync<IEnumerable<WebhookResponse>>("/api/webhooks") ?? [];
    }
}
