// ------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version: 17.0.0.0
//  
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
// ------------------------------------------------------------------------------
namespace Intent.Modules.AspNetCore.Controllers.Templates.JsonResponse
{
    using Intent.Modules.Common.CSharp.Templates;
    using System;
    
    /// <summary>
    /// Class to produce the template output
    /// </summary>
    
    #line 1 "C:\Dev\Intent.Modules.NET\Modules\Intent.Modules.AspNetCore.Controllers\Templates\JsonResponse\JsonResponseTemplate.tt"
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.TextTemplating", "17.0.0.0")]
    public partial class JsonResponseTemplate : CSharpTemplateBase<object>
    {
#line hidden
        /// <summary>
        /// Create the template output
        /// </summary>
        public override string TransformText()
        {
            this.Write("\r\n[assembly: DefaultIntentManaged(Mode.Fully)]\r\n\r\nnamespace ");
            
            #line 7 "C:\Dev\Intent.Modules.NET\Modules\Intent.Modules.AspNetCore.Controllers\Templates\JsonResponse\JsonResponseTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Namespace));
            
            #line default
            #line hidden
            this.Write("\r\n{\r\n    /// <summary>\r\n    /// Implicit wrapping of types that serialize to non-" +
                    "complex values.\r\n    /// </summary>\r\n    /// <typeparam name=\"T\">Types such as s" +
                    "tring, Guid, int, long, etc.</typeparam>\r\n    public class ");
            
            #line 13 "C:\Dev\Intent.Modules.NET\Modules\Intent.Modules.AspNetCore.Controllers\Templates\JsonResponse\JsonResponseTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(ClassName));
            
            #line default
            #line hidden
            this.Write("<T>\r\n    {\r\n        public ");
            
            #line 15 "C:\Dev\Intent.Modules.NET\Modules\Intent.Modules.AspNetCore.Controllers\Templates\JsonResponse\JsonResponseTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(ClassName));
            
            #line default
            #line hidden
            this.Write("(T value)\r\n        {\r\n            Value = value;\r\n        }\r\n\r\n        public T V" +
                    "alue { get; set; }\r\n    }\r\n}");
            return this.GenerationEnvironment.ToString();
        }
    }
    
    #line default
    #line hidden
}
