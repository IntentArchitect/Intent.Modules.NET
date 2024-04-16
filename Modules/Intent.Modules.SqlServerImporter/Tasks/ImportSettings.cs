using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Intent.Modules.SqlServerImporter.Tasks
{
	public class ImportSettings
	{
		public string ApplicationId { get; set; }
		public string DesignerId { get; set; }
		public string PackageId { get; set; }

		//Sql Extractor Contract

		public string EntityNameConvention { get; set; }
		public string TableStereotypes { get; set; }

		public List<string> TypesToExport { get; set; }

		public List<string> SchemaFilter { get; set; }
		public string TableViewFilterFilePath { get; set; }

		public string ConnectionString { get; set; }

		public string? PackageFileName { get; set; }

		public string SettingPersistence { get; set; }

	}
}
