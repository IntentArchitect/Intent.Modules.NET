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

        public void AddDomain(string credentialKey, string endpoint, Action<DomainOptions>? domainAction)
        {
            ArgumentNullException.ThrowIfNull(credentialKey);
            ArgumentNullException.ThrowIfNull(endpoint);

            var domainOptions = new DomainOptions(credentialKey, endpoint);
            domainAction?.Invoke(domainOptions);

            _entries.AddRange(domainOptions.ToEntries());
        }
    }

    public class DomainOptions
    {
        private readonly string _credentialKey;
        private readonly string _endpoint;
        private readonly List<AzureEventGridPublisherEntry> _entries = [];

        public DomainOptions(string credentialKey, string endpoint)
        {
            _credentialKey = credentialKey;
            _endpoint = endpoint;
        }

        public void Add<TMessage>(string source)
        {
            ArgumentNullException.ThrowIfNull(source);
            _entries.Add(new AzureEventGridPublisherEntry(typeof(TMessage), _credentialKey, _endpoint, source));
        }

        public IEnumerable<AzureEventGridPublisherEntry> ToEntries()
        {
            return _entries;
        }
    }

    public record AzureEventGridPublisherEntry(Type MessageType, string CredentialKey, string Endpoint, string Source);
}