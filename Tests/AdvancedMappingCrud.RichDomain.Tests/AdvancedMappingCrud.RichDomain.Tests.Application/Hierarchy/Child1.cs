using AdvancedMappingCrud.RichDomain.Tests.Application.Common.Mappings;
using AdvancedMappingCrud.RichDomain.Tests.Domain.Entities;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace AdvancedMappingCrud.RichDomain.Tests.Application.Hierarchy
{
    public class Child1 : Parent1, IMapFrom<ChildSimple>
    {
        public Child1()
        {
            ChildName = null!;
        }

        public string ChildName { get; set; }

        public static Child1 Create(string parentName, string childName)
        {
            return new Child1
            {
                ParentName = parentName,
                ChildName = childName
            };
        }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<ChildSimple, Child1>();
        }
    }
}