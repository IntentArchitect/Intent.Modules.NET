using Intent.Metadata.Models;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intent.Modules.MongoDb.Templates
{
    public class MongoDbProvider
    {
        public const string Id  = "5b85dde4-47fc-467d-81f7-cd5eb1aa906e";

        public static bool FilterDbProvider(ClassModel x)
        {
            if (!x.InternalElement.Package.HasStereotype("Document Database"))
                return false;
            var setting = x.InternalElement.Package.GetStereotypeProperty<IElement>("Document Database", "Provider");
            return setting == null ||
                setting.Id == Id;
        }
    }
}
