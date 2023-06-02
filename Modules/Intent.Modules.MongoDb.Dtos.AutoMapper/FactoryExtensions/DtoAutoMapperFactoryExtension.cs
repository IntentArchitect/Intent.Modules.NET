using Intent.Engine;
using Intent.Modules.Common.Plugins;
using Intent.Modules.DocumentDB.Dtos.AutoMapper.Shared;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.MongoDb.Dtos.AutoMapper.FactoryExtensions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class DtoAutoMapperFactoryExtension : FactoryExtensionBase
    {
        public override string Id => "Intent.MongoDb.Dtos.AutoMapper.DtoAutoMapperFactoryExtension";

        [IntentManaged(Mode.Ignore)]
        public override int Order => 0;

        protected override void OnAfterTemplateRegistrations(IApplication application)
        {
            CrossAggregateMappingConfigurator.Execute(application);
        }
    }
}