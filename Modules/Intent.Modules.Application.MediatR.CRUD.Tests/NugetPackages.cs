using Intent.Modules.Common.VisualStudio;

namespace Intent.Modules.Application.MediatR.CRUD.Tests;

public static class NugetPackages
{
    public static readonly INugetPackageInfo AutoFixture = new NugetPackageInfo("AutoFixture", "4.18.0");
    public static readonly INugetPackageInfo FluentAssertions = new NugetPackageInfo("FluentAssertions", "6.10.0");
    public static readonly INugetPackageInfo MicrosoftNetTestSdk = new NugetPackageInfo("Microsoft.NET.Test.Sdk", "17.6.0");
    public static readonly INugetPackageInfo NSubstitute = new NugetPackageInfo("NSubstitute", "5.0.0");
    public static readonly INugetPackageInfo Xunit = new NugetPackageInfo("xunit", "2.4.2");
    public static readonly INugetPackageInfo XunitRunnerVisualstudio = new NugetPackageInfo("xunit.runner.visualstudio", "2.4.5");
    
}