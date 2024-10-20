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

namespace Intent.Modules.SharedKernel.Consumer.FactoryExtensions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class DbContextExtension : FactoryExtensionBase
    {
        public override string Id => "Intent.SharedKernel.Consumer.DbContextExtension";

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
                var sharedKernel = TemplateHelper.GetSharedKernel();

                var @class = file.Classes.First();
                @class.WithBaseType($"{sharedKernel.ApplicationName}.Infrastructure.Persistence.ApplicationDbContext");

                var ctor = @class.Constructors.First();
                if (ctor is not null)
                {
                    ctor.ConstructorCall.AddArgument("domainEventService");
                }
            });

        }
    }
}