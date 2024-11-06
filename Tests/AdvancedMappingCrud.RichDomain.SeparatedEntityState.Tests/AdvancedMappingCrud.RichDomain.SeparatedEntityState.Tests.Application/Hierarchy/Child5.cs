using System;
using AdvancedMappingCrud.RichDomain.SeparatedEntityState.Tests.Application.Common.Mappings;
using AdvancedMappingCrud.RichDomain.SeparatedEntityState.Tests.Domain.Entities;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace AdvancedMappingCrud.RichDomain.SeparatedEntityState.Tests.Application.Hierarchy
{
    public class Child5 : Parent5, IMapFrom<FamilyComplex>
    {
        public Child5()
        {
            ChildName = null!;
        }

        public Guid Id { get; set; }
        public string ChildName { get; set; }

        public static Child5 Create(
            long greatGrandParentId,
            string greatGrandParentName,
            long grandparentId,
            int parentId,
            string parentName,
            Aunt5 aunt,
            Guid id,
            string childName)
        {
            return new Child5
            {
                GreatGrandParentId = greatGrandParentId,
                GreatGrandParentName = greatGrandParentName,
                GrandparentId = grandparentId,
                ParentId = parentId,
                ParentName = parentName,
                Aunt = aunt,
                Id = id,
                ChildName = childName
            };
        }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<FamilyComplex, Child5>();
        }
    }
}