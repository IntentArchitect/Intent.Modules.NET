using System;
using System.Reflection;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Redis.Om.Repositories.Templates.ReflectionHelper", Version = "1.0")]

namespace Redis.Om.Repositories.Infrastructure.Persistence.Documents
{
    internal static class ReflectionHelper
    {
        public static T CreateNewInstanceOf<T>()
        {
            var constructorInfo = typeof(T).GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, Array.Empty<Type>(), null)!;
            var instance = (T)constructorInfo.Invoke(Array.Empty<object>());
            return instance;
        }

        public static void ForceSetProperty<T>(T instance, string propertyName, object? value)
        {
            var propertyInfo = typeof(T).GetProperty(propertyName)!;
            propertyInfo = propertyInfo.DeclaringType!.GetProperty(propertyName)!;
            propertyInfo.SetValue(instance, value, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, null, null);
        }
    }
}