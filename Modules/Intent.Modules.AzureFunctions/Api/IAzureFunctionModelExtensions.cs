using System;
using System.Collections.Generic;
using System.Linq;
using Intent.AzureFunctions.Api;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;
using static Intent.AzureFunctions.Api.AzureFunctionModelStereotypeExtensions;

namespace Intent.Modules.AzureFunctions.Api
{
    internal static class IAzureFunctionModelExtensions
    {
        public static CosmosDBTrigger GetCosmosDBTrigger(this IAzureFunctionModel model)
        {
            var stereotype = model.GetStereotype("Cosmos DB Trigger");
            return stereotype != null ? new CosmosDBTrigger(stereotype) : null;
        }


        public static bool HasCosmosDBTrigger(this IAzureFunctionModel model)
        {
            return model.HasStereotype("Cosmos DB Trigger");
        }

        public static bool TryGetCosmosDBTrigger(this IAzureFunctionModel model, out CosmosDBTrigger stereotype)
        {
            if (!HasCosmosDBTrigger(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new CosmosDBTrigger(model.GetStereotype("Cosmos DB Trigger"));
            return true;
        }

    }
}
