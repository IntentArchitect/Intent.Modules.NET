// ------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version: 17.0.0.0
//  
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
// ------------------------------------------------------------------------------
namespace Intent.Modules.MediatR.DomainEvents.Templates.AggregateManager
{
    using System.Collections.Generic;
    using System.Linq;
    using Intent.Modules.Common;
    using Intent.Modules.Common.Templates;
    using Intent.Modules.Common.CSharp.Templates;
    using Intent.Templates;
    using Intent.Metadata.Models;
    using Intent.Modules.DomainEvents.Templates;
    using System;
    
    /// <summary>
    /// Class to produce the template output
    /// </summary>
    
    #line 1 "C:\Dev\Intent.Modules.NET\Modules\Intent.Modules.MediatR.DomainEvents\Templates\AggregateManager\AggregateManagerTemplate.tt"
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.TextTemplating", "17.0.0.0")]
    public partial class AggregateManagerTemplate : CSharpTemplateBase<Intent.Modelers.Domain.Api.ClassModel>
    {
#line hidden
        /// <summary>
        /// Create the template output
        /// </summary>
        public override string TransformText()
        {
            this.Write("using MediatR;\r\nusing System;\r\nusing System.Threading;\r\nusing System.Threading.Ta" +
                    "sks;\r\n\r\n[assembly: DefaultIntentManaged(Mode.Merge)]\r\n\r\nnamespace ");
            
            #line 18 "C:\Dev\Intent.Modules.NET\Modules\Intent.Modules.MediatR.DomainEvents\Templates\AggregateManager\AggregateManagerTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Namespace));
            
            #line default
            #line hidden
            this.Write("\r\n{\r\n    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]\r\n    public class ");
            
            #line 21 "C:\Dev\Intent.Modules.NET\Modules\Intent.Modules.MediatR.DomainEvents\Templates\AggregateManager\AggregateManagerTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(ClassName));
            
            #line default
            #line hidden
            this.Write(" : ");
            
            #line 21 "C:\Dev\Intent.Modules.NET\Modules\Intent.Modules.MediatR.DomainEvents\Templates\AggregateManager\AggregateManagerTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(GetInterfaces()));
            
            #line default
            #line hidden
            this.Write("\r\n    {\r\n        [IntentInitialGen]\r\n        public ");
            
            #line 24 "C:\Dev\Intent.Modules.NET\Modules\Intent.Modules.MediatR.DomainEvents\Templates\AggregateManager\AggregateManagerTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(ClassName));
            
            #line default
            #line hidden
            this.Write("()\r\n        {\r\n        }\r\n");
            
            #line 27 "C:\Dev\Intent.Modules.NET\Modules\Intent.Modules.MediatR.DomainEvents\Templates\AggregateManager\AggregateManagerTemplate.tt"
  foreach(var domainEvent in GetDomainEventModels()) {
            
            #line default
            #line hidden
            this.Write("\r\n        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]\r\n        public async T" +
                    "ask Handle(");
            
            #line 30 "C:\Dev\Intent.Modules.NET\Modules\Intent.Modules.MediatR.DomainEvents\Templates\AggregateManager\AggregateManagerTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(this.GetDomainEventNotificationName()));
            
            #line default
            #line hidden
            this.Write("<");
            
            #line 30 "C:\Dev\Intent.Modules.NET\Modules\Intent.Modules.MediatR.DomainEvents\Templates\AggregateManager\AggregateManagerTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(this.GetDomainEventName(domainEvent)));
            
            #line default
            #line hidden
            this.Write("> notification, CancellationToken cancellationToken)\r\n        {\r\n            thro" +
                    "w new NotImplementedException(\"Implement your handler logic here...\");\r\n        " +
                    "}\r\n");
            
            #line 34 "C:\Dev\Intent.Modules.NET\Modules\Intent.Modules.MediatR.DomainEvents\Templates\AggregateManager\AggregateManagerTemplate.tt"
  }
            
            #line default
            #line hidden
            this.Write("    }\r\n}");
            return this.GenerationEnvironment.ToString();
        }
    }
    
    #line default
    #line hidden
}
