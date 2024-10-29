using Intent.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intent.Modules.MongoDb.Templates
{
    public class DocumentTemplateHelpers
    {
        internal static bool IsSeparateDatabaseMultiTenancy(IApplicationSettingsProvider settings)
        {
            const string multiTenancySettings = "41ae5a02-3eb2-42a6-ade2-322b3c1f1115";
            const string dataIsolationSetting = "be7c671e-bbef-4d75-b42d-a6547de3ae82";

            return settings.GetGroup(multiTenancySettings)?.GetSetting(dataIsolationSetting)?.Value == "separate-database";
        }

    }
}
