// ------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version: 17.0.0.0
//  
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
// ------------------------------------------------------------------------------
namespace Intent.Modules.WindowsServiceHost.Templates.DeploymentMD
{
    using System.Collections.Generic;
    using System.Linq;
    using Intent.Modules.Common;
    using Intent.Modules.Common.Templates;
    using Intent.Metadata.Models;
    using System;
    
    /// <summary>
    /// Class to produce the template output
    /// </summary>
    
    #line 1 "C:\Dev\Intent.Modules.NET\Modules\Intent.Modules.WindowsServiceHost\Templates\DeploymentMD\DeploymentMDTemplate.tt"
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.TextTemplating", "17.0.0.0")]
    public partial class DeploymentMDTemplate : IntentTemplateBase<object>
    {
#line hidden
        /// <summary>
        /// Create the template output
        /// </summary>
        public override string TransformText()
        {
            this.Write(@"

# Deployment

## Publish the application

```cmd
dotnet publish --output ""C:\custom\publish\directory""
```

## Windows Service Installation

From a command console (Terminal) running as an administrator.

### Install Windows Service

```cmd
sc.exe create """);
            
            #line 25 "C:\Dev\Intent.Modules.NET\Modules\Intent.Modules.WindowsServiceHost\Templates\DeploymentMD\DeploymentMDTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(ApplicationName()));
            
            #line default
            #line hidden
            this.Write("\" binpath=\"C:\\custom\\publish\\directory\\");
            
            #line 25 "C:\Dev\Intent.Modules.NET\Modules\Intent.Modules.WindowsServiceHost\Templates\DeploymentMD\DeploymentMDTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(ExecutableName()));
            
            #line default
            #line hidden
            this.Write(".exe\"\r\n```\r\n\r\n### Remove Windows Service\r\n\r\n```cmd\r\nsc.exe delete \"");
            
            #line 31 "C:\Dev\Intent.Modules.NET\Modules\Intent.Modules.WindowsServiceHost\Templates\DeploymentMD\DeploymentMDTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(ApplicationName()));
            
            #line default
            #line hidden
            this.Write("\"\r\n```\r\n\r\n### Start Windows Service\r\n\r\n```cmd\r\nsc.exe start \"");
            
            #line 37 "C:\Dev\Intent.Modules.NET\Modules\Intent.Modules.WindowsServiceHost\Templates\DeploymentMD\DeploymentMDTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(ApplicationName()));
            
            #line default
            #line hidden
            this.Write("\"\r\n```\r\n\r\n### Stop Windows Service\r\n\r\n```cmd\r\nsc.exe stop \"");
            
            #line 43 "C:\Dev\Intent.Modules.NET\Modules\Intent.Modules.WindowsServiceHost\Templates\DeploymentMD\DeploymentMDTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(ApplicationName()));
            
            #line default
            #line hidden
            this.Write("\"\r\n```\r\n\r\n## Check you service in Windows\r\n\r\n- Press `Windows Key` + \'R\'\r\n- Type " +
                    "`Services`\r\n- Look for your service by name \"");
            
            #line 50 "C:\Dev\Intent.Modules.NET\Modules\Intent.Modules.WindowsServiceHost\Templates\DeploymentMD\DeploymentMDTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(ApplicationName()));
            
            #line default
            #line hidden
            this.Write("\"");
            return this.GenerationEnvironment.ToString();
        }
    }
    
    #line default
    #line hidden
}
