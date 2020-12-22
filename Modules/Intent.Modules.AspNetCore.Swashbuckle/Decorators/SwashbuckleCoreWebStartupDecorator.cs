using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Intent.Modules.AspNetCore.Templates.Startup;
using Intent.Modules.Common;
using Intent.Modules.Common.VisualStudio;

namespace Intent.Modules.AspNetCore.Swashbuckle.Decorators
{
    public class SwashbuckleCoreWebStartupDecorator : StartupDecorator, IHasNugetDependencies, IDeclareUsings
    {
        public const string Identifier = "Intent.AspNetCore.Swashbuckle.StartupDecorator";
        public SwashbuckleCoreWebStartupDecorator()
        {
        }

        public IEnumerable<INugetPackageInfo> GetNugetDependencies()
        {
            return new[]
            {
                NugetPackages.SwashbuckleAspNetCore,
            };
        }

        public IEnumerable<string> DeclareUsings()
        {
            yield return "Microsoft.OpenApi.Models";
        }
    }
}
