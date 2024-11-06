using AdvancedMappingCrud.RichDomain.SeparatedEntityState.Tests.Application.Common.Mappings;
using AdvancedMappingCrud.RichDomain.SeparatedEntityState.Tests.Domain.Entities;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace AdvancedMappingCrud.RichDomain.SeparatedEntityState.Tests.Application.Hierarchy
{
    public class Child2 : Parent2, IMapFrom<ChildParentExcluded>
    {
        public Child2()
        {
            ChildName = null!;
        }

        public string ChildName { get; set; }

        public static Child2 Create(int parentAge, string childName)
        {
            return new Child2
            {
                ParentAge = parentAge,
                ChildName = childName
            };
        }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<ChildParentExcluded, Child2>();
        }
    }
}