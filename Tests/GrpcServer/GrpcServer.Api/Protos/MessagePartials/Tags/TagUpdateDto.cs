using System;
using System.Diagnostics.CodeAnalysis;
using GrpcServer.Application.Tags;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.Grpc.MessagePartial", Version = "1.0")]

namespace GrpcServer.Api.Protos.Messages.Tags
{
    public partial class TagUpdateDto
    {
        public Application.Tags.TagUpdateDto ToContract()
        {
            return new Application.Tags.TagUpdateDto
            {
                Id = Guid.Parse(Id),
                Name = Name
            };
        }

        [return: NotNullIfNotNull(nameof(contract))]
        public static TagUpdateDto? Create(Application.Tags.TagUpdateDto? contract)
        {
            if (contract == null)
            {
                return null;
            }

            var message = new TagUpdateDto
            {
                Id = contract.Id.ToString(),
                Name = contract.Name
            };

            return message;
        }
    }
}