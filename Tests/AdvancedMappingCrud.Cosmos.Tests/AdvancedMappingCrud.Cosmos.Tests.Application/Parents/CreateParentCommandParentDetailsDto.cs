using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace AdvancedMappingCrud.Cosmos.Tests.Application.Parents
{
    public class CreateParentCommandParentDetailsDto
    {
        public CreateParentCommandParentDetailsDto()
        {
            DetailsLine1 = null!;
            DetailsLine2 = null!;
        }

        public string DetailsLine1 { get; set; }
        public string DetailsLine2 { get; set; }
        public List<CreateParentCommandParentDetailsTagsDto>? ParentDetailsTags { get; set; }
        public CreateParentCommandParentSubDetailsDto? ParentSubDetails { get; set; }

        public static CreateParentCommandParentDetailsDto Create(
            string detailsLine1,
            string detailsLine2,
            List<CreateParentCommandParentDetailsTagsDto>? parentDetailsTags,
            CreateParentCommandParentSubDetailsDto? parentSubDetails)
        {
            return new CreateParentCommandParentDetailsDto
            {
                DetailsLine1 = detailsLine1,
                DetailsLine2 = detailsLine2,
                ParentDetailsTags = parentDetailsTags,
                ParentSubDetails = parentSubDetails
            };
        }
    }
}