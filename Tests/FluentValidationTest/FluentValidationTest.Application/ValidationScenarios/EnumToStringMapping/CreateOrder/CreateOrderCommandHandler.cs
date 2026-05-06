using FluentValidationTest.Domain.Entities.ValidationScenarios.EnumMapping;
using FluentValidationTest.Domain.Repositories.ValidationScenarios.EnumMapping;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace FluentValidationTest.Application.ValidationScenarios.EnumToStringMapping.CreateOrder
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand>
    {
        private readonly IOrderRepository _orderRepository;

        [IntentManaged(Mode.Merge)]
        public CreateOrderCommandHandler(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        /// <summary>
        /// CreateOrderCommand DTO with enum-to-string mapping scenario.
        /// 
        /// Test Case: Verify enum field (Status) generates IsInEnum() validator.
        /// Also verify string field (Notes) generates MaximumLength(100) validator.
        /// 
        /// Domain Entity Target: Order (OrderStatus enum mapped to Order.StatusText string)
        /// </summary>
        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(CreateOrderCommand request, CancellationToken cancellationToken)
        {
            var order = new Order
            {
                StatusText = request.Status.ToString(),
                Notes = request.Notes,
                ProcessText = request.Process.ToString()
            };

            _orderRepository.Add(order);
        }
    }
}