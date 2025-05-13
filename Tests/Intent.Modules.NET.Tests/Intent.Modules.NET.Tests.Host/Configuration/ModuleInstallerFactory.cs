using Intent.Modules.NET.Tests.Infrastructure.Core.Interfaces;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModularMonolith.Host.ModuleConfiguration", Version = "1.0")]

namespace Intent.Modules.NET.Tests.Host.Configuration
{
    [IntentMerge]
    [DefaultIntentManaged(Mode.Merge, Targets = Targets.Methods)]
    public static class ModuleInstallerFactory
    {
        public static IEnumerable<IModuleInstaller> GetModuleInstallers()
        {
            var result = new List<IModuleInstaller>();
            result.Add(new Intent.Modules.NET.Tests.Module1.Api.ModuleInstaller());
            result.Add(new Intent.Modules.NET.Tests.Module2.Api.ModuleInstaller());
            return result;
        }
    }
}