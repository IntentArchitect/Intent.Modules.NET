using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.AzureEventGrid.PublisherOptions", Version = "1.0")]

namespace AzureFunctions.AzureEventGrid.Infrastructure.Configuration
{
    public class PublisherOptions
    {
        private readonly List<PublisherEntry> _entries = [];

        public IReadOnlyList<PublisherEntry> Entries => _entries;

        public void Add<TMessage>(string credentialKey, string endpoint)
        {
            ArgumentNullException.ThrowIfNull(credentialKey);
            ArgumentNullException.ThrowIfNull(endpoint);
            _entries.Add(new PublisherEntry(typeof(TMessage), credentialKey, endpoint));
        }
    }

    public record PublisherEntry(Type MessageType, string CredentialKey, string Endpoint);
}