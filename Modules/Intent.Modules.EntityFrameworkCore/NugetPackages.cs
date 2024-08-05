using System;
using Intent.Engine;
using Intent.Modules.Common.VisualStudio;

namespace Intent.Modules.EntityFrameworkCore
{
    public static class NugetPackages
    {

        public static NugetPackageInfo MicrosoftEntityFrameworkCore(IOutputTarget outputTarget) => new NugetPackageInfo(
            name: "Microsoft.EntityFrameworkCore",
            version: outputTarget.GetMaxNetAppVersion() switch
            {
                (>= 8, 0) => "8.0.7",
                //Locked for Azure Functions
                (>= 6, 0) => "6.0.32",
                _ => throw new Exception($"Unsupported Framework `{outputTarget.GetMaxNetAppVersion().Major}` for NuGet package 'Microsoft.EntityFrameworkCore'")
            });

        public static NugetPackageInfo MicrosoftEntityFrameworkCoreDesign(IOutputTarget outputTarget) => new NugetPackageInfo(
            name: "Microsoft.EntityFrameworkCore.Design",
            version: outputTarget.GetMaxNetAppVersion() switch
            {
                (>= 8, 0) => "8.0.7",
                //Locked for Azure Functions
                (>= 6, 0) => "6.0.32",
                _ => throw new Exception($"Unsupported Framework `{outputTarget.GetMaxNetAppVersion().Major}` for NuGet package 'Microsoft.EntityFrameworkCore.Design'")
            });

        public static NugetPackageInfo MicrosoftEntityFrameworkCoreTools(IOutputTarget outputTarget) => new NugetPackageInfo(
            name: "Microsoft.EntityFrameworkCore.Tools",
            version: outputTarget.GetMaxNetAppVersion() switch
            {
                (>= 8, 0) => "8.0.7",
                //Locked for Azure Functions
                (>= 6, 0) => "6.0.32",
                _ => throw new Exception($"Unsupported Framework `{outputTarget.GetMaxNetAppVersion().Major}` for NuGet package 'Microsoft.EntityFrameworkCore.Tools'")
            });

        public static NugetPackageInfo MicrosoftEntityFrameworkCoreSqlServer(IOutputTarget outputTarget) => new NugetPackageInfo(
            name: "Microsoft.EntityFrameworkCore.SqlServer",
            version: outputTarget.GetMaxNetAppVersion() switch
            {
                (>= 8, 0) => "8.0.7",
                //Locked for Azure Functions
                (>= 6, 0) => "6.0.32",
                _ => throw new Exception($"Unsupported Framework `{outputTarget.GetMaxNetAppVersion().Major}` for NuGet package 'Microsoft.EntityFrameworkCore.SqlServer'")
            });

        public static NugetPackageInfo MicrosoftEntityFrameworkCoreCosmos(IOutputTarget outputTarget) => new NugetPackageInfo(
            name: "Microsoft.EntityFrameworkCore.Cosmos",
            version: outputTarget.GetMaxNetAppVersion() switch
            {
                (>= 8, 0) => "8.0.7",
                //Locked for Azure Functions
                (>= 6, 0) => "6.0.32",
                _ => throw new Exception($"Unsupported Framework `{outputTarget.GetMaxNetAppVersion().Major}` for NuGet package 'Microsoft.EntityFrameworkCore.Cosmos'")
            });

        public static NugetPackageInfo MicrosoftEntityFrameworkCoreInMemory(IOutputTarget outputTarget) => new NugetPackageInfo(
            name: "Microsoft.EntityFrameworkCore.InMemory",
            version: outputTarget.GetMaxNetAppVersion() switch
            {
                (>= 8, 0) => "8.0.7",
                //Locked for Azure Functions
                (>= 6, 0) => "6.0.32",
                _ => throw new Exception($"Unsupported Framework `{outputTarget.GetMaxNetAppVersion().Major}` for NuGet package 'Microsoft.EntityFrameworkCore.InMemory'")
            });

        public static NugetPackageInfo MicrosoftEntityFrameworkCoreProxies(IOutputTarget outputTarget) => new NugetPackageInfo(
            name: "Microsoft.EntityFrameworkCore.Proxies",
            version: outputTarget.GetMaxNetAppVersion() switch
            {
                (>= 8, 0) => "8.0.7",
                //Locked for Azure Functions
                (>= 6, 0) => "6.0.32",
                _ => throw new Exception($"Unsupported Framework `{outputTarget.GetMaxNetAppVersion().Major}` for NuGet package 'Microsoft.EntityFrameworkCore.Proxies'")
            });

        public static NugetPackageInfo NpgsqlEntityFrameworkCorePostgreSQL(IOutputTarget outputTarget) => new NugetPackageInfo(
            name: "Npgsql.EntityFrameworkCore.PostgreSQL",
            version: outputTarget.GetMaxNetAppVersion() switch
            {
                (>= 8, 0) => "8.0.4",
                (>= 7, 0) => "7.0.18",
                (>= 6, 0) => "7.0.18",
                _ => throw new Exception($"Unsupported Framework `{outputTarget.GetMaxNetAppVersion().Major}` for NuGet package 'Npgsql.EntityFrameworkCore.PostgreSQL'")
            });

        public static NugetPackageInfo PomeloEntityFrameworkCoreMySql(IOutputTarget outputTarget) => new NugetPackageInfo(
            name: "Pomelo.EntityFrameworkCore.MySql",
            version: outputTarget.GetMaxNetAppVersion() switch
            {
                (>= 8, 0) => "8.0.2",
                (>= 7, 0) => "7.0.0",
                (>= 6, 0) => "7.0.0",
                _ => throw new Exception($"Unsupported Framework `{outputTarget.GetMaxNetAppVersion().Major}` for NuGet package 'Pomelo.EntityFrameworkCore.MySql'")
            });

        public static NugetPackageInfo OracleEntityFrameworkCore(IOutputTarget outputTarget) => new NugetPackageInfo(
            name: "Oracle.EntityFrameworkCore",
            version: outputTarget.GetMaxNetAppVersion() switch
            {
                (>= 8, 0) => "8.23.50",
                (>= 6, 0) => "7.21.13",
                _ => throw new Exception($"Unsupported Framework `{outputTarget.GetMaxNetAppVersion().Major}` for NuGet package 'Oracle.EntityFrameworkCore'")
            });

        public static NugetPackageInfo ErikEJEntityFrameworkCoreSqlServerDateOnlyTimeOnly(IOutputTarget outputTarget) => new NugetPackageInfo(
            name: "ErikEJ.EntityFrameworkCore.SqlServer.DateOnlyTimeOnly",
            version: outputTarget.GetMaxNetAppVersion() switch
            {
                (>= 6, 0) => "7.0.10",
                _ => throw new Exception($"Unsupported Framework `{outputTarget.GetMaxNetAppVersion().Major}` for NuGet package 'ErikEJ.EntityFrameworkCore.SqlServer.DateOnlyTimeOnly'")
            });

        public static NugetPackageInfo NpgsqlEntityFrameworkCorePostgreSQLNetTopologySuite(IOutputTarget outputTarget) => new NugetPackageInfo(
            name: "Npgsql.EntityFrameworkCore.PostgreSQL.NetTopologySuite",
            version: outputTarget.GetMaxNetAppVersion() switch
            {
                (>= 8, 0) => "8.0.4",
                (>= 6, 0) => "7.0.18",
                _ => throw new Exception($"Unsupported Framework `{outputTarget.GetMaxNetAppVersion().Major}` for NuGet package 'Npgsql.EntityFrameworkCore.PostgreSQL.NetTopologySuite'")
            });

        public static NugetPackageInfo MicrosoftEntityFrameworkCoreSqlServerNetTopologySuite(IOutputTarget outputTarget) => new NugetPackageInfo(
            name: "Microsoft.EntityFrameworkCore.SqlServer.NetTopologySuite",
            version: outputTarget.GetMaxNetAppVersion() switch
            {
                (>= 8, 0) => "8.0.7",
                (>= 6, 0) => "7.0.20",
                _ => throw new Exception($"Unsupported Framework `{outputTarget.GetMaxNetAppVersion().Major}` for NuGet package 'Microsoft.EntityFrameworkCore.SqlServer.NetTopologySuite'")
            });

        public static NugetPackageInfo PomeloEntityFrameworkCoreMySqlNetTopologySuite(IOutputTarget outputTarget) => new NugetPackageInfo(
            name: "Pomelo.EntityFrameworkCore.MySql.NetTopologySuite",
            version: outputTarget.GetMaxNetAppVersion() switch
            {
                (>= 8, 0) => "8.0.2",
                (>= 7, 0) => "7.0.0",
                (>= 6, 0) => "7.0.0",
                _ => throw new Exception($"Unsupported Framework `{outputTarget.GetMaxNetAppVersion().Major}` for NuGet package 'Pomelo.EntityFrameworkCore.MySql.NetTopologySuite'")
            });
    }
}
