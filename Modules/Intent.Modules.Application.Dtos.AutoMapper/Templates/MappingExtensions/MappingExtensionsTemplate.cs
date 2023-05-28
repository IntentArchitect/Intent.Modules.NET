// ------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version: 17.0.0.0
//  
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
// ------------------------------------------------------------------------------
namespace Intent.Modules.Application.Dtos.AutoMapper.Templates.MappingExtensions
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
    
    #line 1 "C:\Dev\Intent.Modules.NET\Modules\Intent.Modules.Application.Dtos.AutoMapper\Templates\MappingExtensions\MappingExtensionsTemplate.tt"
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.TextTemplating", "17.0.0.0")]
    public partial class MappingExtensionsTemplate : CSharpTemplateBase<Intent.Modelers.Services.Api.DTOModel>
    {
#line hidden
        /// <summary>
        /// Create the template output
        /// </summary>
        public override string TransformText()
        {
            this.Write("using AutoMapper;\r\nusing System.Collections.Generic;\r\nusing System.Linq;\r\n\r\n[asse" +
                    "mbly: DefaultIntentManaged(Mode.Fully)]\r\n\r\nnamespace ");
            
            #line 16 "C:\Dev\Intent.Modules.NET\Modules\Intent.Modules.Application.Dtos.AutoMapper\Templates\MappingExtensions\MappingExtensionsTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Namespace));
            
            #line default
            #line hidden
            this.Write("\r\n{\r\n    public static class ");
            
            #line 18 "C:\Dev\Intent.Modules.NET\Modules\Intent.Modules.Application.Dtos.AutoMapper\Templates\MappingExtensions\MappingExtensionsTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(ClassName));
            
            #line default
            #line hidden
            this.Write(" \r\n    {\r\n        public static ");
            
            #line 20 "C:\Dev\Intent.Modules.NET\Modules\Intent.Modules.Application.Dtos.AutoMapper\Templates\MappingExtensions\MappingExtensionsTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(GetDtoModelName()));
            
            #line default
            #line hidden
            this.Write(" MapTo");
            
            #line 20 "C:\Dev\Intent.Modules.NET\Modules\Intent.Modules.Application.Dtos.AutoMapper\Templates\MappingExtensions\MappingExtensionsTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(GetDtoModelName()));
            
            #line default
            #line hidden
            this.Write("(this ");
            
            #line 20 "C:\Dev\Intent.Modules.NET\Modules\Intent.Modules.Application.Dtos.AutoMapper\Templates\MappingExtensions\MappingExtensionsTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(GetEntityName()));
            
            #line default
            #line hidden
            this.Write(" projectFrom, IMapper mapper)\r\n            => mapper.Map<");
            
            #line 21 "C:\Dev\Intent.Modules.NET\Modules\Intent.Modules.Application.Dtos.AutoMapper\Templates\MappingExtensions\MappingExtensionsTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(GetDtoModelName()));
            
            #line default
            #line hidden
            this.Write(">(projectFrom);\r\n\r\n        public static List<");
            
            #line 23 "C:\Dev\Intent.Modules.NET\Modules\Intent.Modules.Application.Dtos.AutoMapper\Templates\MappingExtensions\MappingExtensionsTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(GetDtoModelName()));
            
            #line default
            #line hidden
            this.Write("> MapTo");
            
            #line 23 "C:\Dev\Intent.Modules.NET\Modules\Intent.Modules.Application.Dtos.AutoMapper\Templates\MappingExtensions\MappingExtensionsTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(GetDtoModelName()));
            
            #line default
            #line hidden
            this.Write("List(this IEnumerable<");
            
            #line 23 "C:\Dev\Intent.Modules.NET\Modules\Intent.Modules.Application.Dtos.AutoMapper\Templates\MappingExtensions\MappingExtensionsTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(GetEntityName()));
            
            #line default
            #line hidden
            this.Write("> projectFrom, IMapper mapper)\r\n            => projectFrom.Select(x => x.MapTo");
            
            #line 24 "C:\Dev\Intent.Modules.NET\Modules\Intent.Modules.Application.Dtos.AutoMapper\Templates\MappingExtensions\MappingExtensionsTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(GetDtoModelName()));
            
            #line default
            #line hidden
            this.Write("(mapper)).ToList();\r\n    }\r\n}");
            return this.GenerationEnvironment.ToString();
        }
    }
    
    #line default
    #line hidden
}
