using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common;
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

        public override ITemplate CreateTemplateInstance(IOutputTarget outputTarget, ClassModel model)
        {
            return new EntityTypeConfigurationTemplate(outputTarget, model);
        }

        [IntentManaged(Mode.Merge, Body = Mode.Ignore, Signature = Mode.Fully)]
        public override IEnumerable<ClassModel> GetModels(IApplication application)
        {
            IEnumerable<ClassModel> models = _metadataManager.Domain(application).GetClassModels()
                .Where(x => !x.IsOwned(application));

            //if (application.Settings.GetDatabaseSettings().DatabaseProvider().IsCosmos())
            //{
            //    models = models.Where(p => p.IsAggregateRoot() || (p.IsAbstract && !application.Settings.GetDatabaseSettings().InheritanceStrategy().IsTPC()));
            //}
            //else
            //{
            //    models = models.Where(p => p.IsAggregateRoot() && (!p.IsAbstract || !application.Settings.GetDatabaseSettings().InheritanceStrategy().IsTPC()));
            //}

            return models.ToArray();
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
                return !type.AsClassModel().IsAggregateRoot() && !type.HasStereotype("Repository");
            }

            return type.IsValueObject(executionContext);
        }

        public static bool IsValueObject(this ICanBeReferencedType type, ISoftwareFactoryExecutionContext executionContext)
        {
            return executionContext.FindTemplateInstance(TemplateFulfillingRoles.Domain.ValueObject, type.Id) != null;
        }

    }
}