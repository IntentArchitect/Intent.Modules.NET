using System;
using Intent.Engine;
using Intent.Modules.Common.CSharp.VisualStudio;
using Intent.Modules.Common.VisualStudio;

namespace Intent.Modules.AspNetCore.HealthChecks;

public static class NugetPackage
{
    public static INugetPackageInfo AspNetCoreHealthChecksUIClient(IOutputTarget outputTarget) => new NugetPackageInfo("AspNetCore.HealthChecks.UI.Client", GetHealthChecksVersion(outputTarget.GetProject()));

    public static INugetPackageInfo AspNetCoreHealthChecksSqlServer(IOutputTarget outputTarget) => new NugetPackageInfo("AspNetCore.HealthChecks.SqlServer", GetHealthCheckSqlServerVersion(outputTarget.GetProject()));
    public static INugetPackageInfo AspNetCoreHealthChecksNpgSql(IOutputTarget outputTarget) => new NugetPackageInfo("AspNetCore.HealthChecks.NpgSql", GetHealthCheckNpgSqlVersion(outputTarget.GetProject()));
    public static INugetPackageInfo AspNetCoreHealthChecksMySql(IOutputTarget outputTarget) => new NugetPackageInfo("AspNetCore.HealthChecks.MySql", GetHealthCheckMySqlVersion(outputTarget.GetProject()));
    public static INugetPackageInfo AspNetCoreHealthChecksCosmosDb(IOutputTarget outputTarget) => new NugetPackageInfo("AspNetCore.HealthChecks.CosmosDb", GetHealthCheckCosmosDbVersion(outputTarget.GetProject()));

    private static string GetHealthCheckSqlServerVersion(ICSharpProject project)
    {
        return project switch
        {
            _ when project.IsNetApp(5) => "5.0.3",
            _ when project.IsNetApp(6) => "6.0.2",
            _ when project.IsNetApp(7) => "7.0.0-rc2.11",
            _ when project.IsNetApp(8) => "7.0.0-rc2.11",
            _ => throw new Exception("Not supported version of .NET Core") 
        };
    }
    
    private static string GetHealthCheckNpgSqlVersion(ICSharpProject project)
    {
        return project switch
        {
            _ when project.IsNetApp(5) => "5.0.2",
            _ when project.IsNetApp(6) => "6.0.2",
            _ when project.IsNetApp(7) => "7.0.0-rc2.7",
            _ when project.IsNetApp(8) => "7.0.0-rc2.7",
            _ => throw new Exception("Not supported version of .NET Core") 
        };
    }
    
    private static string GetHealthCheckMySqlVersion(ICSharpProject project)
    {
        return project switch
        {
            _ when project.IsNetApp(5) => "5.0.1",
            _ when project.IsNetApp(6) => "6.0.2",
            _ when project.IsNetApp(7) => "7.0.0-rc2.5",
            _ when project.IsNetApp(8) => "7.0.0-rc2.5",
            _ => throw new Exception("Not supported version of .NET Core") 
        };
    }
    
    private static string GetHealthCheckCosmosDbVersion(ICSharpProject project)
    {
        return project switch
        {
            _ when project.IsNetApp(5) => "5.0.4",
            _ when project.IsNetApp(6) => "6.1.0",
            _ when project.IsNetApp(7) => "7.0.0-rc2.5",
            _ when project.IsNetApp(8) => "7.0.0-rc2.5",
            _ => throw new Exception("Not supported version of .NET Core") 
        };
    }
    
    private static string GetHealthChecksVersion(ICSharpProject project)
    {
        return project switch
        {
            _ when project.IsNetApp(5) => "5.0.1",
            _ when project.IsNetApp(6) => "6.0.5",
            _ when project.IsNetApp(7) => "7.0.0-rc2.7",
            _ when project.IsNetApp(8) => "7.0.0-rc2.7",
            _ => throw new Exception("Not supported version of .NET Core") 
        };
    }
}