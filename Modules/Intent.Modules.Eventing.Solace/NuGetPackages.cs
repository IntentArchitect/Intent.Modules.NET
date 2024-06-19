using Intent.Modules.Common.VisualStudio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intent.Modules.Eventing.Solace
{
	internal class NuGetPackages
	{
		public static readonly INugetPackageInfo SolaceSystemsSolclientMessaging = new NugetPackageInfo("SolaceSystems.Solclient.Messaging", "10.24.0");
		public static INugetPackageInfo MicrosoftExtensionsHosting = new NugetPackageInfo("Microsoft.Extensions.Hosting", "8.0.0");
	}
}
