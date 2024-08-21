using System.Linq;
using System.Reflection;
using System.Threading;
using Intent.Engine;
using Intent.Metadata.DocumentDB.Api;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Common.Templates;
using Intent.Modules.Entities.Repositories.Api.Templates.EntityRepositoryInterface;
using Intent.Modules.Modelers.Domain.Settings;
using Intent.Modules.MongoDb.Repositories.Templates;
using Intent.Modules.MongoDb.Repositories.Templates.MongoRepositoryInterface;
using Intent.Modules.MongoDb.Templates;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.MongoDb.Repositories.FactoryExtensions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class MongoRepoFactoryExtension : FactoryExtensionBase
    {
        public override string Id => "Intent.MongoDb.Repositories.MongoRepoFactoryExtension";

        [IntentManaged(Mode.Ignore)]
        public override int Order => 0;

        protected override void OnAfterTemplateRegistrations(IApplication application)
        {
            UpdateRepositoryInterfaceTemplate(application);
        }

        private static void UpdateRepositoryInterfaceTemplate(IApplication application)
        {
            var createEntityInterfaces = application.Settings.GetDomainSettings().CreateEntityInterfaces();

            var repositoryTemplates = application.FindTemplateInstances<ICSharpFileBuilderTemplate>(TemplateDependency.OnTemplate(EntityRepositoryInterfaceTemplate.TemplateId));
            foreach (var repositoryTemplate in repositoryTemplates)
            {
                repositoryTemplate.CSharpFile.OnBuild(file =>
                {
                    file.AddUsing("System.Linq.Expressions");
                    var @interface = file.Interfaces.First();
                    var model = @interface.GetMetadata<ClassModel>("model");

                    if (MongoDbProvider.FilterDbProvider(model) == false)
                    {
                        return;
                    }

                    @interface.AddMethod($"List<{repositoryTemplate.GetTypeName("Domain.Entity.Interface", model)}>", "SearchText", method =>
                    {
                        method.AddAttribute(CSharpIntentManagedAttribute.Fully());
                        method.AddParameter("string", "searchText");
                        method.AddParameter($"Expression<Func<{repositoryTemplate.GetTypeName("Domain.Entity", model)}, bool>>", "filterExpression", param => param.WithDefaultValue("null"));
                    });

                    @interface.AddMethod("void", "Update", method =>
                    {
                        method.AddAttribute(CSharpIntentManagedAttribute.Fully());
                        method.AddParameter(repositoryTemplate.GetTypeName("Domain.Entity.Interface", model), "entity");
                    });
                }, 0);
            }
        }
    }
}