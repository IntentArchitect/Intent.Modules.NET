using Intent.RoslynWeaver.Attributes;
using MediatR;
using Redis.Om.Repositories.Application.Common.Interfaces;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace Redis.Om.Repositories.Application.DerivedTypes.CreateDerivedType
{
    public class CreateDerivedTypeCommand : IRequest<string>, ICommand
    {
        public CreateDerivedTypeCommand(string derivedName,
            string baseName,
            CreateDerivedTypeDerivedTypeAggregateDto derivedTypeAggregate)
        {
            DerivedName = derivedName;
            BaseName = baseName;
            DerivedTypeAggregate = derivedTypeAggregate;
        }

        public string DerivedName { get; set; }
        public string BaseName { get; set; }

        public CreateDerivedTypeDerivedTypeAggregateDto DerivedTypeAggregate { get; set; }
    }
}