// ------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version: 16.0.0.0
//  
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
// ------------------------------------------------------------------------------
namespace Intent.Modules.Eventing.GoogleCloud.PubSub.Templates.GoogleEventBusTopicEventManager
{
    using System.Collections.Generic;
    using System.Linq;
    using Intent.Modules.Common;
    using Intent.Modules.Common.Templates;
    using Intent.Modules.Common.CSharp.Templates;
    using Intent.Templates;
    using Intent.Metadata.Models;
    using System;
    
    /// <summary>
    /// Class to produce the template output
    /// </summary>
    
    #line 1 "C:\Dev\Intent.Modules.NET\Modules\Intent.Modules.Eventing.GoogleCloud.PubSub\Templates\GoogleEventBusTopicEventManager\GoogleEventBusTopicEventManagerTemplate.tt"
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.TextTemplating", "16.0.0.0")]
    public partial class GoogleEventBusTopicEventManagerTemplate : CSharpTemplateBase<object>
    {
#line hidden
        /// <summary>
        /// Create the template output
        /// </summary>
        public override string TransformText()
        {
            this.Write("using System;\r\nusing System.Collections.Generic;\r\nusing Google.Cloud.PubSub.V1;\r\n\r\n[assembly: DefaultIntentManaged(Mode.Fully)]\r\n\r\nnamespace ");
            
            #line 16 "C:\Dev\Intent.Modules.NET\Modules\Intent.Modules.Eventing.GoogleCloud.PubSub\Templates\GoogleEventBusTopicEventManager\GoogleEventBusTopicEventManagerTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Namespace));
            
            #line default
            #line hidden
            this.Write(";\r\n\r\npublic class ");
            
            #line 18 "C:\Dev\Intent.Modules.NET\Modules\Intent.Modules.Eventing.GoogleCloud.PubSub\Templates\GoogleEventBusTopicEventManager\GoogleEventBusTopicEventManagerTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(ClassName));
            
            #line default
            #line hidden
            this.Write(" : ");
            
            #line 18 "C:\Dev\Intent.Modules.NET\Modules\Intent.Modules.Eventing.GoogleCloud.PubSub\Templates\GoogleEventBusTopicEventManager\GoogleEventBusTopicEventManagerTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(this.GetEventBusTopicEventManagerInterfaceName()));
            
            #line default
            #line hidden
            this.Write("\r\n{\r\n    private readonly ");
            
            #line 20 "C:\Dev\Intent.Modules.NET\Modules\Intent.Modules.Eventing.GoogleCloud.PubSub\Templates\GoogleEventBusTopicEventManager\GoogleEventBusTopicEventManagerTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(this.GetCloudResourceManagerInterfaceName()));
            
            #line default
            #line hidden
            this.Write(" _resourceManager;\r\n    private readonly Dictionary<string, string> _topicLookup;\r\n\r\n    public ");
            
            #line 23 "C:\Dev\Intent.Modules.NET\Modules\Intent.Modules.Eventing.GoogleCloud.PubSub\Templates\GoogleEventBusTopicEventManager\GoogleEventBusTopicEventManagerTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(ClassName));
            
            #line default
            #line hidden
            this.Write("(");
            
            #line 23 "C:\Dev\Intent.Modules.NET\Modules\Intent.Modules.Eventing.GoogleCloud.PubSub\Templates\GoogleEventBusTopicEventManager\GoogleEventBusTopicEventManagerTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(this.GetCloudResourceManagerInterfaceName()));
            
            #line default
            #line hidden
            this.Write(" resourceManager)\r\n    {\r\n        _resourceManager = resourceManager;\r\n        _topicLookup = new Dictionary<string, string>();\r\n    }\r\n    \r\n    public void RegisterTopicEvent<TMessage>(string topicId) where TMessage : class\r\n    {\r\n        _topicLookup.Add(typeof(TMessage).FullName!, topicId);\r\n    }\r\n    \r\n    public TopicName GetTopicName(PubsubMessage message)\r\n    {\r\n        var messageType = message.Attributes[\"MessageType\"]!;\r\n        if (!_topicLookup.TryGetValue(messageType, out var topicId))\r\n        {\r\n            throw new InvalidOperationException($\"Could not find a Topic Id for Message Type: {messageType}\");\r\n        }\r\n        return new TopicName(_resourceManager.ProjectId, topicId);\r\n    }\r\n}");
            return this.GenerationEnvironment.ToString();
        }
    }
    
}
