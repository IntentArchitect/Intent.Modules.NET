
namespace Intent.Modules.MongoDb.Dtos.AutoMapper.FactoryExtensions
{
    public partial class DtoAutoMapperFactoryExtension
    {
        internal class PathAggregate
        {
            public PathAggregate(int index, string expression)
            {
                Index = index;
                Expression = expression;
            }

            public int Index { get; }
            public string Expression { get; }
        }

    }
}