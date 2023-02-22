using System;
using System.Linq;
using Intent.Engine;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using Intent.MongoDb.Api;
using Intent.RoslynWeaver.Attributes;
using Intent.Utils;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.MongoDb.FactoryExtensions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class EntityFactoryExtension : FactoryExtensionBase
    {
        public override string Id => "Intent.MongoDb.EntityFactoryExtension";

        [IntentManaged(Mode.Ignore)]
        public override int Order => 0;

        protected override void OnAfterTemplateRegistrations(IApplication application)
        {
            var templates = application.FindTemplateInstances<ICSharpFileBuilderTemplate>(TemplateDependency.OnTemplate(TemplateFulfillingRoles.Domain.Entity.Primary));
            foreach (var template in templates)
            {
                var templateModel = ((CSharpTemplateBase<ClassModel>)template).Model;
                if (!templateModel.InternalElement.Package.IsMongoDomainPackageModel())
                {
                    continue;
                }

                LogAggregateRelationshipsAsError(templateModel);

                template.CSharpFile.OnBuild(file =>
                {
                    file.AddUsing("System");
                    var @class = file.Classes.First();
                    var model = @class.GetMetadata<ClassModel>("model");

                    var idAttr = model.Attributes.First(p => p.Name.Equals("id", StringComparison.OrdinalIgnoreCase));
                    var prop = @class.GetAllProperties().First(p => p.Name.Equals(idAttr.Name, StringComparison.OrdinalIgnoreCase));

                    if (!IsAggregateRoot(model))
                    {
                        @class.AddField(template.GetTypeName(idAttr.TypeReference) + "?", "_id");
                    }
                    if (!IsAggregateRoot(model))
                    {
                        prop.Getter.WithExpressionImplementation($"_id ??= Guid.NewGuid()");
                        prop.Setter.WithExpressionImplementation($"_id = value");
                    }

                    // var pks = model.GetExplicitPrimaryKey();
                    // var primaryKeyProperties = new List<CSharpProperty>();
                    // if (pks.Any())
                    // {
                    //     foreach (var attribute in pks)
                    //     {
                    //         var existingPk = @class.GetAllProperties().FirstOrDefault(x => x.Name.Equals(attribute.Name, StringComparison.InvariantCultureIgnoreCase));
                    //         if (existingPk == null)
                    //         {
                    //             var typeName = attribute.Type != null
                    //                 ? template.GetTypeName(attribute.Type.Element.AsTypeReference(isNullable: attribute.Type.IsNullable, isCollection: attribute.Type.IsCollection))
                    //                 : GetDefaultSurrogateKeyType(template.ExecutionContext) + (attribute.Type.IsNullable ? "?" : string.Empty);
                    //
                    //             @class.AddField(template.UseType(typeName) + "?", attribute.Name.ToPrivateMemberName());
                    //             @class.InsertProperty(0, template.UseType(typeName), attribute.Name, property =>
                    //             {
                    //                 if (!IsAggregateRoot(model))
                    //                 {
                    //                     property.Getter.WithExpressionImplementation($"{attribute.Name.ToPrivateMemberName()} ??= Guid.NewGuid()");
                    //                     property.Setter.WithExpressionImplementation($"{attribute.Name.ToPrivateMemberName()} = value");
                    //                 }
                    //
                    //                 primaryKeyProperties.Add(property);
                    //             });
                    //         }
                    //         else
                    //         {
                    //             @class.AddField(template.UseType(existingPk.Type) + "?", existingPk.Name.ToPrivateMemberName());
                    //             if (!IsAggregateRoot(model))
                    //             {
                    //                 existingPk.Getter.WithExpressionImplementation($"{attribute.Name.ToPrivateMemberName()} ??= Guid.NewGuid()");
                    //                 existingPk.Setter.WithExpressionImplementation($"{attribute.Name.ToPrivateMemberName()} = value");
                    //             }
                    //
                    //             primaryKeyProperties.Add(existingPk);
                    //         }
                    //     }
                    // }
                    // else
                    // {
                    //     @class.AddField(template.UseType(GetDefaultSurrogateKeyType(template.ExecutionContext)) + "?", "_id");
                    //     @class.InsertProperty(0, GetDefaultSurrogateKeyType(template.ExecutionContext), "Id", property =>
                    //     {
                    //         if (!IsAggregateRoot(model))
                    //         {
                    //             property.Getter.WithExpressionImplementation($"_id ??= Guid.NewGuid()");
                    //             property.Setter.WithExpressionImplementation($"_id = value");
                    //         }
                    //
                    //         primaryKeyProperties.Add(property);
                    //     });
                    // }
                    //
                    // if (!@class.TryGetMetadata("primary-keys", out var keys))
                    // {
                    //     @class.AddMetadata("primary-keys", primaryKeyProperties.ToArray());
                    // }
                });
            }
        }

        private static bool IsAggregateRoot(ClassModel entity)
        {
            return !entity.AssociationEnds()
                .Any(x => x.IsSourceEnd() && !x.IsCollection && !x.IsNullable);
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

        // static string GetDefaultSurrogateKeyType(ISoftwareFactoryExecutionContext executionContext)
        // {
        //     var settingType = executionContext.Settings.GetDatabaseSettings()?.KeyType().Value ?? "guid";
        //     switch (settingType)
        //     {
        //         case "guid":
        //             return "System.Guid";
        //         case "int":
        //             return "int";
        //         case "long":
        //             return "long";
        //         default:
        //             return settingType;
        //     }
        // }
    }
}