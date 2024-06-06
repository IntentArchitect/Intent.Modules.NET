using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;
using TrainingModel.Tests.Application.Common.Validation;
using TrainingModel.Tests.Domain.Repositories;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace TrainingModel.Tests.Application.Orders.CreateOrder
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class CreateOrderCommandValidator : AbstractValidator<CreateOrderCommand>
    {
        private readonly IOrderRepository _orderRepository;

        [IntentManaged(Mode.Merge)]
        public CreateOrderCommandValidator(IValidatorProvider provider, IOrderRepository orderRepository)
        {
            ConfigureValidationRules(provider);
            _orderRepository = orderRepository;
        }

        private void ConfigureValidationRules(IValidatorProvider provider)
        {
            RuleFor(v => v.RefNo)
                .NotNull()
                .MustAsync(CheckUniqueConstraint_RefNo)
                .WithMessage("RefNo already exists.");

            RuleFor(v => v.OrderItems)
                .NotNull()
                .ForEach(x => x.SetValidator(provider.GetValidator<CreateOrderCommandOrderItemsDto>()!));
        }

        private async Task<bool> CheckUniqueConstraint_RefNo(string value, CancellationToken cancellationToken)
        {
            return !await _orderRepository.AnyAsync(p => p.RefNo == value, cancellationToken);
        }
    }
}