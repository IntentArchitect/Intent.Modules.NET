using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.GoogleCloud.PubSub.MessageTemplates.GenericMessage", Version = "1.0")]

namespace Publish.AspNetCore.GooglePubSub.TestApplication.Application.IntegrationEvents;

public record GenericMessage(string MessageId, IDictionary<string, string> Attributes, string MessageBody);