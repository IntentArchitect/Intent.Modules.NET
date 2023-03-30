using Intent.Engine;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.Application.Dtos.AutoMapper.FactoryExtentions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class AutoMapperDtoFactoryExtension : FactoryExtensionBase
    {
        public override string Id => "Intent.Application.Dtos.AutoMapper.AutoMapperDtoFactoryExtension";

        [IntentManaged(Mode.Ignore)]
        public override int Order => 0;

        protected override void OnAfterTemplateRegistrations(IApplication application)
        {
            var templates = application.FindTemplateInstances<ICSharpFileBuilderTemplate>(TemplateDependency.OnTemplate(TemplateFulfillingRoles.Domain.Entity.Primary));
            foreach (var template in templates)
            {
                template.CSharpFile.AfterBuild(file =>
                {

                });
            }
        }
    }
}