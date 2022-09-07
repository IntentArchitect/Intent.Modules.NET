﻿// ------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version: 16.0.0.0
//  
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
// ------------------------------------------------------------------------------
namespace Intent.Modules.ServiceProxies.Templates.ServiceProxyClient
{
    using Intent.Modules.Common.CSharp.Templates;
    using Intent.Modules.Application.Contracts.Clients.Templates;
    using Intent.Modules.Common.Templates;
    using System;
    
    /// <summary>
    /// Class to produce the template output
    /// </summary>
    
    #line 1 "C:\Dev\Intent.Modules.NET_Proxy\Modules\Intent.Modules.ServiceProxies\Templates\ServiceProxyClient\ServiceProxyClientTemplate.tt"
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.TextTemplating", "16.0.0.0")]
    public partial class ServiceProxyClientTemplate : CSharpTemplateBase<Intent.Modelers.Types.ServiceProxies.Api.ServiceProxyModel>
    {
#line hidden
        /// <summary>
        /// Create the template output
        /// </summary>
        public override string TransformText()
        {
            this.Write("using System;\r\nusing System.Collections.Generic;\r\nusing System.Net;\r\nusing System.Net.Http;\r\nusing System.Net.Http.Headers;\r\nusing System.Text;\r\nusing System.Text.Json;\r\nusing System.Threading;\r\nusing System.Threading.Tasks;\r\nusing Microsoft.AspNetCore.WebUtilities;\r\n\r\n[assembly: DefaultIntentManaged(Mode.Fully)]\r\n\r\nnamespace ");
            
            #line 20 "C:\Dev\Intent.Modules.NET_Proxy\Modules\Intent.Modules.ServiceProxies\Templates\ServiceProxyClient\ServiceProxyClientTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Namespace));
            
            #line default
            #line hidden
            this.Write("\r\n{\r\n    public class ");
            
            #line 22 "C:\Dev\Intent.Modules.NET_Proxy\Modules\Intent.Modules.ServiceProxies\Templates\ServiceProxyClient\ServiceProxyClientTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(ClassName));
            
            #line default
            #line hidden
            this.Write(" : ");
            
            #line 22 "C:\Dev\Intent.Modules.NET_Proxy\Modules\Intent.Modules.ServiceProxies\Templates\ServiceProxyClient\ServiceProxyClientTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(this.GetServiceContractName()));
            
            #line default
            #line hidden
            this.Write("\r\n    {\r\n        private readonly HttpClient _httpClient;\r\n\r\n        private readonly JsonSerializerOptions _serializerOptions = new JsonSerializerOptions()\r\n        {\r\n            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,\r\n        };\r\n\r\n        public ");
            
            #line 31 "C:\Dev\Intent.Modules.NET_Proxy\Modules\Intent.Modules.ServiceProxies\Templates\ServiceProxyClient\ServiceProxyClientTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(ClassName));
            
            #line default
            #line hidden
            this.Write("(HttpClient httpClient)\r\n        {\r\n            _httpClient = httpClient;\r\n        }\r\n\r\n");
            
            #line 36 "C:\Dev\Intent.Modules.NET_Proxy\Modules\Intent.Modules.ServiceProxies\Templates\ServiceProxyClient\ServiceProxyClientTemplate.tt"

    foreach (var operation in Model.MappedService.Operations)
    {

            
            #line default
            #line hidden
            this.Write("        public async ");
            
            #line 40 "C:\Dev\Intent.Modules.NET_Proxy\Modules\Intent.Modules.ServiceProxies\Templates\ServiceProxyClient\ServiceProxyClientTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(GetReturnType(operation)));
            
            #line default
            #line hidden
            this.Write(" ");
            
            #line 40 "C:\Dev\Intent.Modules.NET_Proxy\Modules\Intent.Modules.ServiceProxies\Templates\ServiceProxyClient\ServiceProxyClientTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(GetOperationName(operation)));
            
            #line default
            #line hidden
            this.Write("(");
            
            #line 40 "C:\Dev\Intent.Modules.NET_Proxy\Modules\Intent.Modules.ServiceProxies\Templates\ServiceProxyClient\ServiceProxyClientTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(GetOperationParameters(operation)));
            
            #line default
            #line hidden
            this.Write(")\r\n        {");
            
            #line 41 "C:\Dev\Intent.Modules.NET_Proxy\Modules\Intent.Modules.ServiceProxies\Templates\ServiceProxyClient\ServiceProxyClientTemplate.tt"

        // We're leveraging the C# $"" notation to actually take leverage of the parameters
        // that are meant to be Route-based.

            
            #line default
            #line hidden
            this.Write("            var relativeUri = $\"");
            
            #line 45 "C:\Dev\Intent.Modules.NET_Proxy\Modules\Intent.Modules.ServiceProxies\Templates\ServiceProxyClient\ServiceProxyClientTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(GetRelativeUri(operation)));
            
            #line default
            #line hidden
            this.Write("\";\r\n");
            
            #line 46 "C:\Dev\Intent.Modules.NET_Proxy\Modules\Intent.Modules.ServiceProxies\Templates\ServiceProxyClient\ServiceProxyClientTemplate.tt"

        if (HasQueryParameter(operation))
        {

            
            #line default
            #line hidden
            this.Write("            \r\n            var queryParams = new Dictionary<string, string>();\r\n");
            
            #line 52 "C:\Dev\Intent.Modules.NET_Proxy\Modules\Intent.Modules.ServiceProxies\Templates\ServiceProxyClient\ServiceProxyClientTemplate.tt"

            foreach (var queryParameter in GetQueryParameters(operation))
            {

            
            #line default
            #line hidden
            this.Write("            queryParams.Add(\"");
            
            #line 56 "C:\Dev\Intent.Modules.NET_Proxy\Modules\Intent.Modules.ServiceProxies\Templates\ServiceProxyClient\ServiceProxyClientTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(queryParameter.Name.ToCamelCase()));
            
            #line default
            #line hidden
            this.Write("\", ");
            
            #line 56 "C:\Dev\Intent.Modules.NET_Proxy\Modules\Intent.Modules.ServiceProxies\Templates\ServiceProxyClient\ServiceProxyClientTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(GetParameterValueExpression(queryParameter)));
            
            #line default
            #line hidden
            this.Write(");\r\n");
            
            #line 57 "C:\Dev\Intent.Modules.NET_Proxy\Modules\Intent.Modules.ServiceProxies\Templates\ServiceProxyClient\ServiceProxyClientTemplate.tt"

            }

            
            #line default
            #line hidden
            this.Write("            relativeUri = QueryHelpers.AddQueryString(relativeUri, queryParams);\r\n\r\n");
            
            #line 62 "C:\Dev\Intent.Modules.NET_Proxy\Modules\Intent.Modules.ServiceProxies\Templates\ServiceProxyClient\ServiceProxyClientTemplate.tt"

        }

            
            #line default
            #line hidden
            this.Write("            var request = new HttpRequestMessage(");
            
            #line 65 "C:\Dev\Intent.Modules.NET_Proxy\Modules\Intent.Modules.ServiceProxies\Templates\ServiceProxyClient\ServiceProxyClientTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(GetHttpVerb(operation)));
            
            #line default
            #line hidden
            this.Write(", relativeUri);\r\n            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(\"application/json\"));\r\n");
            
            #line 67 "C:\Dev\Intent.Modules.NET_Proxy\Modules\Intent.Modules.ServiceProxies\Templates\ServiceProxyClient\ServiceProxyClientTemplate.tt"

        foreach (var headerParameter in GetHeaderParameters(operation))
        {

            
            #line default
            #line hidden
            this.Write("            request.Headers.Add(\"");
            
            #line 71 "C:\Dev\Intent.Modules.NET_Proxy\Modules\Intent.Modules.ServiceProxies\Templates\ServiceProxyClient\ServiceProxyClientTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(headerParameter.HeaderName));
            
            #line default
            #line hidden
            this.Write("\", ");
            
            #line 71 "C:\Dev\Intent.Modules.NET_Proxy\Modules\Intent.Modules.ServiceProxies\Templates\ServiceProxyClient\ServiceProxyClientTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(headerParameter.Parameter.Name.ToParameterName()));
            
            #line default
            #line hidden
            this.Write(");\r\n\r\n");
            
            #line 73 "C:\Dev\Intent.Modules.NET_Proxy\Modules\Intent.Modules.ServiceProxies\Templates\ServiceProxyClient\ServiceProxyClientTemplate.tt"

        }

        if (HasBodyParameter(operation))
        {

            
            #line default
            #line hidden
            this.Write("            \r\n            var content = JsonSerializer.Serialize(");
            
            #line 80 "C:\Dev\Intent.Modules.NET_Proxy\Modules\Intent.Modules.ServiceProxies\Templates\ServiceProxyClient\ServiceProxyClientTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(GetBodyParameterName(operation)));
            
            #line default
            #line hidden
            this.Write(", _serializerOptions);\r\n            request.Content = new StringContent(content, Encoding.Default, \"application/json\");\r\n\r\n");
            
            #line 83 "C:\Dev\Intent.Modules.NET_Proxy\Modules\Intent.Modules.ServiceProxies\Templates\ServiceProxyClient\ServiceProxyClientTemplate.tt"

        }
        else if (HasFormUrlEncodedParameter(operation))
        {

            
            #line default
            #line hidden
            this.Write("            \r\n            var formVariables = new List<KeyValuePair<string, string>>();\r\n");
            
            #line 90 "C:\Dev\Intent.Modules.NET_Proxy\Modules\Intent.Modules.ServiceProxies\Templates\ServiceProxyClient\ServiceProxyClientTemplate.tt"

            foreach (var formParameter in GetFormUrlEncodedParameters(operation))
            {

            
            #line default
            #line hidden
            this.Write("            formVariables.Add(new KeyValuePair<string, string>(\"");
            
            #line 94 "C:\Dev\Intent.Modules.NET_Proxy\Modules\Intent.Modules.ServiceProxies\Templates\ServiceProxyClient\ServiceProxyClientTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(formParameter.Name.ToPascalCase()));
            
            #line default
            #line hidden
            this.Write("\", ");
            
            #line 94 "C:\Dev\Intent.Modules.NET_Proxy\Modules\Intent.Modules.ServiceProxies\Templates\ServiceProxyClient\ServiceProxyClientTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(GetParameterValueExpression(formParameter)));
            
            #line default
            #line hidden
            this.Write("));\r\n");
            
            #line 95 "C:\Dev\Intent.Modules.NET_Proxy\Modules\Intent.Modules.ServiceProxies\Templates\ServiceProxyClient\ServiceProxyClientTemplate.tt"

            }

            
            #line default
            #line hidden
            this.Write("            var content = new FormUrlEncodedContent(formVariables);\r\n            request.Content = content;\r\n");
            
            #line 100 "C:\Dev\Intent.Modules.NET_Proxy\Modules\Intent.Modules.ServiceProxies\Templates\ServiceProxyClient\ServiceProxyClientTemplate.tt"

        }

            
            #line default
            #line hidden
            this.Write("            \r\n            using (var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false))\r\n            {\r\n                if (!response.IsSuccessStatusCode)\r\n                {\r\n                    throw await GetHttpRequestException(request, response, cancellationToken).ConfigureAwait(false);\r\n                }\r\n");
            
            #line 110 "C:\Dev\Intent.Modules.NET_Proxy\Modules\Intent.Modules.ServiceProxies\Templates\ServiceProxyClient\ServiceProxyClientTemplate.tt"

        if (HasResponseType(operation))
        {

            
            #line default
            #line hidden
            this.Write("                if (response.StatusCode == HttpStatusCode.NoContent || response.Content.Headers.ContentLength == 0)\r\n                {\r\n                    return null;\r\n                }\r\n\r\n                using (var contentStream = await response.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false))\r\n                {\r\n                    return await JsonSerializer.DeserializeAsync<");
            
            #line 121 "C:\Dev\Intent.Modules.NET_Proxy\Modules\Intent.Modules.ServiceProxies\Templates\ServiceProxyClient\ServiceProxyClientTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(GetTypeName(operation.ReturnType)));
            
            #line default
            #line hidden
            this.Write(">(contentStream, _serializerOptions, cancellationToken).ConfigureAwait(false);\r\n                }\r\n");
            
            #line 123 "C:\Dev\Intent.Modules.NET_Proxy\Modules\Intent.Modules.ServiceProxies\Templates\ServiceProxyClient\ServiceProxyClientTemplate.tt"

        }

            
            #line default
            #line hidden
            this.Write("            }\r\n        }\r\n");
            
            #line 128 "C:\Dev\Intent.Modules.NET_Proxy\Modules\Intent.Modules.ServiceProxies\Templates\ServiceProxyClient\ServiceProxyClientTemplate.tt"

    }

            
            #line default
            #line hidden
            this.Write("\r\n        public void Dispose()\r\n        {\r\n        }\r\n\r\n        private async Task<HttpRequestException> GetHttpRequestException(HttpRequestMessage request, HttpResponseMessage response, CancellationToken cancellationToken)\r\n        {\r\n            var fullRequestUri = new Uri(_httpClient.BaseAddress");
            
            #line 138 "C:\Dev\Intent.Modules.NET_Proxy\Modules\Intent.Modules.ServiceProxies\Templates\ServiceProxyClient\ServiceProxyClientTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(NotNull));
            
            #line default
            #line hidden
            this.Write(", request.RequestUri");
            
            #line 138 "C:\Dev\Intent.Modules.NET_Proxy\Modules\Intent.Modules.ServiceProxies\Templates\ServiceProxyClient\ServiceProxyClientTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(NotNull));
            
            #line default
            #line hidden
            this.Write(");\r\n            var content = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);\r\n            return new HttpRequestException(\r\n                $\"Request to {fullRequestUri} failed with status code {(int)response.StatusCode} {response.ReasonPhrase}.{Environment.NewLine}{content}\",\r\n                null,\r\n                response.StatusCode);\r\n        }\r\n    }\r\n}");
            return this.GenerationEnvironment.ToString();
        }
    }
    
}
