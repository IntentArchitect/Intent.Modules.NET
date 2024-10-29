using System;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Constants;
using Intent.Modules.MongoDb.Templates;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.MongoDb.FactoryExtensions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class MultiTenanctFactoryExtension : FactoryExtensionBase
    {
        public override string Id => "Intent.MongoDb.MultiTenanctFactoryExtension";

        [IntentManaged(Mode.Ignore)]
        public override int Order => 0;

        protected override void OnAfterTemplateRegistrations(IApplication application)
        {
            if (!application.TemplateExists(TemplateRoles.Distribution.WebApi.MultiTenancyConfiguration))
            {
                return;
            }


            if (DocumentTemplateHelpers.IsSeparateDatabaseMultiTenancy(application.Settings))
            {
                return;
            }

            throw new Exception("MongoDb Currently only supports the Multi-tenancy `Data Isolation` option `Separate Database`. If you would like other options please contact us about adding additional options. (support@intentarchitect.com)");
        }
    }
}