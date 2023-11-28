﻿using Intent.Engine;
using Intent.Modules.Common.VisualStudio;

namespace Intent.Modules.AspNetCore.Identity.AccountController;

public static class NugetPackages
{
    public static readonly NugetPackageInfo IdentityModel = new("IdentityModel", "6.0.0");

    public static NugetPackageInfo MicrosoftAspNetCoreAuthenticationJwtBearer(IOutputTarget outputTarget) => new(
        name: "Microsoft.AspNetCore.Authentication.JwtBearer",
        version: outputTarget.GetMaxNetAppVersion() switch
        {
            (5, 0) => "5.0.17",
            (6, 0) => "6.0.25",
            (7, 0) => "7.0.14",
            _ => "8.0.0"
        });
}