using Intent.Engine;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Templates;

namespace Intent.Modules.UserContext.Templates.UserContextInterface
{
    partial class UserContextInterfaceTemplate : CSharpTemplateBase<object>
    {
        public const string Identifier = "Intent.UserContext.UserContextInterface";

        public UserContextInterfaceTemplate(IOutputTarget outputTarget)
            : base(Identifier, outputTarget, null)
        {
        }

        protected override CSharpFileConfig DefineFileConfig()
        {
            return new CSharpFileConfig(
                className: "IUserContextData",
                @namespace: $"{this.GetNamespace()}",
                relativeLocation: string.Empty,
                overwriteBehaviour: OverwriteBehaviour.Always,
                fileName: "IUserContextData",
                fileExtension: "cs");
        }
    }
}
