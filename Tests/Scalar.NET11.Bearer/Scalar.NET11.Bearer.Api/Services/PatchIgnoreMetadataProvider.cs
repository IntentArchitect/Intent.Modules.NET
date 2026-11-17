using Intent.RoslynWeaver.Attributes;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using Scalar.NET11.Bearer.Application.Common.Patching;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.Controllers.JsonPatch.Templates.PatchIgnoreMetadataProvider", Version = "1.0")]

namespace Scalar.NET11.Bearer.Api.Services
{
    /// <summary>
    /// Tells ASP.NET Core's model binding to skip validation on properties 
    /// that are read-only or specifically of type IPatchExecutor.
    /// </summary>
    public class PatchIgnoreMetadataProvider : IValidationMetadataProvider
    {
        public void CreateValidationMetadata(ValidationMetadataProviderContext context)
        {
            if (context.Key.PropertyInfo == null)
            {
                return;
            }

            if (!context.Key.PropertyInfo.CanWrite || context.Key.PropertyInfo.GetSetMethod(nonPublic: false) == null)
            {
                context.ValidationMetadata.ValidateChildren = false;
                context.ValidationMetadata.HasValidators = false;
            }
            var propertyType = context.Key.PropertyInfo.PropertyType;

            if (propertyType.IsGenericType && propertyType.GetGenericTypeDefinition() == typeof(IPatchExecutor<>))
            {
                context.ValidationMetadata.ValidateChildren = false;
                context.ValidationMetadata.HasValidators = false;
            }
        }
    }
}