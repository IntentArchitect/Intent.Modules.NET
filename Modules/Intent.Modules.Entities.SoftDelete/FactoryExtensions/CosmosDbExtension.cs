using System.Linq;
using Intent.Engine;
using Intent.Entities.SoftDelete.Api;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using Intent.Modules.Entities.SoftDelete.Templates;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.Entities.SoftDelete.FactoryExtensions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class CosmosDbExtension : FactoryExtensionBase
    {
        public override string Id => "Intent.Entities.SoftDelete.CosmosDbExtension";

        [IntentManaged(Mode.Ignore)]
        public override int Order => 0;

        protected override void OnAfterTemplateRegistrations(IApplication application)
        {
            if (!application.FindTemplateInstances<IIntentTemplate>("Domain.UnitOfWork.CosmosDB").Any())
            {
                return;
            }
            
            InstallSoftDeleteOnEntities(application);
            InstallSoftDeleteOnEntityDocumentInterfaces(application);
        }

        private void InstallSoftDeleteOnEntities(IApplication application)
        {
            var entities = application
                .FindTemplateInstances<IIntentTemplate<ClassModel>>(
                    TemplateDependency.OnTemplate(TemplateRoles.Domain.Entity.Primary))
                .Where(p => p.Model.HasSoftDeleteEntity())
                .Cast<ICSharpFileBuilderTemplate>()
                .ToArray();
            foreach (var entity in entities)
            {
                entity.CSharpFile.OnBuild(file =>
                {
                    var priClass = file.Classes.First();
                    var softDeleteInterfaceName = entity.GetSoftDeleteInterfaceName();
                    if (priClass.Interfaces.All(x => x != softDeleteInterfaceName))
                    {
                        priClass.ImplementsInterface(softDeleteInterfaceName);
                    }
                });
            }
        }
        
        private void InstallSoftDeleteOnEntityDocumentInterfaces(IApplication application)
        {
            var docInterfaces = application.FindTemplateInstances<IIntentTemplate<ClassModel>>("Intent.CosmosDB.CosmosDBDocumentInterface")
                .Where(p => p.Model.HasSoftDeleteEntity())
                .Cast<ICSharpFileBuilderTemplate>()
                .ToArray();
            foreach (var docInterface in docInterfaces)
            {
                docInterface.CSharpFile.OnBuild(file =>
                {
                    var @interface = file.Interfaces.First();
                    var softDeleteInterfaceName = $"{docInterface.GetSoftDeleteInterfaceName()}ReadOnly";
                    @interface.ImplementsInterfaces(softDeleteInterfaceName);
                    var existingDeleteProperty = @interface.Properties.FirstOrDefault(p => p.Name == "IsDeleted");
                    if (existingDeleteProperty is not null)
                    {
                        @interface.Properties.Remove(existingDeleteProperty);
                    }
                }, 1001);
            }
        }
    }
}