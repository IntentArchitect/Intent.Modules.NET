// ------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version: 16.0.0.0
//  
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
// ------------------------------------------------------------------------------
namespace Intent.Modules.Application.AutoMapper.Templates.MappingProfile
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
    
    #line 1 "C:\Dev\Intent.Modules.NET\Modules\Intent.Modules.Application.AutoMapper\Templates\MappingProfile\MappingProfileTemplate.tt"
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.TextTemplating", "16.0.0.0")]
    public partial class MappingProfileTemplate : CSharpTemplateBase<object>
    {
#line hidden
        /// <summary>
        /// Create the template output
        /// </summary>
        public override string TransformText()
        {
            this.Write("using AutoMapper;\r\nusing System;\r\nusing System.Linq;\r\nusing System.Reflection;\r\n\r" +
                    "\n[assembly: DefaultIntentManaged(Mode.Fully)]\r\n\r\nnamespace ");
            
            #line 17 "C:\Dev\Intent.Modules.NET\Modules\Intent.Modules.Application.AutoMapper\Templates\MappingProfile\MappingProfileTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Namespace));
            
            #line default
            #line hidden
            this.Write("\r\n{\r\n    public class ");
            
            #line 19 "C:\Dev\Intent.Modules.NET\Modules\Intent.Modules.Application.AutoMapper\Templates\MappingProfile\MappingProfileTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(ClassName));
            
            #line default
            #line hidden
            this.Write(" : Profile\r\n    {\r\n        public  ");
            
            #line 21 "C:\Dev\Intent.Modules.NET\Modules\Intent.Modules.Application.AutoMapper\Templates\MappingProfile\MappingProfileTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(ClassName));
            
            #line default
            #line hidden
            this.Write(@"()
        {
            ApplyMappingsFromAssembly(Assembly.GetExecutingAssembly());
        }

        private void ApplyMappingsFromAssembly(Assembly assembly)
        {
            var types = assembly.GetExportedTypes()
                .Where(t => t.GetInterfaces().Any(i => 
                    i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IMapFrom<>)))
                .ToList();

            foreach (var type in types)
            {
                var instance = Activator.CreateInstance(type);

                var methodInfo = type.GetMethod(""Mapping"") 
                    ?? type.GetInterface(""IMapFrom`1"").GetMethod(""Mapping"");
                
                methodInfo?.Invoke(instance, new object[] { this });
            }
        }
    }
}");
            return this.GenerationEnvironment.ToString();
        }
    }
    
    #line default
    #line hidden
}
