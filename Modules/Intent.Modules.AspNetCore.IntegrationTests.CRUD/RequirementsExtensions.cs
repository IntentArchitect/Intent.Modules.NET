using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intent.Modules.AspNetCore.IntegrationTests.CRUD
{
    public static class RequirementsExtensions
    {
        private const string CosmosDbProviderId = "3e1a00f7-c6f1-4785-a544-bbcb17602b31";

        public static bool RequiresCosmosDb(this IElement element, IntentTemplateBase template)
        {
            if (!element.Package.HasStereotype("Document Database"))
                return false;

            var setting = element.Package.GetStereotypeProperty<IElement>("Document Database", "Provider");

            return (setting == null  && template.ExecutionContext.InstalledModules.Any(m => m.ModuleId == "Intent.CosmosDB")) || setting?.Id == CosmosDbProviderId || ContainerHelper.RequireCosmosContainer(template);
        }
    }
}
