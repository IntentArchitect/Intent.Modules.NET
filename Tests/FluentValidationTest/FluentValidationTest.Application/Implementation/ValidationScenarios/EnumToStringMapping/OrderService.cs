using FluentValidationTest.Application.Interfaces.ValidationScenarios.EnumToStringMapping;
using FluentValidationTest.Application.ValidationScenarios.EnumToStringMapping;
using FluentValidationTest.Domain.Entities.ValidationScenarios.EnumMapping;
using FluentValidationTest.Domain.Repositories.ValidationScenarios.EnumMapping;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.ServiceImplementations.ServiceImplementation", Version = "1.0")]

namespace FluentValidationTest.Application.Implementation.ValidationScenarios.EnumToStringMapping
{
    [IntentManaged(Mode.Merge)]
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;

        [IntentManaged(Mode.Merge)]
        public OrderService(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task ProcessOrder(ProcessOrderDto dto, CancellationToken cancellationToken = default)
        {
            var order = new Order
            {
                StatusText = dto.Status.ToString(),
                Notes = dto.Notes,
                ProcessText = dto.Process.ToString()
            };

            _orderRepository.Add(order);
        }
    }
}