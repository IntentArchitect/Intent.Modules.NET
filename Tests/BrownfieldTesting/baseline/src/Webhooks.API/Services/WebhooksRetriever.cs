using System.Collections.Generic;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: DefaultIntentManaged(Mode.Merge, Signature = Mode.Fully, Attributes = Mode.Merge, Comments = Mode.Merge, Targets = Targets.Constructors | Targets.Methods)]
[assembly: IntentTemplate("Intent.Services.Guardrails.Contracts.ServiceContract", Version = "1.0")]

namespace Webhooks.API.Services;

public class WebhooksRetriever(WebhooksContext db) : IWebhooksRetriever
{
    public async Task<IEnumerable<WebhookSubscription>> GetSubscriptionsOfType(WebhookType type)
    {
        return await db.Subscriptions.Where(s => s.Type == type).ToListAsync();
    }
}
