using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;
using System.Threading;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Metadata.RDBMS.Api;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using Intent.Modules.EntityFrameworkCore.Settings;
using Intent.Modules.Metadata.RDBMS.Settings;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.EntityFrameworkCore.FactoryExtensions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class LazyLoadingProxyFactoryExtension : FactoryExtensionBase
    {
        private readonly IMetadataManager _metadataManager;

        public LazyLoadingProxyFactoryExtension(IMetadataManager metadataManager)
        {
            _metadataManager = metadataManager;
        }

        public override string Id => "Intent.EntityFrameworkCore.LazyLoadingProxyFactoryExtension";

        [IntentManaged(Mode.Ignore)]
        public override int Order => 0;

        protected override void OnAfterTemplateRegistrations(IApplication application)
        {

            if (!application.Settings.GetDatabaseSettings().LazyLoadingWithProxies())
            {
                return;
            }
            var rdbmsModels = _metadataManager.Domain(application).GetClassModels()
                .Where(p => p.InternalElement.Package.AsDomainPackageModel()?.HasRelationalDatabase() == true)
                .ToList();

            foreach (var model in rdbmsModels)
            {
                ICSharpFileBuilderTemplate entityTetmplate = GetEntityTemplate(application, model);
                entityTetmplate.CSharpFile.OnBuild(file =>
                {
                    var @class = file.Classes.First();
                    foreach (var property in @class.Properties)
                    {
                        if (property.HasMetadata("model"))
                        {
                            var model = property.GetMetadata("model");
                            ITypeReference typeReference;
                            if (model is AttributeModel attributeModel)
                            {
                                typeReference = attributeModel.TypeReference;
                            }
                            else if (model is AssociationEndModel associationEndModel)
                            {
                                typeReference = associationEndModel;
                            }
                            else
                            {
                                continue;
                            }
                            if (typeReference.Element.IsClassModel())
                            {
                                property.Virtual();
                            }
                        }
                    }
                }, 1000);
            }
        }

        private ICSharpFileBuilderTemplate GetEntityTemplate(IApplication application, ClassModel model)
        {
            var entityTemplate = application.FindTemplateInstance<ICSharpFileBuilderTemplate>(TemplateRoles.Domain.Entity.EntityImplementation, model.Id);
            if (((IntentTemplateBase)entityTemplate).TryGetTemplate(TemplateRoles.Domain.Entity.State, model.Id, out ICSharpFileBuilderTemplate stateTempalate))
            {
                entityTemplate = stateTempalate;
            }
            return entityTemplate;
        }
    }
}