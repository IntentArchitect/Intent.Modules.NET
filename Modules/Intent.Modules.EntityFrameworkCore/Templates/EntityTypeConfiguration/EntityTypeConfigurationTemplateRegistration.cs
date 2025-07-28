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
                    x.ShouldOptOutOfCompositeModel(application) || // should opt out of composite model
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
                       !type.AsClassModel().ShouldOptOutOfCompositeModel(executionContext) &&
                       !type.AsClassModel().AssociatedFromClasses().Any(x => x.IsNullable || x.IsCollection) && // cannot have aggregations pointing to it
                       !type.AsClassModel().AssociatedClasses.Any(x => x.IsCollection && x.OtherEnd().IsCollection) && // cannot have any many to many associated with it
                       (executionContext.Settings.GetDatabaseSettings().DatabaseProvider().IsCosmos() || !type.AsClassModel().AssociationEnds().Any(x => x.IsTargetEnd() && x.OtherEnd().IsNullable && !x.OtherEnd().IsCollection)); // cannot have an aggregate relationship outward
            }

            return type.IsValueObject(executionContext);
        }

        private static readonly Dictionary<string, HashSet<string>> _optOutCache = new();
        private static string _cacheKey = "";

        public static bool ShouldOptOutOfCompositeModel(this ClassModel model, ISoftwareFactoryExecutionContext executionContext)
        {
            // Create a cache key based on the application context to handle multiple applications
            var currentCacheKey = executionContext.GetApplicationConfig()?.Name ?? "default";
            
            // Reset cache if we're dealing with a different application
            if (_cacheKey != currentCacheKey)
            {
                _optOutCache.Clear();
                _cacheKey = currentCacheKey;
            }

            // Get or compute the opt-out set for this application
            if (!_optOutCache.TryGetValue(currentCacheKey, out var optOutSet))
            {
                optOutSet = ComputeOptOutEntities(executionContext as IApplication ?? throw new InvalidOperationException("Expected IApplication context"));
                _optOutCache[currentCacheKey] = optOutSet;
            }

            return optOutSet.Contains(model.Id);
        }

        private static HashSet<string> ComputeOptOutEntities(IApplication application)
        {
            var allEntities = application.MetadataManager.Domain(application).GetClassModels().ToList();
            var optOutSet = new HashSet<string>();

            // Pass 1: Find entities with explicit Table stereotypes or direct external references
            foreach (var entity in allEntities)
            {
                // Explicit opt-out via Table stereotype (preserves all current behavior)
                if (entity.HasTable())
                {
                    optOutSet.Add(entity.Id);
                    continue;
                }

                // INHERITANCE SAFETY: Don't auto-opt-out abstract classes in inheritance hierarchies
                if (entity.IsAbstract && entity.GeneralizationEnds().Count > 0)
                    continue;

                // INHERITANCE SAFETY: Don't auto-opt-out classes that are inheriting unless explicit
                if (IsInheriting(entity) && !entity.IsAggregateRoot())
                    continue;

                // Auto opt-out: Has external references AND is not in inheritance hierarchy
                if (entity.AssociatedFromClasses().Any(x => x.IsNullable || x.IsCollection) &&
                    !IsInheriting(entity) && entity.GeneralizationEnds().Count == 0)
                {
                    optOutSet.Add(entity.Id);
                }
            }

            // Pass 2: Propagate opt-out upward through composition hierarchy
            bool changed;
            do
            {
                changed = false;
                foreach (var entity in allEntities)
                {
                    // Skip if already opted out
                    if (optOutSet.Contains(entity.Id))
                        continue;

                    // INHERITANCE SAFETY: Don't auto-opt-out abstract classes in inheritance hierarchies
                    if (entity.IsAbstract && entity.GeneralizationEnds().Count > 0)
                        continue;

                    // INHERITANCE SAFETY: Don't auto-opt-out classes that are inheriting unless explicit
                    if (IsInheriting(entity) && !entity.IsAggregateRoot())
                        continue;

                    // Check if this entity has any child entities that are opting out
                    var hasOptedOutChildren = entity.AssociatedClasses.Any(association =>
                        association.IsCollection && // one-to-many relationship
                        !association.OtherEnd().IsCollection && // not many-to-many
                        optOutSet.Contains(association.Element.Id)); // child is opting out

                    if (hasOptedOutChildren)
                    {
                        optOutSet.Add(entity.Id);
                        changed = true;
                    }
                }
            } while (changed);

            return optOutSet;
        }

        private static bool IsInheriting(ClassModel model) => model?.ParentClass != null;

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