﻿// ------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version: 16.0.0.0
//  
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
// ------------------------------------------------------------------------------
namespace Intent.Modules.Entities.Repositories.Api.Templates.PagedResultInterface
{
    using Intent.Modelers.Domain.Api;
    using Intent.Modules.Common.Templates;
    using Intent.Modules.Common.CSharp.Templates;
    using System;
    using System.IO;
    using System.Diagnostics;
    using System.Linq;
    using System.Collections;
    using System.Collections.Generic;
    using System;
    
    /// <summary>
    /// Class to produce the template output
    /// </summary>
    
    #line 1 "C:\Dev\Intent.Modules.NET\Modules\Intent.Modules.Entities.Repositories.Api\Templates\PagedResultInterface\PagedResultInterfaceTemplate.tt"
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.TextTemplating", "16.0.0.0")]
    public partial class PagedResultInterfaceTemplate : CSharpTemplateBase<object>
    {
#line hidden
        /// <summary>
        /// Create the template output
        /// </summary>
        public override string TransformText()
        {
            this.Write("using System.Collections.Generic;\r\n\r\n[assembly: DefaultIntentManaged(Mode.Fully)]\r\n\r\nnamespace ");
            
            #line 17 "C:\Dev\Intent.Modules.NET\Modules\Intent.Modules.Entities.Repositories.Api\Templates\PagedResultInterface\PagedResultInterfaceTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Namespace));
            
            #line default
            #line hidden
            this.Write("\r\n{\r\n    /// <summary>\r\n    /// Instead of retrieving the entire collection of elements from\r\n    /// a persistence store, a single <see cref=\"IPagedResult{T}\"/> is returned\r\n    /// representing a single \"page\" of elements. Supplying a different <b>PageNo</b>\r\n    /// will return a different \"page\" of elements. \r\n    /// </summary>\r\n    /// <typeparam name=\"T\">Type of elements</typeparam>\r\n    public interface ");
            
            #line 26 "C:\Dev\Intent.Modules.NET\Modules\Intent.Modules.Entities.Repositories.Api\Templates\PagedResultInterface\PagedResultInterfaceTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(ClassName));
            
            #line default
            #line hidden
            this.Write("<out T> : IEnumerable<T>\r\n    {\r\n        int TotalCount { get;  }\r\n        int PageCount { get; }\r\n        int PageNo { get; }\r\n        int PageSize { get; }\r\n    }\r\n}\r\n");
            return this.GenerationEnvironment.ToString();
        }
    }
    
}
