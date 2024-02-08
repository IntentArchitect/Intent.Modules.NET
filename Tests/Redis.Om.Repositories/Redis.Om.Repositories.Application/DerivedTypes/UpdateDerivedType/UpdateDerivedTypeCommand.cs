using Intent.RoslynWeaver.Attributes;
using MediatR;
using Redis.Om.Repositories.Application.Common.Interfaces;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace Redis.Om.Repositories.Application.DerivedTypes.UpdateDerivedType
{
    public class UpdateDerivedTypeCommand : IRequest, ICommand
    {
        public UpdateDerivedTypeCommand(string id,
            string derivedName,
            string baseName,
            UpdateDerivedTypeDerivedTypeAggregateDto derivedTypeAggregate)
        {
            Id = id;
            DerivedName = derivedName;
            BaseName = baseName;
            DerivedTypeAggregate = derivedTypeAggregate;
        }

        public string Id { get; set; }
        public string DerivedName { get; set; }
        public string BaseName { get; set; }
        public UpdateDerivedTypeDerivedTypeAggregateDto DerivedTypeAggregate { get; set; }
    }
}