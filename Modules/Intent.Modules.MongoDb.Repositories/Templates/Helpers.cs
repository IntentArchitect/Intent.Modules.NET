using Intent.Metadata.Models;
using Intent.Modules.Common.Templates;
using Intent.Modules.Modelers.Domain.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intent.Modules.MongoDb.Repositories.Templates
{
    internal static class Helpers
    {
        public static bool CreateEntityInterfaces(this DomainSettings groupSettings) => bool.TryParse(groupSettings.GetSetting("0456dafe-a46e-466b-bf23-1fb35c094899")?.Value.ToPascalCase(), out var result) && result;
    }
}
