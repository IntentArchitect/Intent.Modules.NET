using CleanArchitecture.Dapr.InvocationClient.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace CleanArchitecture.Dapr.InvocationClient.Application.Clients.CallGetClientExtraFields
{
    public class CallGetClientExtraFieldsCommand : IRequest, ICommand
    {
        public CallGetClientExtraFieldsCommand(Guid id, string field1, string field2)
        {
            Id = id;
            Field1 = field1;
            Field2 = field2;
        }

        public Guid Id { get; set; }
        public string Field1 { get; set; }
        public string Field2 { get; set; }
    }
}