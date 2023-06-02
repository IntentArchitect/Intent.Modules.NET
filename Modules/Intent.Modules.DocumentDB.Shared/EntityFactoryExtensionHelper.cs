using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.DocumentDB.Api.Extensions;
using Intent.Metadata.Models;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;

namespace Intent.Modules.DocumentDB.Shared
{
    internal static class EntityFactoryExtensionHelper
    {
        public static void Execute(IApplication application)
        {
            var templates = application.FindTemplateInstances<ICSharpFileBuilderTemplate>(TemplateDependency.OnTemplate(TemplateFulfillingRoles.Domain.Entity.Primary));
            foreach (var template in templates)
            {
                var templateModel = ((CSharpTemplateBase<ClassModel>)template).Model;
                if (!templateModel.InternalElement.Package.HasStereotype("Document Database"))
                {
                    continue;
                }

                template.CSharpFile.OnBuild(file =>
                {
                    file.AddUsing("System");
                    var @class = file.Classes.First();
                    var model = @class.GetMetadata<ClassModel>("model");

                    var toChangeNavigationProperies = GetNavigableAggregateAssociations(model);
                    foreach (var navigation in toChangeNavigationProperies)
                    {
                        //Remove the "Entity" Properties and backing fields
                        var property = @class.GetAllProperties()
                            .FirstOrDefault(x => x.GetMetadata<IMetadataModel>("model").Id == navigation.Id);
                        @class.Properties.Remove(property);

                        var field = @class.Fields
                            .FirstOrDefault(x => x.GetMetadata<IMetadataModel>("model").Id == navigation.Id);
                        if (field != null)
                        {
                            @class.Fields.Remove(field);
                        }

                        /*
                        var key = navigation.Class.GetExplicitPrimaryKey().Single();

                        var typeName = template.GetTypeName(key.Type);
                        if (navigation.IsCollection)
                            typeName += "[]";
                        else if (navigation.IsNullable)
                            typeName += "?";

                        @class.AddProperty(typeName, $"{property.Name}{(navigation.IsCollection ? "Ids" : "Id")}", c => c.AddMetadata("model", navigation))
                        */
                    }

                    var pks = model.GetPrimaryKeys();
                    if (!pks.Any())
                    {
                        return;
                    }

                    var primaryKeyProperties = new List<CSharpProperty>();
                    foreach (var attribute in pks)
                    {
                        var existingPk = @class.GetAllProperties().FirstOrDefault(x =>
                            x.Name.Equals(attribute.Name, StringComparison.InvariantCultureIgnoreCase));
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

        private static bool HasNavigationProperty(ClassModel model, AssociationEndModel association)
        {
            return (association.Association.SourceEnd.Element.Id == model.Id && association.Association.TargetEnd.IsNavigable) ||
                (association.Association.TargetEnd.Element.Id == model.Id && association.Association.SourceEnd.IsNavigable);
        }

        private static IEnumerable<AssociationEndModel> GetNavigableAggregateAssociations(ClassModel model)
        {
            return model
                .AssociationEnds()
                .Where(x => HasNavigationProperty(model, x) &&
                            (x.Association.SourceEnd.IsCollection || x.Association.SourceEnd.IsNullable))
                .ToArray();
        }
    }
}