using FluentValidation;
using Intent.RoslynWeaver.Attributes;
using Morcatko.AspNetCore.JsonMergePatch;
using SwashbuckleSettings.NET11.Application.Common.Patching;
using SwashbuckleSettings.NET11.Application.Common.Validation;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.Controllers.JsonPatch.Templates.JsonMergePatchExecutor", Version = "1.0")]

namespace SwashbuckleSettings.NET11.Api.Patching
{
    public class JsonMergePatchExecutor<T> : IPatchExecutor<T>
        where T : class
    {
        private readonly JsonMergePatchDocument<T> _document;
        private readonly IValidatorProvider _validatorProvider;

        public JsonMergePatchExecutor(JsonMergePatchDocument<T> document, IValidatorProvider validatorProvider)
        {
            _document = document ?? throw new ArgumentNullException(nameof(document));
            _validatorProvider = validatorProvider ?? throw new ArgumentNullException(nameof(validatorProvider));
        }

        public async Task ApplyToAsync(T target, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(target);

            _document.ApplyTo(target);

            var validator = _validatorProvider.GetValidator<T>();

            var validationResult = await validator.ValidateAsync(target, cancellationToken);

            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }
        }
    }
}