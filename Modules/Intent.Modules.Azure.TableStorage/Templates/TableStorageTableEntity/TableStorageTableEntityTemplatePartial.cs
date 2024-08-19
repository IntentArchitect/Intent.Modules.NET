using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using Intent.Engine;
using Intent.Metadata.DocumentDB.Api;
using Intent.Metadata.Models;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.CSharp.TypeResolvers;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using Intent.Modules.Modelers.Domain.Settings;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;
using Intent.Utils;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Azure.TableStorage.Templates.TableStorageTableEntity
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class TableStorageTableEntityTemplate : CSharpTemplateBase<ClassModel>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.Azure.TableStorage.TableStorageTableEntity";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public TableStorageTableEntityTemplate(IOutputTarget outputTarget, ClassModel model) : base(TemplateId, outputTarget, model)
        {
            SetDefaultCollectionFormatter(CSharpCollectionFormatter.CreateICollection());
            AddTypeSource(TemplateId);
            AddTypeSource(TemplateRoles.Domain.Enum);
            AddTypeSource(TemplateRoles.Domain.Entity.Primary);
            AddTypeSource(TemplateRoles.Domain.ValueObject);

            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddClass($"{Model.Name}TableEntity", @class =>
                {
                    @class.Internal();

                    @class.ImplementsInterface($"{this.GetTableStorageTableAdapterInterfaceName()}<{EntityInterfaceName}, {@class.Name}>");
                    @class.ImplementsInterface($"{this.GetTableStorageTableEntityInterfaceName()}");

                    if (model.IsAbstract)
                    {
                        @class.Abstract();
                    }

                })
                .OnBuild(file =>
                {
                    var @class = file.Classes.First();
                    var entityPropertyIds = EntityStateFileBuilder.CSharpFile.Classes.First().Properties
                        .Select(x => x.TryGetMetadata<IMetadataModel>("model", out var metadataModel) && metadataModel is AttributeModel or AssociationEndModel
                                ? metadataModel.Id
                                : null)
                        .Where(x => x != null)
                        .ToHashSet();

                    var attributes = Model.Attributes
                        .Where(x => entityPropertyIds.Contains(x.Id))
                        .ToList();
                    var associationEnds = Model.AssociatedClasses
                        .Where(x => entityPropertyIds.Contains(x.Id) && x.IsNavigable)
                        .ToList();

                    if (model.IsAggregateRoot())
                    {

                        AddPropertiesForAggregate(@class);
                        AddCosmosDBMappingMethods(
                            template: this,
                            @class: @class,
                            attributes: attributes,
                            associationEnds: associationEnds,
                            entityInterfaceTypeName: EntityInterfaceName,
                            entityStateTypeName: EntityStateName);
                    }

                }, 1000);

        }

        public void AddPropertiesForAggregate(CSharpClass @class)
        {
            var entityProperties = EntityStateFileBuilder.CSharpFile.Classes.First()
                .Properties.Where(x => x.TryGetMetadata<IMetadataModel>("model", out var metadataModel) && metadataModel is AttributeModel or AssociationEndModel &&
                    (x.Name != "Timestamp" && x.Name != "ETag"))
                .ToArray();

            foreach (var entityProperty in entityProperties)
            {
                var metadataModel = entityProperty.GetMetadata<IMetadataModel>("model");
                var typeReference = metadataModel switch
                {
                    AttributeModel attribute => attribute.TypeReference,
                    AssociationEndModel associationEnd => associationEnd,
                    _ => throw new InvalidOperationException()
                };

                var typeName = GetTypeName(typeReference);

                if (metadataModel is AttributeModel)
                {
                    @class.AddProperty(typeName, entityProperty.Name, property =>
                    {
                        if (IsNonNullableReferenceType(typeReference))
                        {
                            property.WithInitialValue("default!");
                        }
                    });
                }
                else if (IsCompositional((AssociationEndModel)metadataModel))
                {
                    @class.AddProperty("string", entityProperty.Name, property => property.WithInitialValue("default!"));
                }
            }
            @class.AddProperty(this.UseType("System.DateTimeOffset?"), "Timestamp");
            @class.AddProperty(this.UseType("Azure.ETag"), "ETag", property => property.WithInitialValue("ETag.All"));
        }

        private bool IsCompositional(AssociationEndModel associationEnd)
        {
            return !(associationEnd.OtherEnd().IsNullable || associationEnd.OtherEnd().IsCollection);
        }

        public void AddCosmosDBMappingMethods(
            ICSharpTemplate template,
            CSharpClass @class,
            IReadOnlyList<AttributeModel> attributes,
            IReadOnlyList<AssociationEndModel> associationEnds,
            string entityInterfaceTypeName,
            string entityStateTypeName)
        {

            @class.AddMethod($"{entityInterfaceTypeName}", "ToEntity", method =>
            {
                method.AddParameter($"{entityStateTypeName}?", "entity", p =>
                {
                    if (!@class.IsAbstract)
                    {
                        p.WithDefaultValue("default");
                    }
                });

                method.AddStatement($"entity ??= new {entityStateTypeName}();");

                for (var index = 0; index < attributes.Count; index++)
                {
                    var attribute = attributes[index];
                    var assignmentValueExpression = attribute.Name;

                    method.AddStatement($"entity.{attribute.Name.ToPascalCase()} = {assignmentValueExpression};", index == 0 ? s => s.SeparatedFromPrevious() : null);
                }

                //Only Compositions
                foreach (var associationEnd in associationEnds.Where(a => IsCompositional(a)))
                {
                    method.AddStatement($"entity.{associationEnd.Name.ToPascalCase()} = {template.UseType("System.Text.Json.JsonSerializer")}.Deserialize<{GetAssociationTypeNameExpression(TemplateRoles.Domain.Entity.Primary, associationEnd)}>({associationEnd.Name.ToPascalCase()});");
                }


                method.AddStatement("return entity;", s => s.SeparatedFromPrevious());
            });

            @class.AddMethod($"{@class.Name}", "PopulateFromEntity", method =>
            {
                method.AddParameter($"{entityInterfaceTypeName}", "entity");

                foreach (var attribute in attributes)
                {
                    method.AddStatement($"{attribute.Name} = entity.{attribute.Name};");
                }

                //Only Compositions
                foreach (var associationEnd in associationEnds.Where(a => IsCompositional(a)))
                {
                    var documentTypeName = template.GetTypeName((IElement)associationEnd.TypeReference.Element);

                    method.AddStatement($"{associationEnd.Name} = {template.UseType("System.Text.Json.JsonSerializer")}.Serialize(entity.{associationEnd.Name.ToPascalCase()});");
                }

                method.AddStatement("return this;", s => s.SeparatedFromPrevious());
            });

            @class.AddMethod($"{@class.Name}?", "FromEntity", method =>
            {
                method.AddParameter($"{entityInterfaceTypeName}?", "entity");
                method.Static();

                method.AddIfStatement("entity is null", @if => @if.AddStatement("return null;"));
                method.AddStatement($"return new {@class.Name}().PopulateFromEntity(entity);", s => s.SeparatedFromPrevious());
            });
        }


        public string EntityInterfaceName => GetTypeName(TemplateRoles.Domain.Entity.Interface, Model);

        public string EntityStateName => GetTypeName(TemplateRoles.Domain.Entity.Primary, Model);

        public ICSharpFileBuilderTemplate EntityStateFileBuilder => GetTemplate<ICSharpFileBuilderTemplate>(TemplateRoles.Domain.Entity.Primary, Model);

        private string GetAssociationTypeNameExpression(string templateId, AssociationEndModel associationEndModel)
        {
            var type = GetTypeName(templateId, associationEndModel.TypeReference.Element);
            return associationEndModel.TypeReference switch
            {
                var returnType when returnType.IsCollection => $"{UseType("System.Collections.Generic.ICollection")}<{type}>",
                var returnType when returnType.IsNullable => $"{type}?",
                _ => type
            };
        }


        [IntentManaged(Mode.Fully)]
        public CSharpFile CSharpFile { get; }

        [IntentManaged(Mode.Fully)]
        protected override CSharpFileConfig DefineFileConfig()
        {
            return CSharpFile.GetConfig();
        }

        [IntentManaged(Mode.Fully)]
        public override string TransformText()
        {
            return CSharpFile.ToString();
        }
    }
}