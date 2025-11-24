using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.AzureEventGrid.AzureEventGridPublisherOptions", Version = "1.0")]

namespace AzureFunctions.AzureEventGrid.Infrastructure.Configuration
{
    public class AzureEventGridPublisherOptions
    {
        private readonly List<AzureEventGridPublisherEntry> _entries = [];

        public IReadOnlyList<AzureEventGridPublisherEntry> Entries => _entries;

        public void AddTopic<TMessage>(string credentialKey, string endpoint, string source)
        {
            ArgumentNullException.ThrowIfNull(credentialKey);
            ArgumentNullException.ThrowIfNull(endpoint);
            ArgumentNullException.ThrowIfNull(source);
            _entries.Add(new AzureEventGridPublisherEntry(typeof(TMessage), credentialKey, endpoint, source));
        }
    }

    public record AzureEventGridPublisherEntry(Type MessageType, string CredentialKey, string Endpoint, string Source);
}