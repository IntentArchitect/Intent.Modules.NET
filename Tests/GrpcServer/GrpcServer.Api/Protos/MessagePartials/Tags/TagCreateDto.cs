using System.Diagnostics.CodeAnalysis;
using GrpcServer.Application.Tags;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.Grpc.MessagePartial", Version = "1.0")]

namespace GrpcServer.Api.Protos.Messages.Tags
{
    public partial class TagCreateDto
    {
        public Application.Tags.TagCreateDto ToContract()
        {
            return new Application.Tags.TagCreateDto
            {
                Name = Name
            };
        }

        [return: NotNullIfNotNull(nameof(contract))]
        public static TagCreateDto? Create(Application.Tags.TagCreateDto? contract)
        {
            if (contract == null)
            {
                return null;
            }

            var message = new TagCreateDto
            {
                Name = contract.Name
            };

            return message;
        }
    }
}