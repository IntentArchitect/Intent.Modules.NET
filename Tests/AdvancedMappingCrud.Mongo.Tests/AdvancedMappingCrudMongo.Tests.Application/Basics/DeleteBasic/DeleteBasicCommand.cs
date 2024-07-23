using AdvancedMappingCrudMongo.Tests.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace AdvancedMappingCrudMongo.Tests.Application.Basics.DeleteBasic
{
    public class DeleteBasicCommand : IRequest, ICommand
    {
        public DeleteBasicCommand(string id)
        {
            Id = id;
        }

        public string Id { get; set; }
    }
}