using System.Linq;
using Intent.Metadata.DocumentDB.Api;
using Intent.Modelers.Domain.Api;

namespace Intent.Modules.Dapr.AspNetCore.StateManagement.Templates;

internal static class ClassModelExtensionMethods
{
    public static AttributeModel GetPrimaryKeyAttribute(this ClassModel model)
    {
        var @class = model;
        while (@class != null)
        {
            var primaryKeyAttribute = @class.Attributes.SingleOrDefault(x => x.HasPrimaryKey());
            if (primaryKeyAttribute != null)
            {
                return primaryKeyAttribute;
            }

            @class = @class.ParentClass;
        }

        return null;
    }
}