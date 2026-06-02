using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.Integration.HttpClients.Fakes.FactoryHelpers", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.HttpClients.Infrastructure.HttpClients
{
    internal static class FactoryHelpers
    {
        public static T Configure<T>(T instance, Action<T> configure)
        {
            ArgumentNullException.ThrowIfNull(configure);
            configure(instance);
            return instance;
        }

        public static List<T> List<T>(Func<T> create, int count, Action<T, int>? configure = null)
        {
            ArgumentNullException.ThrowIfNull(create);
            var list = new List<T>(count);
            var index = 0;

            while (index < count)
            {
                var dto = create();
                configure?.Invoke(dto, index);
                list.Add(dto);
                index++;
            }
            return list;
        }
    }
}