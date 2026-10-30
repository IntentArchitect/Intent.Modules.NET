using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: DefaultIntentManaged(Mode.Merge, Signature = Mode.Fully, Attributes = Mode.Merge, Comments = Mode.Merge, Targets = Targets.Constructors | Targets.Methods)]
[assembly: IntentTemplate("Intent.Services.Guardrails.Contracts.DtoContract", Version = "1.0")]

namespace eShop.WebhookClient.Services;

public class WebHookReceived
{
    public DateTime When { get; set; }

    public string? Data { get; set; }

    public string? Token { get; set; }
}
