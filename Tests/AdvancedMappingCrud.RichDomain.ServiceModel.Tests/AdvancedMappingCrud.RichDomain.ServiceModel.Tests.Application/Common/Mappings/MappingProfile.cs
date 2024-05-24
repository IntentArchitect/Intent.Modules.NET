using System;
using System.Linq;
using System.Reflection;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.AutoMapper.MappingProfile", Version = "1.0")]

namespace AdvancedMappingCrud.RichDomain.ServiceModel.Tests.Application.Common.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            ApplyMappingsFromAssembly(Assembly.GetExecutingAssembly());
        }

        private void ApplyMappingsFromAssembly(Assembly assembly)
        {
            var types = assembly.GetExportedTypes()
                .Where(t => t.GetInterfaces().Any(i =>
                    i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IMapFrom<>)))
                .ToList();

            foreach (var type in types)
            {
                var instance = Activator.CreateInstance(type, true);

                var methodInfo = type.GetMethod("Mapping")
                    ?? type.GetInterface("IMapFrom`1")?.GetMethod("Mapping");

                methodInfo?.Invoke(instance, new object[] { this });
            }
        }
    }
}