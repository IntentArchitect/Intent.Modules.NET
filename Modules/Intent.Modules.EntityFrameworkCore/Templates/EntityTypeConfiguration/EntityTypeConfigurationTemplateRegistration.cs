using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Metadata.RDBMS.Api;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Registrations;
using Intent.Modules.Constants;
using Intent.Modules.EntityFrameworkCore.Settings;
using Intent.Modules.Metadata.RDBMS.Settings;
//using Intent.Modules.Metadata.RDBMS.Settings;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.TemplateRegistration.FilePerModel", Version = "1.0")]

namespace Intent.Modules.EntityFrameworkCore.Templates.EntityTypeConfiguration
{
    [IntentManaged(Mode.Merge, Body = Mode.Merge, Signature = Mode.Fully)]
    public class EntityTypeConfigurationTemplateRegistration : FilePerModelTemplateRegistration<ClassModel>
    {
        private readonly IMetadataManager _metadataManager;

        public EntityTypeConfigurationTemplateRegistration(IMetadataManager metadataManager)
        {
            _metadataManager = metadataManager;
        }

        public override string TemplateId => EntityTypeConfigurationTemplate.TemplateId;

        [IntentManaged(Mode.Fully)]
        public override ITemplate CreateTemplateInstance(IOutputTarget outputTarget, ClassModel model)
        {
            return new EntityTypeConfigurationTemplate(outputTarget, model);
        }

        [IntentManaged(Mode.Merge, Body = Mode.Ignore, Signature = Mode.Fully)]
        public override IEnumerable<ClassModel> GetModels(IApplication application)
        {
            return _metadataManager.Domain(application).GetClassModels()
                .Where(x => x.InternalElement.Package.AsDomainPackageModel()?.HasRelationalDatabase() == true &&
                    !x.IsOwned(application) && (
                    !x.IsAbstract || // is concrete class
                    x.HasTable() || // has Table stereotype
                    x.AssociatedFromClasses().Any(x => x.IsNullable || x.IsCollection))) // is referenced by other entities
                .ToArray();
        }
    }

    public static class EntityTypeConfigurationTemplateExtensions
    {
        public static bool IsOwned(this ClassModel @class, ISoftwareFactoryExecutionContext executionContext)
        {
            return IsOwned(@class.InternalElement, executionContext);
        }

        public static bool IsOwned(this ICanBeReferencedType type, ISoftwareFactoryExecutionContext executionContext)
        {
            if (type.IsClassModel())
            {
                return !type.AsClassModel().IsAggregateRoot() &&
                       !type.AsClassModel().HasTable() &&
                       !type.AsClassModel().AssociatedFromClasses().Any(x => x.IsNullable || x.IsCollection) && // cannot have aggregations pointing to it
                       !type.AsClassModel().AssociatedClasses.Any(x => x.IsCollection && x.OtherEnd().IsCollection) && // cannot have any many to many associated with it
                       (executionContext.Settings.GetDatabaseSettings().DatabaseProvider().IsCosmos() || !type.AsClassModel().AssociationEnds().Any(x => x.IsTargetEnd() && x.OtherEnd().IsNullable && !x.OtherEnd().IsCollection)); // cannot have an aggregate relationship outward
            }

            return type.IsValueObject(executionContext);
        }

        public static bool IsValueObject(this ICanBeReferencedType type, ISoftwareFactoryExecutionContext executionContext)
        {
            return executionContext.FindTemplateInstance(TemplateRoles.Domain.ValueObject, type.Id) != null;
        }

        internal static bool IsValueObject(this ICanBeReferencedType type,
            ISoftwareFactoryExecutionContext executionContext, out ICSharpFileBuilderTemplate builderTemplate)
        {
            builderTemplate =
                executionContext.FindTemplateInstance<ICSharpFileBuilderTemplate>(
                    TemplateRoles.Domain.ValueObject, type.Id);
            return builderTemplate != null;
        }
    }
}