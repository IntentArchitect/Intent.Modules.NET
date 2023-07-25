using System;
using System.Collections.Generic;
using System.Text;
using Intent.Engine;
using Intent.Modules.Common.CSharp.VisualStudio;
using Intent.Modules.Common.VisualStudio;

namespace Intent.Modules.Application.DependencyInjection.MediatR
{
    public class NuGetPackages
    {
        public static INugetPackageInfo MediatR = new NugetPackageInfo("MediatR", "12.1.1");
    }
}
