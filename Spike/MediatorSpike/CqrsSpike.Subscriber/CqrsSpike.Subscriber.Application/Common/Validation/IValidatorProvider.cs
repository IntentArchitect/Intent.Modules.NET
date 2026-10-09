using FluentValidation;

namespace CqrsSpike.Subscriber.Application.Common.Validation
{
    public interface IValidatorProvider
    {
        IValidator<T> GetValidator<T>();
    }
}