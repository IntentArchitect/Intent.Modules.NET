using CosmosDB.EnumStrings.Application.Common.Interfaces;
using CosmosDB.EnumStrings.Domain;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace CosmosDB.EnumStrings.Application.RootEntities.UpdateRootEntity
{
    public class UpdateRootEntityCommand : IRequest, ICommand
    {
        public UpdateRootEntityCommand(string name,
            EnumExample enumExample,
            EnumExample? nullableEnumExample,
            UpdateRootEntityEmbeddedObjectDto embedded,
            string id)
        {
            Name = name;
            EnumExample = enumExample;
            NullableEnumExample = nullableEnumExample;
            Embedded = embedded;
            Id = id;
        }

        public string Name { get; set; }
        public EnumExample EnumExample { get; set; }
        public EnumExample? NullableEnumExample { get; set; }
        public UpdateRootEntityEmbeddedObjectDto Embedded { get; set; }
        public string Id { get; set; }
    }
}