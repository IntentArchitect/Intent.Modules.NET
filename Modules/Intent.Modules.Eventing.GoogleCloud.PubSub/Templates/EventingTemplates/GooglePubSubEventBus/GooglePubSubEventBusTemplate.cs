//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:7.0.3
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Intent.Modules.Eventing.GoogleCloud.PubSub.Templates.EventingTemplates.GooglePubSubEventBus {
    using System.Collections.Generic;
    using System.Linq;
    using Intent.Modules.Common;
    using Intent.Modules.Common.Templates;
    using Intent.Modules.Common.CSharp.Templates;
    using Intent.Templates;
    using Intent.Metadata.Models;
    using Intent.Modules.Eventing.Contracts.Templates;
    using System;
    
    
    public partial class GooglePubSubEventBusTemplate : CSharpTemplateBase<object> {
        
        public override string TransformText() {
            this.GenerationEnvironment = null;
            
            #line 11 ""
            this.Write(@"using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Google.Cloud.PubSub.V1;
using Google.Protobuf;
using Microsoft.Extensions.Options;

[assembly: DefaultIntentManaged(Mode.Fully)]

namespace ");
            
            #line default
            #line hidden
            
            #line 23 ""
            this.Write(this.ToStringHelper.ToStringWithCulture( Namespace ));
            
            #line default
            #line hidden
            
            #line 23 ""
            this.Write(";\r\n\r\npublic class ");
            
            #line default
            #line hidden
            
            #line 25 ""
            this.Write(this.ToStringHelper.ToStringWithCulture( ClassName ));
            
            #line default
            #line hidden
            
            #line 25 ""
            this.Write(" : ");
            
            #line default
            #line hidden
            
            #line 25 ""
            this.Write(this.ToStringHelper.ToStringWithCulture( this.GetEventBusInterfaceName() ));
            
            #line default
            #line hidden
            
            #line 25 ""
            this.Write("\r\n{\r\n    private readonly ");
            
            #line default
            #line hidden
            
            #line 27 ""
            this.Write(this.ToStringHelper.ToStringWithCulture( this.GetEventBusTopicEventManagerInterfaceName() ));
            
            #line default
            #line hidden
            
            #line 27 ""
            this.Write(" _topicEventManager;\r\n\r\n    private readonly List<PubsubMessage> _messagesToPubli" +
                    "sh = new();\r\n    private readonly ");
            
            #line default
            #line hidden
            
            #line 30 ""
            this.Write(this.ToStringHelper.ToStringWithCulture( this.GetPubSubOptionsName() ));
            
            #line default
            #line hidden
            
            #line 30 ""
            this.Write(" _pubSubOptions;\r\n\r\n    public GooglePubSubEventBus(");
            
            #line default
            #line hidden
            
            #line 32 ""
            this.Write(this.ToStringHelper.ToStringWithCulture( this.GetEventBusTopicEventManagerInterfaceName() ));
            
            #line default
            #line hidden
            
            #line 32 ""
            this.Write(" topicEventManager, IOptions<");
            
            #line default
            #line hidden
            
            #line 32 ""
            this.Write(this.ToStringHelper.ToStringWithCulture( this.GetPubSubOptionsName() ));
            
            #line default
            #line hidden
            
            #line 32 ""
            this.Write("> pubSubOptions)\r\n    {\r\n        _topicEventManager = topicEventManager;\r\n       " +
                    " _pubSubOptions = pubSubOptions.Value;\r\n    }\r\n    \r\n    public void Publish<T>(" +
                    "T message) where T : class\r\n    {\r\n        if (typeof(T) == typeof(");
            
            #line default
            #line hidden
            
            #line 40 ""
            this.Write(this.ToStringHelper.ToStringWithCulture( this.GetGenericMessageName() ));
            
            #line default
            #line hidden
            
            #line 40 ""
            this.Write("))\r\n        {\r\n            throw new ArgumentException($\"{nameof(");
            
            #line default
            #line hidden
            
            #line 42 ""
            this.Write(this.ToStringHelper.ToStringWithCulture( this.GetGenericMessageName() ));
            
            #line default
            #line hidden
            
            #line 42 ""
            this.Write(@")} is not meant to be published. Create a new Message type intended for your given use case."");
        }
        _messagesToPublish.Add(new PubsubMessage
        {
            Attributes = { { ""MessageType"", typeof(T).FullName } },
            Data = ByteString.CopyFromUtf8(JsonSerializer.Serialize(message))
        });
    }

    public async Task FlushAllAsync(CancellationToken cancellationToken = default)
    {
        await Task.WhenAll(_messagesToPublish.Select(async message =>
        {
            var topicName = _topicEventManager.GetTopicName(message);
            var publisher = await new PublisherClientBuilder()
            {
                TopicName = topicName,
                EmulatorDetection = _pubSubOptions.GetEmulatorDetectionMode()
            }.BuildAsync(cancellationToken);
            await publisher.PublishAsync(message);
        }));
        _messagesToPublish.Clear();
    }
}");
            
            #line default
            #line hidden
            return this.GenerationEnvironment.ToString();
        }
        
        public override void Initialize() {
            base.Initialize();
        }
    }
}
