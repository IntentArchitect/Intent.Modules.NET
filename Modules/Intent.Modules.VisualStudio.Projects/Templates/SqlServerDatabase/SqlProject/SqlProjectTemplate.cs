// ------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version: 17.0.0.0
//  
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
// ------------------------------------------------------------------------------
namespace Intent.Modules.VisualStudio.Projects.Templates.SqlServerDatabase.SqlProject
{
    using System.Collections.Generic;
    using System.Linq;
    using Intent.Modules.Common;
    using Intent.Modules.Common.Templates;
    using Intent.Metadata.Models;
    using Intent.Modules.VisualStudio.Projects.Api;
    using System;
    
    /// <summary>
    /// Class to produce the template output
    /// </summary>
    
    #line 1 "C:\Dev\Intent.Modules.NET\Modules\Intent.Modules.VisualStudio.Projects\Templates\SqlServerDatabase\SqlProject\SqlProjectTemplate.tt"
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.TextTemplating", "17.0.0.0")]
    public partial class SqlProjectTemplate : VisualStudioProjectTemplateBase<SQLServerDatabaseProjectModel>
    {
#line hidden
        /// <summary>
        /// Create the template output
        /// </summary>
        public override string TransformText()
        {
            this.Write(@"<?xml version=""1.0"" encoding=""utf-8""?>
<Project DefaultTargets=""Build"" xmlns=""http://schemas.microsoft.com/developer/msbuild/2003"" ToolsVersion=""4.0"">
  <PropertyGroup>
    <Configuration Condition="" '$(Configuration)' == '' "">Debug</Configuration>
    <Platform Condition="" '$(Platform)' == '' "">AnyCPU</Platform>
    <Name>");
            
            #line 14 "C:\Dev\Intent.Modules.NET\Modules\Intent.Modules.VisualStudio.Projects\Templates\SqlServerDatabase\SqlProject\SqlProjectTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Model.Name));
            
            #line default
            #line hidden
            this.Write("</Name>\r\n    <SchemaVersion>2.0</SchemaVersion>\r\n    <ProjectVersion>4.1</Project" +
                    "Version>\r\n    <ProjectGuid>{");
            
            #line 17 "C:\Dev\Intent.Modules.NET\Modules\Intent.Modules.VisualStudio.Projects\Templates\SqlServerDatabase\SqlProject\SqlProjectTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Model.Id));
            
            #line default
            #line hidden
            this.Write("}</ProjectGuid>\r\n    <DSP>Microsoft.Data.Tools.Schema.Sql.Sql130DatabaseSchemaPro" +
                    "vider</DSP>\r\n    <OutputType>Database</OutputType>\r\n    <RootPath>\r\n    </RootPa" +
                    "th>\r\n    <RootNamespace>");
            
            #line 22 "C:\Dev\Intent.Modules.NET\Modules\Intent.Modules.VisualStudio.Projects\Templates\SqlServerDatabase\SqlProject\SqlProjectTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Model.Name));
            
            #line default
            #line hidden
            this.Write("</RootNamespace>\r\n    <AssemblyName>");
            
            #line 23 "C:\Dev\Intent.Modules.NET\Modules\Intent.Modules.VisualStudio.Projects\Templates\SqlServerDatabase\SqlProject\SqlProjectTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Model.Name));
            
            #line default
            #line hidden
            this.Write("</AssemblyName>\r\n    <ModelCollation>1033, CI</ModelCollation>\r\n    <DefaultFileS" +
                    "tructure>BySchemaAndSchemaType</DefaultFileStructure>\r\n    <DeployToDatabase>Tru" +
                    "e</DeployToDatabase>\r\n    <TargetFrameworkVersion>v4.5</TargetFrameworkVersion>\r" +
                    "\n    <TargetLanguage>CS</TargetLanguage>\r\n    <AppDesignerFolder>Properties</App" +
                    "DesignerFolder>\r\n    <SqlServerVerification>False</SqlServerVerification>\r\n    <" +
                    "IncludeCompositeObjects>True</IncludeCompositeObjects>\r\n    <TargetDatabaseSet>T" +
                    "rue</TargetDatabaseSet>\r\n  </PropertyGroup>\r\n  <PropertyGroup Condition=\" \'$(Con" +
                    "figuration)|$(Platform)\' == \'Release|AnyCPU\' \">\r\n    <OutputPath>bin\\Release\\</O" +
                    "utputPath>\r\n    <BuildScriptName>$(MSBuildProjectName).sql</BuildScriptName>\r\n  " +
                    "  <TreatWarningsAsErrors>False</TreatWarningsAsErrors>\r\n    <DebugType>pdbonly</" +
                    "DebugType>\r\n    <Optimize>true</Optimize>\r\n    <DefineDebug>false</DefineDebug>\r" +
                    "\n    <DefineTrace>true</DefineTrace>\r\n    <ErrorReport>prompt</ErrorReport>\r\n   " +
                    " <WarningLevel>4</WarningLevel>\r\n  </PropertyGroup>\r\n  <PropertyGroup Condition=" +
                    "\" \'$(Configuration)|$(Platform)\' == \'Debug|AnyCPU\' \">\r\n    <OutputPath>bin\\Debug" +
                    "\\</OutputPath>\r\n    <BuildScriptName>$(MSBuildProjectName).sql</BuildScriptName>" +
                    "\r\n    <TreatWarningsAsErrors>false</TreatWarningsAsErrors>\r\n    <DebugSymbols>tr" +
                    "ue</DebugSymbols>\r\n    <DebugType>full</DebugType>\r\n    <Optimize>false</Optimiz" +
                    "e>\r\n    <DefineDebug>true</DefineDebug>\r\n    <DefineTrace>true</DefineTrace>\r\n  " +
                    "  <ErrorReport>prompt</ErrorReport>\r\n    <WarningLevel>4</WarningLevel>\r\n  </Pro" +
                    "pertyGroup>\r\n  <PropertyGroup>\r\n    <VisualStudioVersion Condition=\"\'$(VisualStu" +
                    "dioVersion)\' == \'\'\">11.0</VisualStudioVersion>\r\n    <!-- Default to the v11.0 ta" +
                    "rgets path if the targets file for the current VS version is not found -->\r\n    " +
                    "<SSDTExists Condition=\"Exists(\'$(MSBuildExtensionsPath)\\Microsoft\\VisualStudio\\v" +
                    "$(VisualStudioVersion)\\SSDT\\Microsoft.Data.Tools.Schema.SqlTasks.targets\')\">True" +
                    "</SSDTExists>\r\n    <VisualStudioVersion Condition=\"\'$(SSDTExists)\' == \'\'\">11.0</" +
                    "VisualStudioVersion>\r\n  </PropertyGroup>\r\n  <Import Condition=\"\'$(SQLDBExtension" +
                    "sRefPath)\' != \'\'\" Project=\"$(SQLDBExtensionsRefPath)\\Microsoft.Data.Tools.Schema" +
                    ".SqlTasks.targets\" />\r\n  <Import Condition=\"\'$(SQLDBExtensionsRefPath)\' == \'\'\" P" +
                    "roject=\"$(MSBuildExtensionsPath)\\Microsoft\\VisualStudio\\v$(VisualStudioVersion)\\" +
                    "SSDT\\Microsoft.Data.Tools.Schema.SqlTasks.targets\" />\r\n  <ItemGroup>\r\n    <Folde" +
                    "r Include=\"Properties\" />\r\n  </ItemGroup>\r\n</Project>");
            return this.GenerationEnvironment.ToString();
        }
    }
    
    #line default
    #line hidden
}
