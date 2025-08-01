using System.Diagnostics.CodeAnalysis;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.Contracts.AssemblyAttributes", Version = "1.0")]

[assembly: SuppressMessage("Formatting", "IDE0130:Namespace does not match folder structure.", Target = "AzureFunctions.AzureEventGrid.EventDomain", Scope = "namespaceanddescendants", Justification = "Message namespaces need to be consistent between applications for deserialization to work")]
[assembly: SuppressMessage("Formatting", "IDE0130:Namespace does not match folder structure.", Target = "AzureFunctions.AzureEventGrid.GroupA.Eventing.Messages", Scope = "namespaceanddescendants", Justification = "Message namespaces need to be consistent between applications for deserialization to work")]
[assembly: SuppressMessage("Formatting", "IDE0130:Namespace does not match folder structure.", Target = "AzureFunctions.AzureEventGrid.GroupB.Eventing.Messages", Scope = "namespaceanddescendants", Justification = "Message namespaces need to be consistent between applications for deserialization to work")]
