using System;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.MultiKeyParents
{
    public class UpdateMultiKeyParentCommandMultiKeyChildrenDto
    {
        public UpdateMultiKeyParentCommandMultiKeyChildrenDto()
        {
            ChildName = null!;
            RefNo = null!;
        }

        public string ChildName { get; set; }
        public string RefNo { get; set; }
        public Guid Id { get; set; }

        public static UpdateMultiKeyParentCommandMultiKeyChildrenDto Create(string childName, string refNo, Guid id)
        {
            return new UpdateMultiKeyParentCommandMultiKeyChildrenDto
            {
                ChildName = childName,
                RefNo = refNo,
                Id = id
            };
        }
    }
}