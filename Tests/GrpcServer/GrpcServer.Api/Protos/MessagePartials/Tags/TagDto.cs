using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using GrpcServer.Application.Tags;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.Grpc.MessagePartial", Version = "1.0")]

namespace GrpcServer.Api.Protos.Messages.Tags
{
    public partial class ListOfTagDto
    {
        public List<Application.Tags.TagDto> ToContract()
        {
            return Items.Select(x => x.ToContract()).ToList();
        }

        [return: NotNullIfNotNull(nameof(contract))]
        public static ListOfTagDto? Create(List<Application.Tags.TagDto>? contract)
        {
            if (contract == null)
            {
                return null;
            }
            var message = new ListOfTagDto();
            message.Items.AddRange(contract.Select(TagDto.Create));
            return message;
        }
    }

    public partial class TagDto
    {
        public Application.Tags.TagDto ToContract()
        {
            return new Application.Tags.TagDto
            {
                Name = Name,
                Id = Guid.Parse(Id)
            };
        }

        [return: NotNullIfNotNull(nameof(contract))]
        public static TagDto? Create(Application.Tags.TagDto? contract)
        {
            if (contract == null)
            {
                return null;
            }

            var message = new TagDto
            {
                Name = contract.Name,
                Id = contract.Id.ToString()
            };

            return message;
        }
    }
}