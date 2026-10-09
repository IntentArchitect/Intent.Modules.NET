using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage("Formatting", "IDE0130:Namespace does not match folder structure.", Target = "CqrsSpike.Publisher.Eventing.Messages", Scope = "namespaceanddescendants", Justification = "Message namespaces need to be consistent between applications for deserialization to work")]
[assembly: SuppressMessage("Formatting", "IDE0130:Namespace does not match folder structure.", Target = "CqrsSpike.Subscriber.Eventing.Messages", Scope = "namespaceanddescendants", Justification = "Message namespaces need to be consistent between applications for deserialization to work")]
