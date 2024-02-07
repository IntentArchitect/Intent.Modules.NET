using Intent.Modules.Common.VisualStudio;

namespace Intent.Modules.Redis.Om.Repositories
{
    internal static class NugetDependencies
    {
        public static readonly INugetPackageInfo RedisOM = new NugetPackageInfo("Redis.OM", "0.6.1");
    }
}
