using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using GrpcServer.Application;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.Grpc.MessagePartial", Version = "1.0")]

namespace GrpcServer.Api.Protos.Messages
{
    public partial class ComplexTypeDto
    {
        public Application.ComplexTypeDto ToContract()
        {
            return new Application.ComplexTypeDto
            {
                Field = Field
            };
        }

        [return: NotNullIfNotNull(nameof(contract))]
        public static ComplexTypeDto? Create(Application.ComplexTypeDto? contract)
        {
            if (contract == null)
            {
                return null;
            }

            var message = new ComplexTypeDto
            {
                Field = contract.Field
            };

            return message;
        }
    }

    public partial class ListOfComplexTypeDto
    {
        public List<Application.ComplexTypeDto>? ToContract()
        {
            return Items.Select(x => x.ToContract()).ToList();
        }

        [return: NotNullIfNotNull(nameof(contract))]
        public static ListOfComplexTypeDto? Create(List<Application.ComplexTypeDto>? contract)
        {
            if (contract == null)
            {
                return null;
            }
            var message = new ListOfComplexTypeDto();
            message.Items.AddRange(contract.Select(ComplexTypeDto.Create));
            return message;
        }
    }
}