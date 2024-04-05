using System.Diagnostics;
using Intent.Engine;
using Intent.Modules.Common.VisualStudio;

namespace Intent.Modules.EntityFrameworkCore;

public static class NugetPackages
{
    public static NugetPackageInfo EntityFrameworkCore(IOutputTarget outputTarget) => new(
        name: "Microsoft.EntityFrameworkCore",
        version: GetMicrosoftEfVersion(outputTarget));

    public static NugetPackageInfo EntityFrameworkCoreDesign(IOutputTarget outputTarget) => new(
        name: "Microsoft.EntityFrameworkCore.Design",
        version: GetMicrosoftEfVersion(outputTarget));

    public static NugetPackageInfo EntityFrameworkCoreTools(IOutputTarget outputTarget) => new(
        name: "Microsoft.EntityFrameworkCore.Tools",
        version: GetMicrosoftEfVersion(outputTarget));

    public static NugetPackageInfo EntityFrameworkCoreSqlServer(IOutputTarget outputTarget) => new(
        name: "Microsoft.EntityFrameworkCore.SqlServer",
        version: GetMicrosoftEfVersion(outputTarget));
    public static NugetPackageInfo EntityFrameworkCoreCosmos(IOutputTarget outputTarget) => new(
        name: "Microsoft.EntityFrameworkCore.Cosmos",
        version: GetMicrosoftEfVersion(outputTarget));

    public static NugetPackageInfo EntityFrameworkCoreInMemory(IOutputTarget outputTarget) => new(
        name: "Microsoft.EntityFrameworkCore.InMemory",
        version: GetMicrosoftEfVersion(outputTarget));

    public static NugetPackageInfo EntityFrameworkCoreProxies(IOutputTarget outputTarget) => new(
        name: "Microsoft.EntityFrameworkCore.Proxies",
        version: GetMicrosoftEfVersion(outputTarget));

    public static NugetPackageInfo NpgsqlEntityFrameworkCorePostgreSQL(IOutputTarget outputTarget) => new(
        name: "Npgsql.EntityFrameworkCore.PostgreSQL",
        version: outputTarget.GetMaxNetAppVersion() switch
        {
            (5, 0) => "5.0.10",
            (6, 0) => "6.0.22",
            (7, 0) => "7.0.11",
            _ => "8.0.0"
        });

    public static NugetPackageInfo MySqlEntityFrameworkCore(IOutputTarget outputTarget) => new(
        name: "Pomelo.EntityFrameworkCore.MySql",
        version: outputTarget.GetMaxNetAppVersion() switch
        {
            (5, 0) => "5.0.4",
            (6, 0) => "6.0.2",
            (7, 0) => "7.0.0",
            _ => "8.0.0"
        });

    public static NugetPackageInfo OracleEntityFrameworkCore(IOutputTarget outputTarget) => new(
        name: "Oracle.EntityFrameworkCore",
        version: outputTarget.GetMaxNetAppVersion() switch
        {
            (5, 0) => "5.21.90",
            (6, 0) => "6.21.130",
            (7, 0) => "7.21.13",
            _ => "8.21.121"
        });

    private static string GetMicrosoftEfVersion(IOutputTarget outputTarget) => outputTarget.GetMaxNetAppVersion() switch
    {
        (5, 0) => "5.0.17",
        (6, 0) => "6.0.25",
        (7, 0) => "7.0.14",
        _ => "8.0.0"
    };
}