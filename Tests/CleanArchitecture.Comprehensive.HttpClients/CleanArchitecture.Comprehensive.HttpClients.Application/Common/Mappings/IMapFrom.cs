using AutoMapper;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.AutoMapper.MapFromInterface", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.HttpClients.Application.Common.Mappings
{
    internal interface IMapFrom<T>
    {
        void Mapping(Profile profile);
    }
}