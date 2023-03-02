// ------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version: 16.0.0.0
//  
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
// ------------------------------------------------------------------------------
namespace Intent.Modules.AspNetCore.MultiTenancy.Templates.Swagger.TenantHeaderOperationFilter
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
    
    #line 1 "C:\Dev\Intent.Modules.NET\Modules\Intent.Modules.AspNetCore.MultiTenancy\Templates\Swagger\TenantHeaderOperationFilter\TenantHeaderOperationFilterTemplate.tt"
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.TextTemplating", "16.0.0.0")]
    public partial class TenantHeaderOperationFilterTemplate : CSharpTemplateBase<object>
    {
#line hidden
        /// <summary>
        /// Create the template output
        /// </summary>
        public override string TransformText()
        {
            this.Write("using System.Collections.Generic;\r\nusing Microsoft.OpenApi.Models;\r\nusing Swashbuckle.AspNetCore.SwaggerGen;\r\n\r\n[assembly: DefaultIntentManaged(Mode.Fully)]\r\n\r\nnamespace ");
            
            #line 16 "C:\Dev\Intent.Modules.NET\Modules\Intent.Modules.AspNetCore.MultiTenancy\Templates\Swagger\TenantHeaderOperationFilter\TenantHeaderOperationFilterTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Namespace));
            
            #line default
            #line hidden
            this.Write("\r\n{\r\n    public class ");
            
            #line 18 "C:\Dev\Intent.Modules.NET\Modules\Intent.Modules.AspNetCore.MultiTenancy\Templates\Swagger\TenantHeaderOperationFilter\TenantHeaderOperationFilterTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(ClassName));
            
            #line default
            #line hidden
            this.Write(" : IOperationFilter\r\n    {\r\n        public void Apply(OpenApiOperation operation, OperationFilterContext context)\r\n        {\r\n            if (operation.Parameters == null)\r\n            {\r\n                operation.Parameters = new List<OpenApiParameter>();\r\n            }\r\n\r\n            operation.Parameters.Add(new OpenApiParameter \r\n            {\r\n                Name = \"X-Tenant-Identifier\",\r\n                In = ParameterLocation.Header,\r\n                Description = \"Tenant Id\",\r\n                Required = true\r\n            });\r\n        }\r\n    }\r\n}");
            return this.GenerationEnvironment.ToString();
        }
    }
    
}
