using System.Diagnostics.CodeAnalysis;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.Contracts.AssemblyAttributes", Version = "1.0")]

[assembly: SuppressMessage("Formatting", "IDE0130:Namespace does not match folder structure.", Target = "MassTransit.Messages.Shared", Scope = "namespaceanddescendants", Justification = "Message namespaces need to be consistent between applications for deserialization to work")]
[assembly: SuppressMessage("Formatting", "IDE0130:Namespace does not match folder structure.", Target = "Publish.AspNetCore.MassTransit.OutBoxEF.TestApplication.Domain", Scope = "namespaceanddescendants", Justification = "Message namespaces need to be consistent between applications for deserialization to work")]
