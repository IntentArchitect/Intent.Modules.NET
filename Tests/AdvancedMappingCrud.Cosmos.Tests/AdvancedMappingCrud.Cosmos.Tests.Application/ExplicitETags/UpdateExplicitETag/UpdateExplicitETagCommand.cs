using AdvancedMappingCrud.Cosmos.Tests.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace AdvancedMappingCrud.Cosmos.Tests.Application.ExplicitETags.UpdateExplicitETag
{
    public class UpdateExplicitETagCommand : IRequest, ICommand
    {
        public UpdateExplicitETagCommand(string name, string? eTag, string id)
        {
            Name = name;
            ETag = eTag;
            Id = id;
        }

        public string Name { get; set; }
        public string? ETag { get; set; }
        public string Id { get; set; }
    }
}