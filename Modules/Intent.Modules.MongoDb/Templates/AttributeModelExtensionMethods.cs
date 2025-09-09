using Intent.Metadata.DocumentDB.Api;
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
    internal static class AttributeModelExtensionMethods
    {
        public static AttributeModel GetPrimaryKeyAttribute(this ClassModel model)
        {
            AttributeModel? idAttribute = null;
            var @class = model;
            while (@class != null)
            {
                var primaryKeyAttributes = @class.Attributes.Where(x => x.HasPrimaryKey()).ToList();
                if (primaryKeyAttributes.Any())
                {
                    foreach (var pk in primaryKeyAttributes)
                    {
                        idAttribute = pk;
                    }
                }

                @class = @class.ParentClass;
            }
            return idAttribute;
        }        
    }
}
