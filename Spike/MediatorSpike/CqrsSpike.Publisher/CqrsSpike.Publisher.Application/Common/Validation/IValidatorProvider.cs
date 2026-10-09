using FluentValidation;

namespace CqrsSpike.Publisher.Application.Common.Validation
{
    public interface IValidatorProvider
    {
        IValidator<T> GetValidator<T>();
    }
}