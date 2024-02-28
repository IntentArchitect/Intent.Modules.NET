using Intent.Engine;
using Intent.Modules.Common.VisualStudio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intent.Modules.WindowsServiceHost
{
	internal class NuGetPackages
	{
		public static NugetPackageInfo MicrosoftExtensionsHosting(IOutputTarget outputTarget) => new (
			name: "Microsoft.Extensions.Hosting",
			version : outputTarget.GetMaxNetAppVersion() switch
			{
				(6, 0) => "7.0.0",
				(7, 0) => "7.0.0",
				_ => "8.0.0"
			});

		public static NugetPackageInfo MicrosoftExtensionsHostingWindowsServices(IOutputTarget outputTarget) => new(
			name: "Microsoft.Extensions.Hosting.WindowsServices",
			version: outputTarget.GetMaxNetAppVersion() switch
			{
				(6, 0) => "7.0.0",
				(7, 0) => "7.0.0",
				_ => "8.0.0"
			});

		public static NugetPackageInfo MicrosoftExtensionsDependencyInjection(IOutputTarget outputTarget) => new(
			name: "Microsoft.Extensions.DependencyInjection",
			version: outputTarget.GetMaxNetAppVersion() switch
			{
				(6, 0) => "7.0.0",
				(7, 0) => "7.0.0",
				_ => "8.0.0"
			});
		public static NugetPackageInfo MicrosoftExtensionsConfigurationAbstractions(IOutputTarget outputTarget) => new(
			name: "Microsoft.Extensions.Configuration.Abstractions",
			version: outputTarget.GetMaxNetAppVersion() switch
			{
				(6, 0) => "7.0.0",
				(7, 0) => "7.0.0",
				_ => "8.0.0"
			});
		public static NugetPackageInfo MicrosoftExtensionsConfigurationBinder(IOutputTarget outputTarget) => new(
			name: "Microsoft.Extensions.Configuration.Binder",
			version: outputTarget.GetMaxNetAppVersion() switch
			{
				(6, 0) => "7.0.0",
				(7, 0) => "7.0.0",
				_ => "8.0.0"
			});

		
			

	}
}
