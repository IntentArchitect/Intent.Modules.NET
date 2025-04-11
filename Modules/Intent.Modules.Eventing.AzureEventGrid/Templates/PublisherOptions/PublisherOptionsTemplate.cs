using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly:IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpStringInterpolationTemplate",Version= "1.0")]

namespace Intent.Modules.Eventing.AzureEventGrid.Templates.PublisherOptions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class PublisherOptionsTemplate
    {
        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public override string TransformText()
        {
            return $$"""
                     using System;
                     using System.Collections.Generic;
                     
                     [assembly: DefaultIntentManaged(Mode.Fully)]
                     
                     namespace {{Namespace}}
                     {
                         public class {{ClassName}}
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
                     """;
        }
    }
}