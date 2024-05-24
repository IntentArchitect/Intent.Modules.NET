using System.Reflection;
using AutoFixture.Kernel;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.IntegrationTests.CRUD.PopulateIdsSpecimenBuilder", Version = "1.0")]

namespace ValueObjects.Class.IntegrationTests
{
    public class PopulateIdsSpecimenBuilder : ISpecimenBuilder
    {
        private readonly Dictionary<string, object> _idsToReplace;

        public PopulateIdsSpecimenBuilder(Dictionary<string, object> idsToReplace)
        {
            _idsToReplace = idsToReplace;
        }

        public object Create(object request, ISpecimenContext context)
        {
            var propertyInfo = request as PropertyInfo;

            if (propertyInfo != null)
            {
                if (_idsToReplace.TryGetValue(propertyInfo.Name, out object? value))
                {
                    return value;
                }

                if (propertyInfo.Name.EndsWith("Id"))
                {
                    return new OmitSpecimen();
                }
            }
            return new NoSpecimen();
        }
    }
}