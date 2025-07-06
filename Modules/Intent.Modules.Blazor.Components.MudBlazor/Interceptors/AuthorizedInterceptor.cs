using Intent.Blazor.Api;
using Intent.Metadata.Models;
using Intent.Modules.Blazor.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.RazorBuilder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Intent.Modules.Blazor.Components.MudBlazor.Interceptors;
public class AuthorizedInterceptor(IRazorComponentTemplate template) : IRazorComponentInterceptor
{
    public void Handle(IElement component, IEnumerable<IRazorFileNode> razorNodes, IRazorFileNode node)
    {
        SecuredHelper.AuthorizeComponent(template, component, razorNodes, node);
    }
}
