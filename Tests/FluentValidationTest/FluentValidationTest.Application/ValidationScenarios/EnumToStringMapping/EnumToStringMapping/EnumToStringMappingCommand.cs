using FluentValidationTest.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace FluentValidationTest.Application.ValidationScenarios.EnumToStringMapping.EnumToStringMapping
{
    /// <summary>
    /// CreateOrderCommand DTO with enum-to-string mapping scenario.
    /// 
    /// Test Case: Verify enum field (Status) generates IsInEnum() validator.
    /// Also verify string field (Notes) generates MaximumLength(100) validator.
    /// 
    /// Domain Entity Target: Order (OrderStatus enum mapped to Order.StatusText string)
    /// </summary>
    public class EnumToStringMappingCommand : IRequest, ICommand
    {
        public EnumToStringMappingCommand(OrderStatus status, string notes, Process process)
        {
            Status = status;
            Notes = notes;
            Process = process;
        }

        /// <summary>
        /// Enum field mapped to Order.StatusText. Tests IsInEnum() validator generation.
        /// </summary>
        public OrderStatus Status { get; set; }
        /// <summary>
        /// String field with MaxLength=100 constraint. Tests MaximumLength(100) validator generation.
        /// </summary>
        public string Notes { get; set; }
        public Process Process { get; set; }
    }
}