using Intent.Metadata.Models;
using Intent.Modelers.Services.Api;

namespace Intent.Modules.MongoDb.Dtos.AutoMapper.FactoryExtensions
{
    public partial class DtoAutoMapperFactoryExtension
    {
        internal class CrossAggregateMappedField
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