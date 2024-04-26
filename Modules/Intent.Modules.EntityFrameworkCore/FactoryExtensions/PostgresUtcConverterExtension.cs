using System.Linq;
using System.Threading;
using Intent.Engine;
using Intent.Metadata.RDBMS.Api;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.Plugins;
using Intent.Modules.EntityFrameworkCore.Settings;
using Intent.Modules.Metadata.RDBMS.Settings;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.EntityFrameworkCore.FactoryExtensions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class PostgresUtcConverterExtension : FactoryExtensionBase
    {
        public override string Id => "Intent.EntityFrameworkCore.PostgresUtcConverterExtension";

        [IntentManaged(Mode.Ignore)]
        public override int Order => 0;

        protected override void OnAfterTemplateRegistrations(IApplication application)
        {
            if (!application.Settings.GetDatabaseSettings().DatabaseProvider().IsPostgresql())
            {
                return;
            }
            var classModels = application.MetadataManager.Domain(application).GetClassModels()
                .Where(x => x.InternalElement.Package.AsDomainPackageModel()?.HasRelationalDatabase() == true &&
                x.Attributes.Any(a => a.TypeReference?.Element.Name == "datetimeoffset")
                ).ToArray();

        }
    }
}