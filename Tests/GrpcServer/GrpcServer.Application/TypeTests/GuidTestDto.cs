using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace GrpcServer.Application.TypeTests
{
    public class GuidTestDto
    {
        public GuidTestDto()
        {
            GuidFieldCollection = null!;
        }

        public Guid GuidField { get; set; }
        public List<Guid> GuidFieldCollection { get; set; }
        public Guid? GuidFieldNullable { get; set; }
        public List<Guid>? GuidFieldNullableCollection { get; set; }

        public static GuidTestDto Create(
            Guid guidField,
            List<Guid> guidFieldCollection,
            Guid? guidFieldNullable,
            List<Guid>? guidFieldNullableCollection)
        {
            return new GuidTestDto
            {
                GuidField = guidField,
                GuidFieldCollection = guidFieldCollection,
                GuidFieldNullable = guidFieldNullable,
                GuidFieldNullableCollection = guidFieldNullableCollection
            };
        }
    }
}