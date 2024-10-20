using System.Linq;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Constants;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.SharedKernel.FactoryExtensions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class DbContextExtension : FactoryExtensionBase
    {
        public override string Id => "Intent.SharedKernel.DbContextExtension";

        [IntentManaged(Mode.Ignore)]
        public override int Order => 0;

        protected override void OnAfterTemplateRegistrations(IApplication application)
        {
            var dbContextTemplate = application.FindTemplateInstance<ICSharpFileBuilderTemplate>(TemplateRoles.Infrastructure.Data.DbContext);
            if (dbContextTemplate is null)
            {
                return;
            }

            dbContextTemplate.CSharpFile.OnBuild(file =>
            {
                var @class = file.Classes.First();
                @class.AddConstructor(ctor =>
                {
                    ctor.Protected();
                    ctor.AddParameter("DbContextOptions", "options");
                    ctor.AddParameter("IDomainEventService", "domainEventService");
                    ctor.CallsBase(x => x.AddArgument("options"));
                    ctor.AddStatement("_domainEventService = domainEventService;");
                });
            });
        }
    }
}