using System.Collections.Generic;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.DependencyInjection;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.UserContext.Templates.UserContextInterface;
using Intent.Modules.UserContext.Templates.UserContextProviderInterface;
using Intent.Templates;

namespace Intent.Modules.UserContext.Templates.UserContextProvider
{
    partial class UserContextProviderTemplate : CSharpTemplateBase<object>
    {
        public const string Identifier = "Intent.UserContext.UserContextProvider";

        public UserContextProviderTemplate(IOutputTarget outputTarget)
            : base(Identifier, outputTarget, null)
        {
        }

        protected override CSharpFileConfig DefineFileConfig()
        {
            return new CSharpFileConfig(
                className: "UserContextProvider",
                @namespace: $"{this.GetNamespace()}",
                relativeLocation: string.Empty,
                overwriteBehaviour: OverwriteBehaviour.Always,
                fileName: "UserContextProvider",
                fileExtension: "cs");
        }

        public override IEnumerable<ITemplateDependency> GetTemplateDependencies()
        {
            return new List<ITemplateDependency>
            {
                TemplateDependency.OnTemplate(UserContextInterfaceTemplate.Identifier)
            };
        }

        public override void BeforeTemplateExecution()
        {
            var userContextProviderInterface = Project.FindTemplateInstance<IClassProvider>(UserContextProviderInterfaceTemplate.Identifier);
            var contractTemplate = Project.FindTemplateInstance<IClassProvider>(UserContextInterfaceTemplate.Identifier);
            ExecutionContext.EventDispatcher.Publish(ContainerRegistrationRequest.ToRegister(this)
                .ForInterface($"{userContextProviderInterface.FullTypeName()}<{contractTemplate.FullTypeName()}>"));
        }
    }
}
