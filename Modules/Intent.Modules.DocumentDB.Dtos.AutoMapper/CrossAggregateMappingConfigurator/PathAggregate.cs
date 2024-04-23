namespace Intent.Modules.DocumentDB.Dtos.AutoMapper.CrossAggregateMappingConfigurator
{
    internal static partial class CrossAggregateMappingConfigurator
    {
        private class PathAggregate
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