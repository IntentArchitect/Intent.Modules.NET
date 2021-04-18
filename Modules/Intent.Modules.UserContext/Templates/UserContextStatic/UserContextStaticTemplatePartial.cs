using Intent.Engine;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;

namespace Intent.Modules.UserContext.Templates.UserContextStatic
{
    partial class UserContextStaticTemplate : CSharpTemplateBase<object>
    {
        public const string Identifier = "Intent.UserContext.UserContextStatic";

        public UserContextStaticTemplate(IOutputTarget outputTarget)
            : base(Identifier, outputTarget, null)
        {
        }

        protected override CSharpFileConfig DefineFileConfig()
        {
            return new CSharpFileConfig(
                className: "UserContext",
                @namespace: $"{this.GetNamespace()}");
        }
    }
}
