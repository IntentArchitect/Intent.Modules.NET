using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using Intent.Modules.EntityFrameworkCore.Settings;
using Intent.Modules.EntityFrameworkCore.Templates;
using Intent.Modules.Metadata.RDBMS.Settings;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.EntityFrameworkCore.FactoryExtensions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class ConfigureConventionsExtension : FactoryExtensionBase
    {
        public override string Id => "Intent.EntityFrameworkCore.ConfigureConventionsExtension";

        [IntentManaged(Mode.Ignore)]
        public override int Order => 0;

        /// <summary>
        /// This is an example override which would extend the
        /// <see cref="ExecutionLifeCycleSteps.AfterTemplateRegistrations"/> phase of the Software Factory execution.
        /// See <see cref="FactoryExtensionBase"/> for all available overrides.
        /// </summary>
        /// <remarks>
        /// It is safe to update or delete this method.
        /// </remarks>
        protected override void OnAfterTemplateRegistrations(IApplication application)
        {
            AddConfigureConventions(application);
        }

        private void AddConfigureConventions(IApplication application)
        {
            if (!HasConventions(application))
            {
                return;
            }

            var dbContext = application.FindTemplateInstance<ICSharpFileBuilderTemplate>(TemplateDependency.OnTemplate(TemplateRoles.Infrastructure.Data.DbContext));
            if (dbContext is null)
            {
                return;
            }

            var conventions = new List<CSharpStatement>();

            if (dbContext.ExecutionContext.Settings.GetDatabaseSettings().StoreEnumsAsStrings())
            {
                conventions.Add(GetStoreEnumsAsStringsConvention(dbContext));
            }
            if (application.Settings.GetDatabaseSettings().DatabaseProvider().IsPostgresql())
            {
                conventions.Add(GetPostgresDateTimeOffsetConvention(dbContext));
            }

            dbContext.CSharpFile.OnBuild(file =>
            {
                var @class = file.Classes.First();
                @class.AddMethod("void", "ConfigureConventions", method =>
                {
                    method.Protected();
                    method.Override();
                    method.AddParameter("ModelConfigurationBuilder", "configurationBuilder");
                    method.AddStatement("base.ConfigureConventions(configurationBuilder);");
                    method.AddStatements(conventions);
                });
            });
        }

        public CSharpStatement GetStoreEnumsAsStringsConvention(ICSharpFileBuilderTemplate template)
        {
            return $"configurationBuilder.Properties(typeof({template.UseType("System.Enum")})).HaveConversion<string>();";
        }
        public CSharpStatement GetPostgresDateTimeOffsetConvention(ICSharpFileBuilderTemplate template)
        {
            return $"configurationBuilder.Properties(typeof({template.UseType("System.DateTimeOffset")})).HaveConversion(typeof({template.GetUtcDateTimeOffsetConverterName()}));";
        }

        private bool HasConventions(IApplication application)
        {
            return application.Settings.GetDatabaseSettings().StoreEnumsAsStrings() ||
                application.Settings.GetDatabaseSettings().DatabaseProvider().IsPostgresql();
        }
    }
}