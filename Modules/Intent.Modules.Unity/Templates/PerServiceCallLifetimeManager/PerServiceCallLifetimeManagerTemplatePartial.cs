using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.VisualStudio;
using Intent.Templates;

namespace Intent.Modules.Unity.Templates.PerServiceCallLifetimeManager
{
    partial class PerServiceCallLifetimeManagerTemplate : CSharpTemplateBase<object>, ITemplate, IHasNugetDependencies
    {
        public const string Identifier = "Intent.Unity.PerServiceCallLifetimeManager";

        public PerServiceCallLifetimeManagerTemplate(IOutputTarget outputTarget)
            : base(Identifier, outputTarget, null)
        {
        }

        protected override CSharpFileConfig DefineFileConfig()
        {
            return new CSharpFileConfig(
                className: "PerServiceCallLifetimeManager",
                @namespace: this.GetNamespace(),
                relativeLocation: this.GetFolderPath());
        }

        public override IEnumerable<INugetPackageInfo> GetNugetDependencies()
        {
            return new[]
            {
                NugetPackages.IntentFrameworkCore
            }
            .Union(base.GetNugetDependencies())
            .ToArray();
        }
    }
}
