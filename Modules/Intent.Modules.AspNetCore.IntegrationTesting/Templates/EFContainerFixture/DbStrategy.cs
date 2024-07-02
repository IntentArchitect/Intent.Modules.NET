using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.VisualStudio;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intent.Modules.AspNetCore.IntegrationTesting.Templates.EFContainerFixture
{
    internal class DbStrategy
    {
        public DbStrategy(string containerType, 
            List<string> usings, 
            List<INugetPackageInfo> nuGetPackages, 
            IEnumerable<CSharpStatement> containerInitialization)
        {
            ContainerType = containerType;
            Usings = usings;
            NuGetPackages = nuGetPackages;
            ContainerInitialization = containerInitialization;
        }

        public string ContainerType { get; }
        public IEnumerable<string> Usings { get; }
        public IEnumerable<INugetPackageInfo> NuGetPackages { get; }
        public IEnumerable<CSharpStatement> ContainerInitialization { get;  }
    }
}
