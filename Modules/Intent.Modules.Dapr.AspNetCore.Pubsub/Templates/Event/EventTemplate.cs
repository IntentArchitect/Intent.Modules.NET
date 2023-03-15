// ------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version: 17.0.0.0
//  
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
// ------------------------------------------------------------------------------
namespace Intent.Modules.Dapr.AspNetCore.Pubsub.Templates.Event
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
    
    #line 1 "C:\Dev\Intent.Modules.NET\Modules\Intent.Modules.Dapr.AspNetCore.Pubsub\Templates\Event\EventTemplate.tt"
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.TextTemplating", "17.0.0.0")]
    public partial class EventTemplate : CSharpTemplateBase<Intent.Modelers.Eventing.Api.MessageModel>
    {
#line hidden
        /// <summary>
        /// Create the template output
        /// </summary>
        public override string TransformText()
        {
            this.Write("[assembly: DefaultIntentManaged(Mode.Fully)]\r\n\r\nnamespace ");
            
            #line 12 "C:\Dev\Intent.Modules.NET\Modules\Intent.Modules.Dapr.AspNetCore.Pubsub\Templates\Event\EventTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Namespace));
            
            #line default
            #line hidden
            this.Write("\r\n{\r\n    public class ");
            
            #line 14 "C:\Dev\Intent.Modules.NET\Modules\Intent.Modules.Dapr.AspNetCore.Pubsub\Templates\Event\EventTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(ClassName));
            
            #line default
            #line hidden
            this.Write(" : ");
            
            #line 14 "C:\Dev\Intent.Modules.NET\Modules\Intent.Modules.Dapr.AspNetCore.Pubsub\Templates\Event\EventTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(this.GetEventInterfaceName()));
            
            #line default
            #line hidden
            this.Write("\r\n    {\r\n        public const string PubsubName = ");
            
            #line 16 "C:\Dev\Intent.Modules.NET\Modules\Intent.Modules.Dapr.AspNetCore.Pubsub\Templates\Event\EventTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(PubsubName()));
            
            #line default
            #line hidden
            this.Write(";\r\n        public const string TopicName = ");
            
            #line 17 "C:\Dev\Intent.Modules.NET\Modules\Intent.Modules.Dapr.AspNetCore.Pubsub\Templates\Event\EventTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(TopicName()));
            
            #line default
            #line hidden
            this.Write(";\r\n\r\n");
            
            #line 19 "C:\Dev\Intent.Modules.NET\Modules\Intent.Modules.Dapr.AspNetCore.Pubsub\Templates\Event\EventTemplate.tt"
 foreach (var property in Model.Properties) { 
            
            #line default
            #line hidden
            this.Write("        public ");
            
            #line 20 "C:\Dev\Intent.Modules.NET\Modules\Intent.Modules.Dapr.AspNetCore.Pubsub\Templates\Event\EventTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(GetTypeName(property)));
            
            #line default
            #line hidden
            this.Write(" ");
            
            #line 20 "C:\Dev\Intent.Modules.NET\Modules\Intent.Modules.Dapr.AspNetCore.Pubsub\Templates\Event\EventTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(property.Name.ToPascalCase()));
            
            #line default
            #line hidden
            this.Write(" { get; set; }\r\n\r\n");
            
            #line 22 "C:\Dev\Intent.Modules.NET\Modules\Intent.Modules.Dapr.AspNetCore.Pubsub\Templates\Event\EventTemplate.tt"
 }
            
            #line default
            #line hidden
            this.Write("        string ");
            
            #line 23 "C:\Dev\Intent.Modules.NET\Modules\Intent.Modules.Dapr.AspNetCore.Pubsub\Templates\Event\EventTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(this.GetEventInterfaceName()));
            
            #line default
            #line hidden
            this.Write(".PubsubName => PubsubName;\r\n\r\n        string ");
            
            #line 25 "C:\Dev\Intent.Modules.NET\Modules\Intent.Modules.Dapr.AspNetCore.Pubsub\Templates\Event\EventTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(this.GetEventInterfaceName()));
            
            #line default
            #line hidden
            this.Write(".TopicName => TopicName;\r\n    }\r\n}");
            return this.GenerationEnvironment.ToString();
        }
    }
    
    #line default
    #line hidden
}
