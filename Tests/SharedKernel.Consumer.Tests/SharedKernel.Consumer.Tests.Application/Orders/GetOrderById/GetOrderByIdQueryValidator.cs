using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace SharedKernel.Consumer.Tests.Application.Orders.GetOrderById
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class GetOrderByIdQueryValidator : AbstractValidator<GetOrderByIdQuery>
    {
        [IntentManaged(Mode.Merge)]
        public GetOrderByIdQueryValidator()
        {
        }
    }
}