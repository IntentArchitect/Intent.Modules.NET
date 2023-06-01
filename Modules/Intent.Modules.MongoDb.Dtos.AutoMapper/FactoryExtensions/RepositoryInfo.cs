using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;

namespace Intent.Modules.MongoDb.Dtos.AutoMapper.FactoryExtensions
{
    public partial class DtoAutoMapperFactoryExtension
    {
        internal class RepositoryInfo
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