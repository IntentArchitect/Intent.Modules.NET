﻿// ------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version: 17.0.0.0
//  
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
// ------------------------------------------------------------------------------
namespace Intent.Modules.AspNetCore.Docker.Templates.DockerFile
{
    using Intent.Modules.Common.Templates;
    using System;
    
    /// <summary>
    /// Class to produce the template output
    /// </summary>
    
    #line 1 "C:\Dev\Intent.Modules.NET\Modules\Intent.Modules.AspNetCore.Docker\Templates\DockerFile\DockerfileTemplate.tt"
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.TextTemplating", "17.0.0.0")]
    public partial class DockerfileTemplate : IntentFileTemplateBase<object>
    {
#line hidden
        /// <summary>
        /// Create the template output
        /// </summary>
        public override string TransformText()
        {
            this.Write(@"#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

#Depending on the operating system of the host machines(s) that will build or run the containers, the image specified in the FROM statement may need to be changed.
#For more information, please see https://aka.ms/containercompat

FROM ");
            
            #line 9 "C:\Dev\Intent.Modules.NET\Modules\Intent.Modules.AspNetCore.Docker\Templates\DockerFile\DockerfileTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(GetRuntime()));
            
            #line default
            #line hidden
            this.Write(" AS base\r\nWORKDIR /app\r\nEXPOSE 8080\r\nEXPOSE 443\r\n\r\nFROM ");
            
            #line 14 "C:\Dev\Intent.Modules.NET\Modules\Intent.Modules.AspNetCore.Docker\Templates\DockerFile\DockerfileTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(GetSdk()));
            
            #line default
            #line hidden
            this.Write(" AS build\r\nWORKDIR /src\r\nCOPY [\"");
            
            #line 16 "C:\Dev\Intent.Modules.NET\Modules\Intent.Modules.AspNetCore.Docker\Templates\DockerFile\DockerfileTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Project.Name));
            
            #line default
            #line hidden
            this.Write(".csproj\", \"");
            
            #line 16 "C:\Dev\Intent.Modules.NET\Modules\Intent.Modules.AspNetCore.Docker\Templates\DockerFile\DockerfileTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Project.Name));
            
            #line default
            #line hidden
            this.Write("/\"]\r\nRUN dotnet restore \"");
            
            #line 17 "C:\Dev\Intent.Modules.NET\Modules\Intent.Modules.AspNetCore.Docker\Templates\DockerFile\DockerfileTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Project.Name));
            
            #line default
            #line hidden
            this.Write("/");
            
            #line 17 "C:\Dev\Intent.Modules.NET\Modules\Intent.Modules.AspNetCore.Docker\Templates\DockerFile\DockerfileTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Project.Name));
            
            #line default
            #line hidden
            this.Write(".csproj\"\r\nCOPY . .\r\nWORKDIR \"/src/");
            
            #line 19 "C:\Dev\Intent.Modules.NET\Modules\Intent.Modules.AspNetCore.Docker\Templates\DockerFile\DockerfileTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Project.Name));
            
            #line default
            #line hidden
            this.Write("\"\r\nRUN dotnet build \"");
            
            #line 20 "C:\Dev\Intent.Modules.NET\Modules\Intent.Modules.AspNetCore.Docker\Templates\DockerFile\DockerfileTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Project.Name));
            
            #line default
            #line hidden
            this.Write(".csproj\" -c Release -o /app/build\r\n\r\nFROM build AS publish\r\nRUN dotnet publish \"");
            
            #line 23 "C:\Dev\Intent.Modules.NET\Modules\Intent.Modules.AspNetCore.Docker\Templates\DockerFile\DockerfileTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Project.Name));
            
            #line default
            #line hidden
            this.Write(".csproj\" -c Release -o /app/publish\r\n\r\nFROM base AS final\r\nWORKDIR /app/publish\r\n" +
                    "COPY --from=publish /app .\r\nENTRYPOINT [\"dotnet\", \"");
            
            #line 28 "C:\Dev\Intent.Modules.NET\Modules\Intent.Modules.AspNetCore.Docker\Templates\DockerFile\DockerfileTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Project.Name));
            
            #line default
            #line hidden
            this.Write(".dll\"]");
            return this.GenerationEnvironment.ToString();
        }
    }
    
    #line default
    #line hidden
}
