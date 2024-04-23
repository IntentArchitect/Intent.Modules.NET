using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intent.Modules.OpenApi.Importer.Tasks
{
	public class ImportSettings
	{

		//CLI Settings
		public string IslnFile { get; set; }
		public string ApplicationName { get; set; }
		public string OpenApiSpecificationFile { get; set; }
		public string TargetFolderId { get; set; }
		public string PackageId { get; set; }

		public bool AddPostFixes { get; set; }
		public bool IsAzureFunctions { get; set; }
		public string ServiceType { get; set; }
		public bool AllowRemoval { get; set; }
		public string SettingPersistence { get; set; }

	}
}
