using System;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: DefaultIntentManaged(Mode.Merge, Signature = Mode.Fully, Attributes = Mode.Merge, Comments = Mode.Merge, Targets = Targets.Constructors | Targets.Methods)]
[assembly: IntentTemplate("Intent.Services.Guardrails.Contracts.DtoContract", Version = "1.0")]

namespace Webhooks.API.Model;

public class WebhookData
{
    public DateTime When { get; }

    public string Payload { get; }

    public string Type { get; }

    public WebhookData(WebhookType hookType, object data)
    {
        When = DateTime.UtcNow;
        Type = hookType.ToString();
        Payload = JsonSerializer.Serialize(data);
    }
}
