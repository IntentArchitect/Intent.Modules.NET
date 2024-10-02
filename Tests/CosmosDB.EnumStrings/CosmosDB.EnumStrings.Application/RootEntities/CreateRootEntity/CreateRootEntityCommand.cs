using System.Collections.Generic;
using CosmosDB.EnumStrings.Application.Common.Interfaces;
using CosmosDB.EnumStrings.Domain;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace CosmosDB.EnumStrings.Application.RootEntities.CreateRootEntity
{
    public class CreateRootEntityCommand : IRequest<string>, ICommand
    {
        public CreateRootEntityCommand(string name,
            EnumExample enumExample,
            EnumExample? nullableEnumExample,
            CreateRootEntityEmbeddedObjectDto embedded,
            List<CreateRootEntityCommandNestedEntitiesDto> nestedEntities)
        {
            Name = name;
            EnumExample = enumExample;
            NullableEnumExample = nullableEnumExample;
            Embedded = embedded;
            NestedEntities = nestedEntities;
        }

        public string Name { get; set; }
        public EnumExample EnumExample { get; set; }
        public EnumExample? NullableEnumExample { get; set; }
        public CreateRootEntityEmbeddedObjectDto Embedded { get; set; }
        public List<CreateRootEntityCommandNestedEntitiesDto> NestedEntities { get; set; }
    }
}