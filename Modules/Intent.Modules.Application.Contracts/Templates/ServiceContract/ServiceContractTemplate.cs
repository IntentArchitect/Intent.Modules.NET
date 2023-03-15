// ------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version: 17.0.0.0
//  
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
// ------------------------------------------------------------------------------
namespace Intent.Modules.Application.Contracts.Templates.ServiceContract
{
    using Intent.Modules.Application.Dtos.Templates;
    using Intent.Modules.Common.Templates;
    using Intent.Modules.Common.CSharp.Templates;
    using Intent.Modelers.Services.Api;
    using System;
    using System.IO;
    using System.Diagnostics;
    using System.Linq;
    using System.Collections;
    using System.Collections.Generic;
    
    /// <summary>
    /// Class to produce the template output
    /// </summary>
    
    #line 1 "C:\Dev\Intent.Modules.NET\Modules\Intent.Modules.Application.Contracts\Templates\ServiceContract\ServiceContractTemplate.tt"
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.TextTemplating", "17.0.0.0")]
    public partial class ServiceContractTemplate : CSharpTemplateBase<Intent.Modelers.Services.Api.ServiceModel, Intent.Modules.Application.Contracts.Templates.ServiceContract.ServiceContractDecorator>
    {
#line hidden
        /// <summary>
        /// Create the template output
        /// </summary>
        public override string TransformText()
        {
            this.Write(" \r\nusing System;\r\nusing System.Collections.Generic;\r\nusing System.Threading.Tasks" +
                    ";\r\n");
            
            #line 17 "C:\Dev\Intent.Modules.NET\Modules\Intent.Modules.Application.Contracts\Templates\ServiceContract\ServiceContractTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(DependencyUsings));
            
            #line default
            #line hidden
            this.Write("\r\n\r\n[assembly: DefaultIntentManaged(Mode.Fully)]\r\n\r\nnamespace ");
            
            #line 21 "C:\Dev\Intent.Modules.NET\Modules\Intent.Modules.Application.Contracts\Templates\ServiceContract\ServiceContractTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Namespace));
            
            #line default
            #line hidden
            this.Write("\r\n{\r\n");
            
            #line 23 "C:\Dev\Intent.Modules.NET\Modules\Intent.Modules.Application.Contracts\Templates\ServiceContract\ServiceContractTemplate.tt"
 
    foreach (var line in Model.GetXmlDocLines())
    {

            
            #line default
            #line hidden
            this.Write("    /// ");
            
            #line 27 "C:\Dev\Intent.Modules.NET\Modules\Intent.Modules.Application.Contracts\Templates\ServiceContract\ServiceContractTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(line));
            
            #line default
            #line hidden
            this.Write("\r\n");
            
            #line 28 "C:\Dev\Intent.Modules.NET\Modules\Intent.Modules.Application.Contracts\Templates\ServiceContract\ServiceContractTemplate.tt"

    }

            
            #line default
            #line hidden
            this.Write("    ");
            
            #line 31 "C:\Dev\Intent.Modules.NET\Modules\Intent.Modules.Application.Contracts\Templates\ServiceContract\ServiceContractTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(ContractAttributes()));
            
            #line default
            #line hidden
            this.Write("\r\n    public interface ");
            
            #line 32 "C:\Dev\Intent.Modules.NET\Modules\Intent.Modules.Application.Contracts\Templates\ServiceContract\ServiceContractTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(ClassName));
            
            #line default
            #line hidden
            this.Write(" : IDisposable\r\n    {");
            
            #line 33 "C:\Dev\Intent.Modules.NET\Modules\Intent.Modules.Application.Contracts\Templates\ServiceContract\ServiceContractTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(EnterClass()));
            
            #line default
            #line hidden
            this.Write("\r\n");
            
            #line 34 "C:\Dev\Intent.Modules.NET\Modules\Intent.Modules.Application.Contracts\Templates\ServiceContract\ServiceContractTemplate.tt"

    foreach (var o in Model.Operations)
    {
        foreach (var line in o.GetXmlDocLines())
        {

            
            #line default
            #line hidden
            this.Write("        /// ");
            
            #line 40 "C:\Dev\Intent.Modules.NET\Modules\Intent.Modules.Application.Contracts\Templates\ServiceContract\ServiceContractTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(line));
            
            #line default
            #line hidden
            this.Write("\r\n");
            
            #line 41 "C:\Dev\Intent.Modules.NET\Modules\Intent.Modules.Application.Contracts\Templates\ServiceContract\ServiceContractTemplate.tt"

        }

            
            #line default
            #line hidden
            this.Write("        ");
            
            #line 44 "C:\Dev\Intent.Modules.NET\Modules\Intent.Modules.Application.Contracts\Templates\ServiceContract\ServiceContractTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(OperationAttributes(o)));
            
            #line default
            #line hidden
            this.Write("\r\n        ");
            
            #line 45 "C:\Dev\Intent.Modules.NET\Modules\Intent.Modules.Application.Contracts\Templates\ServiceContract\ServiceContractTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(GetOperationReturnType(o)));
            
            #line default
            #line hidden
            this.Write(" ");
            
            #line 45 "C:\Dev\Intent.Modules.NET\Modules\Intent.Modules.Application.Contracts\Templates\ServiceContract\ServiceContractTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(o.Name.ToPascalCase()));
            
            #line default
            #line hidden
            this.Write("(");
            
            #line 45 "C:\Dev\Intent.Modules.NET\Modules\Intent.Modules.Application.Contracts\Templates\ServiceContract\ServiceContractTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(GetOperationDefinitionParameters(o)));
            
            #line default
            #line hidden
            this.Write(");\r\n");
            
            #line 46 "C:\Dev\Intent.Modules.NET\Modules\Intent.Modules.Application.Contracts\Templates\ServiceContract\ServiceContractTemplate.tt"

    }

            
            #line default
            #line hidden
            
            #line 48 "C:\Dev\Intent.Modules.NET\Modules\Intent.Modules.Application.Contracts\Templates\ServiceContract\ServiceContractTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(ExitClass()));
            
            #line default
            #line hidden
            this.Write("\r\n    }\r\n}");
            return this.GenerationEnvironment.ToString();
        }
    }
    
    #line default
    #line hidden
}
