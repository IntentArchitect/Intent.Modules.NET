using Intent.Modules.Common.VisualStudio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intent.Modules.QuartzScheduler
{
	internal class NuGetPackages
	{
		public static readonly INugetPackageInfo QuartzExtensionsHosting = new NugetPackageInfo("Quartz.Extensions.Hosting", "3.8.1");
		public static readonly INugetPackageInfo QuartzAspNetCore = new NugetPackageInfo("Quartz.AspNetCore", "3.8.1");
	}
}
