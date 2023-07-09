// ------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version: 17.0.0.0
//  
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
// ------------------------------------------------------------------------------
namespace Intent.Modules.Blazor.HttpClients.Templates.HttpClientConfiguration
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
    
    #line 1 "C:\Dev\Intent.Modules.NET\Modules\Intent.Modules.Blazor.HttpClients\Templates\HttpClientConfiguration\HttpClientConfigurationTemplate.tt"
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.TextTemplating", "17.0.0.0")]
    public partial class HttpClientConfigurationTemplate : CSharpTemplateBase<IList<Intent.Modelers.Types.ServiceProxies.Api.ServiceProxyModel>>
    {
#line hidden
        /// <summary>
        /// Create the template output
        /// </summary>
        public override string TransformText()
        {
            this.Write("using Microsoft.Extensions.Configuration;\r\nusing Microsoft.Extensions.DependencyI" +
                    "njection;\r\n\r\n[assembly: DefaultIntentManaged(Mode.Fully)]\r\n\r\nnamespace ");
            
            #line 15 "C:\Dev\Intent.Modules.NET\Modules\Intent.Modules.Blazor.HttpClients\Templates\HttpClientConfiguration\HttpClientConfigurationTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Namespace));
            
            #line default
            #line hidden
            this.Write("\r\n{\r\n    public static class ");
            
            #line 17 "C:\Dev\Intent.Modules.NET\Modules\Intent.Modules.Blazor.HttpClients\Templates\HttpClientConfiguration\HttpClientConfigurationTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(ClassName));
            
            #line default
            #line hidden
            this.Write("\r\n    {\r\n        public static void AddHttpClients(this IServiceCollection servic" +
                    "es, IConfiguration configuration)\r\n        {\r\n");
            
            #line 21 "C:\Dev\Intent.Modules.NET\Modules\Intent.Modules.Blazor.HttpClients\Templates\HttpClientConfiguration\HttpClientConfigurationTemplate.tt"

    foreach (var proxy in Model)
    {

            
            #line default
            #line hidden
            this.Write("            services.AddHttpClient<");
            
            #line 25 "C:\Dev\Intent.Modules.NET\Modules\Intent.Modules.Blazor.HttpClients\Templates\HttpClientConfiguration\HttpClientConfigurationTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(this.GetServiceContractName(proxy)));
            
            #line default
            #line hidden
            this.Write(", ");
            
            #line 25 "C:\Dev\Intent.Modules.NET\Modules\Intent.Modules.Blazor.HttpClients\Templates\HttpClientConfiguration\HttpClientConfigurationTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(this.GetHttpClientName(proxy)));
            
            #line default
            #line hidden
            this.Write(">(http =>\r\n            {\r\n                http.BaseAddress = configuration.GetVal" +
                    "ue<Uri>(\"");
            
            #line 27 "C:\Dev\Intent.Modules.NET\Modules\Intent.Modules.Blazor.HttpClients\Templates\HttpClientConfiguration\HttpClientConfigurationTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(GetConfigKey(proxy, "Uri")));
            
            #line default
            #line hidden
            this.Write("\");\r\n                http.Timeout = configuration.GetValue<TimeSpan?>(\"");
            
            #line 28 "C:\Dev\Intent.Modules.NET\Modules\Intent.Modules.Blazor.HttpClients\Templates\HttpClientConfiguration\HttpClientConfigurationTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(GetConfigKey(proxy, "Timeout")));
            
            #line default
            #line hidden
            this.Write("\") ?? TimeSpan.FromSeconds(100);\r\n            });\r\n");
            
            #line 30 "C:\Dev\Intent.Modules.NET\Modules\Intent.Modules.Blazor.HttpClients\Templates\HttpClientConfiguration\HttpClientConfigurationTemplate.tt"
        
    }

            
            #line default
            #line hidden
            this.Write("        }\r\n    }\r\n}");
            return this.GenerationEnvironment.ToString();
        }
    }
    
    #line default
    #line hidden
}
