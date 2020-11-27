using System;
using System.Collections.Generic;
using System.Text;
using Intent.Modules.Common.VisualStudio;

namespace Intent.Modules.Application.MediatR.Templates
{
    public class NuGetPackages
    {
        public static INugetPackageInfo MediatR = new NugetPackageInfo("MediatR", "9.0.*");
    }
}
