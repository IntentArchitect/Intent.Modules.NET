namespace Intent.Modules.DocumentDB.Dtos.AutoMapper.Shared
{
    public static partial class CrossAggregateMappingConfigurator
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