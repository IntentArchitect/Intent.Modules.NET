using System.Diagnostics.CodeAnalysis;
using System.Linq;
using GrpcServer.Application.TypeTests;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.Grpc.MessagePartial", Version = "1.0")]

namespace GrpcServer.Api.Protos.Messages.TypeTests
{
    public partial class PagedResultTestDto
    {
        public Application.TypeTests.PagedResultTestDto ToContract()
        {
            return new Application.TypeTests.PagedResultTestDto
            {
                PagedResultField = PagedResultField.ToContract(),
                PagedResultFieldCollection = PagedResultFieldCollection.Select(x => x.ToContract()).ToList(),
                PagedResultFieldNullable = PagedResultFieldNullable?.ToContract(),
                PagedResultFieldNullableCollection = PagedResultFieldNullableCollection?.Items.Select(x => x.ToContract()).ToList()
            };
        }

        [return: NotNullIfNotNull(nameof(contract))]
        public static PagedResultTestDto? Create(Application.TypeTests.PagedResultTestDto? contract)
        {
            if (contract == null)
            {
                return null;
            }

            var message = new PagedResultTestDto
            {
                PagedResultField = PagedResultOfComplexTypeDto.Create(contract.PagedResultField),
                PagedResultFieldNullable = PagedResultOfComplexTypeDto.Create(contract.PagedResultFieldNullable),
                PagedResultFieldNullableCollection = ListOfPagedResultOfComplexTypeDto.Create(contract.PagedResultFieldNullableCollection)
            };

            message.PagedResultFieldCollection.AddRange(contract.PagedResultFieldCollection.Select(PagedResultOfComplexTypeDto.Create));
            return message;
        }
    }
}