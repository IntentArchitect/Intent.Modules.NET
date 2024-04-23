using Intent.Metadata.Models;
using Intent.Modelers.Services.Api;

namespace Intent.Modules.DocumentDB.Dtos.AutoMapper.CrossAggregateMappingConfigurator
{
    internal static partial class CrossAggregateMappingConfigurator
    {
        private class CrossAggregateMappedField
        {
            public CrossAggregateMappedField(DTOFieldModel field, IAssociationEnd mappedFrom, int pathIndex)
            {
                Field = field;
                MappedFrom = mappedFrom;
                PathIndex = pathIndex;
            }

            public DTOFieldModel Field { get; }
            public IAssociationEnd MappedFrom { get; }
            public int PathIndex { get; }
        }
    }
}