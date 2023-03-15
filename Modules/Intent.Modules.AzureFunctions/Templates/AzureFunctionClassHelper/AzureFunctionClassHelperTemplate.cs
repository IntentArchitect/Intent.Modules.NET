// ------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version: 17.0.0.0
//  
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
// ------------------------------------------------------------------------------
namespace Intent.Modules.AzureFunctions.Templates.AzureFunctionClassHelper
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
    
    #line 1 "C:\Dev\Intent.Modules.NET\Modules\Intent.Modules.AzureFunctions\Templates\AzureFunctionClassHelper\AzureFunctionClassHelperTemplate.tt"
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.TextTemplating", "17.0.0.0")]
    public partial class AzureFunctionClassHelperTemplate : CSharpTemplateBase<object>
    {
#line hidden
        /// <summary>
        /// Create the template output
        /// </summary>
        public override string TransformText()
        {
            this.Write("using System;\r\nusing Microsoft.AspNetCore.Http;\r\n\r\n[assembly: DefaultIntentManage" +
                    "d(Mode.Fully)]\r\n\r\nnamespace ");
            
            #line 15 "C:\Dev\Intent.Modules.NET\Modules\Intent.Modules.AzureFunctions\Templates\AzureFunctionClassHelper\AzureFunctionClassHelperTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Namespace));
            
            #line default
            #line hidden
            this.Write("\r\n{\r\n    static class ");
            
            #line 17 "C:\Dev\Intent.Modules.NET\Modules\Intent.Modules.AzureFunctions\Templates\AzureFunctionClassHelper\AzureFunctionClassHelperTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(ClassName));
            
            #line default
            #line hidden
            this.Write(@"
    {
        public static T GetQueryParam<T>(string paramName, IQueryCollection query, ParseDelegate<T> parse)
            where T : struct
        {
            var strVal = query[paramName];
            if (string.IsNullOrEmpty(strVal) || !parse(strVal, out T parsed))
            {
                throw new FormatException($""Parameter '{paramName}' could not be parsed as a {typeof(T).Name}."");
            }

            return parsed;
        }
        
        public static T? GetQueryParamNullable<T>(string paramName, IQueryCollection query, ParseDelegate<T> parse)
            where T : struct
        {
            var strVal = query[paramName];
            if (string.IsNullOrEmpty(strVal))
            {
                return null;
            }

            if (!parse(strVal, out T parsed))
            {
                throw new FormatException($""Parameter '{paramName}' could not be parsed as a {typeof(T).Name}."");
            }

            return parsed;
        }
        
        public delegate bool ParseDelegate<T>(string strVal, out T parsed);
    }
}");
            return this.GenerationEnvironment.ToString();
        }
    }
    
    #line default
    #line hidden
}
