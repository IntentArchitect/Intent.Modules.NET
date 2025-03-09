using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace GrpcServer.Application.TypeTests
{
    public class ComplexValueTestDto
    {
        public ComplexValueTestDto()
        {
            ComplexTypeField = null!;
            ComplexTypeFieldCollection = null!;
        }

        public ComplexTypeDto ComplexTypeField { get; set; }
        public List<ComplexTypeDto> ComplexTypeFieldCollection { get; set; }
        public ComplexTypeDto? ComplexTypeFieldNullable { get; set; }
        public List<ComplexTypeDto>? ComplexTypeFieldNullableCollection { get; set; }

        public static ComplexValueTestDto Create(
            ComplexTypeDto complexTypeField,
            List<ComplexTypeDto> complexTypeFieldCollection,
            ComplexTypeDto? complexTypeFieldNullable,
            List<ComplexTypeDto>? complexTypeFieldNullableCollection)
        {
            return new ComplexValueTestDto
            {
                ComplexTypeField = complexTypeField,
                ComplexTypeFieldCollection = complexTypeFieldCollection,
                ComplexTypeFieldNullable = complexTypeFieldNullable,
                ComplexTypeFieldNullableCollection = complexTypeFieldNullableCollection
            };
        }
    }
}