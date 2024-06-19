using Intent.Modules.Common.VisualStudio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intent.Modules.Dapper
{
	internal class NuGetPackages
	{
		public static readonly INugetPackageInfo Dapper = new NugetPackageInfo("Dapper", "2.1.35");
		public static readonly INugetPackageInfo SystemDataSqlClient = new NugetPackageInfo("System.Data.SqlClient", "4.8.6");
	}
}
