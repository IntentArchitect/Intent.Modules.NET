using Intent.Modules.Common.Templates;

namespace Intent.Modules.DocumentDB.Dtos.AutoMapper.CrossAggregateMappingConfigurator
{
    internal static partial class CrossAggregateMappingConfigurator
    {
        private class RepositoryInfo
        {
            public string InterfaceName { get; }
            public string FieldName { get; }
            public string ArgumentName { get; }

            public RepositoryInfo(string @interface)
            {
                InterfaceName = @interface;
                ArgumentName = InterfaceName.Substring(1).ToCamelCase();
                FieldName = $"_{ArgumentName}";
            }

            public override bool Equals(object obj)
            {
                return InterfaceName.Equals(((RepositoryInfo)obj).InterfaceName);
            }

            public override int GetHashCode()
            {
                return InterfaceName.GetHashCode();
            }
        }

    }
}