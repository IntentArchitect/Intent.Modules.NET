using Intent.Metadata.Models;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intent.Modules.CosmosDB.Templates
{
    internal class CosmosDbProvider
    {
        public const string Id = "3e1a00f7-c6f1-4785-a544-bbcb17602b31";

        internal static bool FilterDbProvider(ClassModel x)
        {
            if (!x.InternalElement.Package.HasStereotype("Document Database"))
                return false;
            var setting = x.InternalElement.Package.GetStereotypeProperty<IElement>("Document Database", "Provider");
            return setting == null ||
                setting.Id == Id;
        }
    }
}
