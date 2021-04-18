using System.Collections.Generic;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.UserContext.Templates.UserContextInterface;
using Intent.Templates;

namespace Intent.Modules.UserContext.Templates.UserContextProviderInterface
{
    partial class UserContextProviderInterfaceTemplate : CSharpTemplateBase<object>
    {
        public const string Identifier = "Intent.UserContext.UserContextProviderInterface";


        public UserContextProviderInterfaceTemplate(IOutputTarget outputTarget)
            : base(Identifier, outputTarget, null)
        {
        }

        protected override CSharpFileConfig DefineFileConfig()
        {
            return new CSharpFileConfig(
                className: "IUserContextProvider",
                @namespace: $"{this.GetNamespace()}",
                relativeLocation: string.Empty,
                overwriteBehaviour: OverwriteBehaviour.Always,
                fileName: "IUserContextProvider",
                fileExtension: "cs");
        }

        public override IEnumerable<ITemplateDependency> GetTemplateDependencies()
        {
            return new List<ITemplateDependency>
            {
                TemplateDependency.OnTemplate(UserContextInterfaceTemplate.Identifier),
            };
        }
    }
}
