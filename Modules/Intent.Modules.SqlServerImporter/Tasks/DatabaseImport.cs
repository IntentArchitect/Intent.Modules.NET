using Intent.Plugins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intent.Modules.SqlServerImporter.Tasks
{
	public class DatabaseImport : IModuleTask
	{
		public string TaskTypeId => "Intent.Modules.SqlServerImporter.Tasks.DatabaseImport";

		public string TaskTypeName => "Rdbms Database Import";

		public int Order => 0;

		public string Execute(params string[] args)
		{
			return "hello";
		}
	}
}
