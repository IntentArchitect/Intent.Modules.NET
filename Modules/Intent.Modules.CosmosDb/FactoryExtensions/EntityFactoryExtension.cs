using System;
using System.Collections.Generic;
using System.Linq;
using Intent.CosmosDb.Api;
using Intent.Engine;
using Intent.Metadata.RDBMS.Api;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.TypeResolution;
using Intent.Modules.Constants;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;
using Intent.Utils;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.CosmosDb.FactoryExtensions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class EntityFactoryExtension : FactoryExtensionBase
    {
        public override string Id => "Intent.CosmosDb.EntityFactoryExtension";

        [IntentManaged(Mode.Ignore)]
        public override int Order => 0;

        protected override void OnAfterTemplateRegistrations(IApplication application)
        {
            var templates = application.FindTemplateInstances<ICSharpFileBuilderTemplate>(TemplateDependency.OnTemplate(TemplateFulfillingRoles.Domain.Entity.Primary));
            foreach (var template in templates)
            {
                var templateModel = ((CSharpTemplateBase<ClassModel>)template).Model;
                if (!templateModel.InternalElement.Package.IsCosmosDbDomainPackageModel())
                {
                    continue;
                }

                LogAggregateRelationshipsAsError(templateModel);

                template.CSharpFile.OnBuild(file =>
                {
                    file.AddUsing("System");
                    var @class = file.Classes.First();
                    var model = @class.GetMetadata<ClassModel>("model");

                    var pks = model.GetExplicitPrimaryKey();
                    if (!pks.Any())
                    {
                        throw new Exception($"No Id has been specified for {@class.Name}");
                    }
                    var primaryKeyProperties = new List<CSharpProperty>();
                    foreach (var attribute in pks)
                    {
                        var existingPk = @class.GetAllProperties().FirstOrDefault(x => x.Name.Equals(attribute.Name, StringComparison.InvariantCultureIgnoreCase));
                        if (!model.IsAggregateRoot())
                        {
                            @class.AddField(template.GetTypeName(attribute.TypeReference) + "?", "_id");
                        }
                        if (!model.IsAggregateRoot())
                        {
                            existingPk.Getter.WithExpressionImplementation($"_id ??= Guid.NewGuid()");
                            existingPk.Setter.WithExpressionImplementation($"_id = value");
                        }

                        primaryKeyProperties.Add(existingPk);
                    }
                    if (!@class.TryGetMetadata("primary-keys", out var keys))
                    {
                        @class.AddMetadata("primary-keys", primaryKeyProperties.ToArray());
                    }
                });
            }
        }

        private void LogAggregateRelationshipsAsError(ClassModel model)
        {
            var notSupportedAssociations = model
                .AssociationEnds()
                .Where(x => x.Association.SourceEnd.Element.Id == model.Id &&
                            (x.Association.SourceEnd.IsCollection || x.Association.SourceEnd.IsNullable))
                .ToArray();
            foreach (var association in notSupportedAssociations)
            {
                var sourceClass = model;
                var targetClass = association.Class;
                Logging.Log.Failure($"Association not supported between {sourceClass.Name} and {targetClass.Name}. Ensure the source end is set to not be Is Nullable and not Is Collection.");
            }
        }
    }
}