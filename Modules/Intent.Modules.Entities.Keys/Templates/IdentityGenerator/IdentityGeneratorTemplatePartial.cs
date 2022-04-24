using Intent.Engine;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Templates;

namespace Intent.Modules.Entities.Keys.Templates.IdentityGenerator
{
    partial class IdentityGeneratorTemplate : CSharpTemplateBase<object>, ITemplate
    {
        public const string Identifier = "Intent.Entities.Keys.IdentityGenerator";

        public IdentityGeneratorTemplate(IOutputTarget project)
            : base(Identifier, project, null)
        {
            AddNugetDependency("RT.Comb", "2.3.0");
        }

        protected override CSharpFileConfig DefineFileConfig()
        {
            return new CSharpFileConfig(
                className: "IdentityGenerator",
                @namespace: $"{OutputTarget.GetNamespace()}");
        }
    }
}
