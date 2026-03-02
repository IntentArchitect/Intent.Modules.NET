using Intent.Modelers.Domain.Api;
using Intent.Modules.Common.Templates;

namespace Intent.Modules.Application.MediatR.CRUD.Eventing.FactoryExtensions
{
    internal static class ClassModelExtensionMethods
    {
        public static string GetExistingVariableName(this ClassModel @class)
        {
            return $"existing{@class.Name.ToPascalCase()}";
        }

        public static string GetNewVariableName(this ClassModel @class)
        {
            return $"new{@class.Name.ToPascalCase()}";
        }
    }
}
