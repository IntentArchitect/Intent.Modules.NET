using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.Plugins;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.DocumentDB.Dtos.AutoMapper.FactoryExtensions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class DtoAutoMapperFactoryExtension : FactoryExtensionBase
    {
        public override string Id => "Intent.DocumentDB.Dtos.AutoMapper.DtoAutoMapperFactoryExtension";

        [IntentManaged(Mode.Ignore)]
        public override int Order => 0;

        protected override void OnAfterTemplateRegistrations(IApplication application)
        {
            CrossAggregateMappingConfigurator.CrossAggregateMappingConfigurator.Execute(application);
        }
    }
}