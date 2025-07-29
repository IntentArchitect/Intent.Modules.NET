using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Blazor.Components.MudBlazor.FluentValidationProviderExtensions", Version = "1.0")]

namespace Blazor.InteractiveAuto.Jwt.Client.Common.Validation
{
    public static class ValidatorProviderExtensions
    {
        public static Func<object, string, Task<IEnumerable<string>>> GetValidationFunc<TModel>(this IValidatorProvider provider)
        {
            return async (model, propertyName) =>
            {
                var result = await provider.GetValidator<TModel>().ValidateAsync(ValidationContext<TModel>.CreateWithOptions((TModel)model, x => x.IncludeProperties(propertyName)));
                if (result.IsValid)
                    return Array.Empty<string>();
                return result.Errors.Select(e => e.ErrorMessage);
            };
        }
    }
}