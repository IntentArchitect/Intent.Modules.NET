using Intent.Metadata.Models;
using Intent.Modules.Common.CSharp.Interactions;

namespace Intent.Modules.Application.DomainInteractions.Extensions;

public record AggregateDetails : EntityDetails
{
    public AggregateDetails(
        IElement ElementModel,
        string VariableName,
        IDataAccessProvider DataAccessProvider,
        bool AssociationToOwnedEntityIsCollection)
        : base(
            ElementModel,
            VariableName,
            DataAccessProvider,
            false)
    {
        this.AssociationToOwnedEntityIsCollection = AssociationToOwnedEntityIsCollection;
    }

    public bool AssociationToOwnedEntityIsCollection { get; }
}