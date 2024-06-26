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
            this.Write("#See https://aka.ms/customizecontainer to learn how to customize your debug conta" +
                    "iner and how Visual Studio uses this Dockerfile to build your images for faster " +
                    "debugging.\r\n\r\nFROM ");
            
            #line 6 "C:\Dev\Intent.Modules.NET\Modules\Intent.Modules.AspNetCore.Docker\Templates\DockerFile\DockerfileTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(GetRuntime()));
            
            #line default
            #line hidden
            this.Write(" AS base\r\nUSER app\r\nWORKDIR /app\r\nEXPOSE 8080\r\nEXPOSE 8081\r\n\r\nFROM ");
            
            #line 12 "C:\Dev\Intent.Modules.NET\Modules\Intent.Modules.AspNetCore.Docker\Templates\DockerFile\DockerfileTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(GetSdk()));
            
            #line default
            #line hidden
            this.Write(" AS build\r\nARG BUILD_CONFIGURATION=Release\r\nWORKDIR /src\r\nCOPY [\"");
            
            #line 15 "C:\Dev\Intent.Modules.NET\Modules\Intent.Modules.AspNetCore.Docker\Templates\DockerFile\DockerfileTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Project.Name));
            
            #line default
            #line hidden
            this.Write(".csproj\", \"");
            
            #line 15 "C:\Dev\Intent.Modules.NET\Modules\Intent.Modules.AspNetCore.Docker\Templates\DockerFile\DockerfileTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Project.Name));
            
            #line default
            #line hidden
            this.Write("/\"]\r\nRUN dotnet restore \"");
            
            #line 16 "C:\Dev\Intent.Modules.NET\Modules\Intent.Modules.AspNetCore.Docker\Templates\DockerFile\DockerfileTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Project.Name));
            
            #line default
            #line hidden
            this.Write("/");
            
            #line 16 "C:\Dev\Intent.Modules.NET\Modules\Intent.Modules.AspNetCore.Docker\Templates\DockerFile\DockerfileTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Project.Name));
            
            #line default
            #line hidden
            this.Write(".csproj\"\r\nCOPY . .\r\nWORKDIR \"/src/");
            
            #line 18 "C:\Dev\Intent.Modules.NET\Modules\Intent.Modules.AspNetCore.Docker\Templates\DockerFile\DockerfileTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Project.Name));
            
            #line default
            #line hidden
            this.Write("\"\r\nRUN dotnet build \"");
            
            #line 19 "C:\Dev\Intent.Modules.NET\Modules\Intent.Modules.AspNetCore.Docker\Templates\DockerFile\DockerfileTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Project.Name));
            
            #line default
            #line hidden
            this.Write(".csproj\" -c $BUILD_CONFIGURATION -o /app/build\r\n\r\nFROM build AS publish\r\nARG BUIL" +
                    "D_CONFIGURATION=Release\r\nRUN dotnet publish \"");
            
            #line 23 "C:\Dev\Intent.Modules.NET\Modules\Intent.Modules.AspNetCore.Docker\Templates\DockerFile\DockerfileTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Project.Name));
            
            #line default
            #line hidden
            this.Write(".csproj\" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false\r\n\r\nFROM base" +
                    " AS final\r\nWORKDIR /app\r\nCOPY --from=publish /app/publish .\r\nENTRYPOINT [\"dotnet" +
                    "\", \"");
            
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
