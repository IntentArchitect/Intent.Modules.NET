﻿using Intent.Engine;
using Intent.Modules.Common.VisualStudio;
using System;

namespace Intent.Modules.IntegrationTesting
{
    public class NugetPackages
    {
        
        public static readonly INugetPackageInfo MicrosoftNETTestSdk = new NugetPackageInfo("Microsoft.NET.Test.Sdk", "17.6.0");
        public static readonly INugetPackageInfo Xunit = new NugetPackageInfo("xunit", "2.4.2");
        public static readonly INugetPackageInfo XunitRunnerVisualstudio = new NugetPackageInfo("xunit.runner.visualstudio", "2.4.5");
        public static readonly INugetPackageInfo TestcontainersCosmosDb = new NugetPackageInfo("Testcontainers.CosmosDb", "3.7.0");
        public static readonly INugetPackageInfo TestcontainersMsSql = new NugetPackageInfo("Testcontainers.MsSql", "3.7.0");       

        public static NugetPackageInfo MicrosoftAspNetCoreMvcTesting(IOutputTarget outputTarget) => new(
            name: "Microsoft.AspNetCore.Mvc.Testing",
            version: outputTarget.GetMaxNetAppVersion() switch
            {
                (6, 0) => "6.0.26",
                (7, 0) => "7.0.15",
                _ => "8.0.1"
            });
    }
}
