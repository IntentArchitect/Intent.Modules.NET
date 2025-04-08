using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTesting.DtoContract", Version = "2.0")]

namespace AdvancedMappingCrud.Cosmos.Tests.IntegrationTests.Services.Parents
{
    public class UpdateParentCommandParentDetailsDto
    {
        public UpdateParentCommandParentDetailsDto()
        {
            DetailsLine1 = null!;
            DetailsLine2 = null!;
        }

        public string DetailsLine1 { get; set; }
        public string DetailsLine2 { get; set; }
        public List<UpdateParentCommandParentDetailsTagsDto>? ParentDetailsTags { get; set; }
        public UpdateParentCommandParentSubDetailsDto? ParentSubDetails { get; set; }

        public static UpdateParentCommandParentDetailsDto Create(
            string detailsLine1,
            string detailsLine2,
            List<UpdateParentCommandParentDetailsTagsDto>? parentDetailsTags,
            UpdateParentCommandParentSubDetailsDto? parentSubDetails)
        {
            return new UpdateParentCommandParentDetailsDto
            {
                DetailsLine1 = detailsLine1,
                DetailsLine2 = detailsLine2,
                ParentDetailsTags = parentDetailsTags,
                ParentSubDetails = parentSubDetails
            };
        }
    }
}