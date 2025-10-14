using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpStringInterpolationTemplate", Version = "1.0")]

namespace Intent.Modules.Aws.Sqs.Templates.SqsPublisherOptions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class SqsPublisherOptionsTemplate
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
                             
                             public void AddQueue<TMessage>(string queueUrl)
                             {
                                 ArgumentNullException.ThrowIfNull(queueUrl);
                                 _entries.Add(new PublisherEntry(typeof(TMessage), queueUrl));
                             }
                         }
                         
                         public record PublisherEntry(Type MessageType, string QueueUrl);
                     }
                     """;
        }
    }
}
