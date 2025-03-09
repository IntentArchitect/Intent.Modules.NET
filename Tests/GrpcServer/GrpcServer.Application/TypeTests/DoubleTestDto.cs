using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace GrpcServer.Application.TypeTests
{
    public class DoubleTestDto
    {
        public DoubleTestDto()
        {
            DoubleFieldCollection = null!;
        }

        public double DoubleField { get; set; }
        public List<double> DoubleFieldCollection { get; set; }
        public double? DoubleFieldNullable { get; set; }
        public List<double>? DoubleFieldNullableCollection { get; set; }

        public static DoubleTestDto Create(
            double doubleField,
            List<double> doubleFieldCollection,
            double? doubleFieldNullable,
            List<double>? doubleFieldNullableCollection)
        {
            return new DoubleTestDto
            {
                DoubleField = doubleField,
                DoubleFieldCollection = doubleFieldCollection,
                DoubleFieldNullable = doubleFieldNullable,
                DoubleFieldNullableCollection = doubleFieldNullableCollection
            };
        }
    }
}