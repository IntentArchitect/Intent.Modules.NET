using Intent.Engine;
using Intent.EntityFrameworkCore.Api;
using Intent.Metadata.Models;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.CSharp.VisualStudio;
using Intent.Modules.Common.Templates;
using Intent.Modules.EntityFrameworkCore.Settings;
using Intent.RoslynWeaver.Attributes;
using Intent.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.EntityFrameworkCore.Templates.EntityTypeConfiguration
{
    public class EntityTypeConfigurationCreatedEvent
    {
        public EntityTypeConfigurationCreatedEvent(EntityTypeConfigurationTemplate template)
        {
            Template = template;
        }

        public EntityTypeConfigurationTemplate Template { get; set; }
    }

    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public partial class EntityTypeConfigurationTemplate : CSharpTemplateBase<ClassModel, ITemplateDecorator>, ICSharpFileBuilderTemplate
    {
        private IIntentTemplate _entityTemplate;

        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "Intent.EntityFrameworkCore.EntityTypeConfiguration";

        [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
        public EntityTypeConfigurationTemplate(IOutputTarget outputTarget, ClassModel model) : base(TemplateId, outputTarget, model)
        {
            AddNugetDependency(NugetPackages.EntityFrameworkCore(Project));
            AddTypeSource("Domain.Entity");
            AddTypeSource("Domain.ValueObject");

            CSharpFile = new CSharpFile(OutputTarget.GetNamespace(), "")
                .AddUsing("Microsoft.EntityFrameworkCore")
                .AddUsing("Microsoft.EntityFrameworkCore.Metadata.Builders")
                .AddClass($"{Model.Name}Configuration", @class =>
                {
                    if (!TryGetTemplate("Domain.Entity.State", Model, out _entityTemplate))
                    {
                        _entityTemplate = GetTemplate<IIntentTemplate>("Domain.Entity", Model);
                    }
                    @class.ImplementsInterface($"IEntityTypeConfiguration<{GetTypeName(_entityTemplate)}>")
                        .AddMethod("void", "Configure", method =>
                        {
                            method.AddMetadata("model", Model.InternalElement);
                            method.AddParameter($"EntityTypeBuilder<{GetTypeName(_entityTemplate)}>", "builder");
                            method.AddStatements(GetTypeConfiguration(Model.InternalElement, @class));

                            if (_entityTemplate is ICSharpFileBuilderTemplate builderTemplate)
                            {
                                // GCB - this approach (using the properties) is potentially worth exploring as it decouples the EF Core from the Domain designer
                                //builderTemplate.CSharpFile.OnBuild(file =>
                                //{
                                //    foreach (var property in file.Classes.First().GetAllProperties())
                                //    {
                                //        if (property.TryGetMetadata<AttributeModel>("model", out var attribute))
                                //        {
                                //            method.AddStatement(GetAttributeMapping(attribute, @class));
                                //        }
                                //        else if (property.TryGetMetadata<AssociationEndModel>("model", out var associationEnd))
                                //        {
                                //            method.AddStatement(GetAssociationMapping(associationEnd, @class));
                                //        }
                                //    }
                                //    method.Statements.SeparateAll();
                                //});
                                builderTemplate.CSharpFile.OnBuild(file =>
                                {
                                    foreach (var property in file.Classes.First().GetAllProperties())
                                    {
                                        if (property.TryGetMetadata<bool>("non-persistent", out var nonPersistent) && nonPersistent &&
                                            !TryGetTemplate<EntityTypeConfigurationTemplate>(Id, Model.ParentClass?.Id, out var template))
                                        {
                                            method.AddStatement($"builder.Ignore(e => e.{property.Name});");
                                        }
                                    }
                                    method.Statements.SeparateAll();
                                }, order: 100); // Needs to run after other decorators of the entity
                            }

                            method.Statements.SeparateAll();
                        });
                });
        }
        public CSharpFile CSharpFile { get; }

        public override void BeforeTemplateExecution()
        {
            base.BeforeTemplateExecution();
            ExecutionContext.EventDispatcher.Publish(new EntityTypeConfigurationCreatedEvent(this));
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        protected override CSharpFileConfig DefineFileConfig()
        {
            return new CSharpFileConfig(
                className: $"{Model.Name}Configuration",
                @namespace: $"{OutputTarget.GetNamespace()}");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public override string TransformText()
        {
            return CSharpFile.ToString();
        }

        //private IEnumerable<string> GetTableMapping(ClassModel model)
        //{
        //    if (ExecutionContext.Settings.GetDatabaseSettings().DatabaseProvider().IsCosmos() && model.IsAggregateRoot())
        //    {
        //        // Is there an easier way to get this?
        //        var domainPackage = new DomainPackageModel(this.Model.InternalElement.Package);
        //        var cosmosSettings = domainPackage.GetCosmosDBContainerSettings();

        //        var containerName = string.IsNullOrWhiteSpace(cosmosSettings?.ContainerName())
        //            ? OutputTarget.ApplicationName()
        //            : cosmosSettings.ContainerName();

        //        yield return $@"builder.ToContainer(""{containerName}"");";

        //        var partitionKey = cosmosSettings?.PartitionKey()?.ToPascalCase();
        //        if (string.IsNullOrEmpty(partitionKey))
        //        {
        //            partitionKey = "PartitionKey";
        //        }

        //        if (GetAttributes(Model.InternalElement).Any(p =>
        //                p.Name.ToPascalCase().Equals(partitionKey) && p.HasPartitionKey()))
        //        {
        //            yield return $@"builder.HasPartitionKey(x => x.{partitionKey});";
        //        }
        //    }
        //    else
        //    {
        //        if (model.HasTable())
        //        {
        //            yield return
        //                $@"builder.ToTable(""{model.GetTable()?.Name() ?? model.Name}""{(!string.IsNullOrWhiteSpace(model.GetTable()?.Schema()) ? @$", ""{model.GetTable().Schema() ?? "dbo"}""" : "")});";
        //        }

        //        if ((model.ParentClass != null || model.ChildClasses.Any()) &&
        //            !ExecutionContext.Settings.GetDatabaseSettings().InheritanceStrategy().IsTPH())
        //        {
        //            yield return
        //                $@"builder.ToTable(""{model.Name}""{(!string.IsNullOrWhiteSpace(model.GetTable()?.Schema()) ? @$", ""{model.GetTable().Schema() ?? "dbo"}""" : "")});";
        //        }
        //    }

        //    if (model.ParentClass != null &&
        //        ExecutionContext.Settings.GetDatabaseSettings().InheritanceStrategy().IsTPH())
        //    {
        //        yield return $@"builder.HasBaseType<{GetTypeName("Domain.Entity", model.ParentClass)}>();";
        //    }
        //    yield return string.Empty;
        //}


        private bool RequiresConfiguration(AttributeModel attribute)
        {
            return true;
            //return attribute.InternalElement.ParentElement.AsClassModel()?.GetExplicitPrimaryKey().All(key => !key.Equals(attribute)) == true &&
            //       !attribute.Name.Equals("id", StringComparison.InvariantCultureIgnoreCase);
        }

        private bool RequiresConfiguration(AssociationEndModel associationEnd)
        {
            return associationEnd.IsTargetEnd();
        }

        private EFCoreConfigStatementBase GetAttributeMapping(AttributeModel attribute, CSharpClass @class)
        {
            EFCoreFieldConfigStatement result = null;

            if (!IsOwned(attribute.TypeReference.Element))
            {
                return new EFCoreFieldConfigStatement($"builder.Property(x => x.{attribute.Name.ToPascalCase()})", attribute)
                    .AddStatements(GetAttributeMappingStatements(attribute));
            }

            @class.AddMethod("void", $"Configure{attribute.Name.ToPascalCase()}", method =>
            {
                method.AddMetadata("model", attribute.TypeReference.Element);
                method.AddParameter($"OwnedNavigationBuilder<{GetTypeName(attribute.InternalElement.ParentElement)}, {GetTypeName((IElement)attribute.TypeReference.Element)}>", "builder");
                method.AddStatements(GetTypeConfiguration((IElement)attribute.TypeReference.Element, @class).ToArray());
                method.Statements.SeparateAll();
            });

            if (attribute.TypeReference.IsCollection)
            {
                return new EFCoreFieldConfigStatement($"builder.OwnsMany(x => x.{attribute.Name.ToPascalCase()}, Configure{attribute.Name.ToPascalCase()})", attribute);
            }
            else
            {
                var field = new EFCoreFieldConfigStatement($"builder.OwnsOne(x => x.{attribute.Name.ToPascalCase()}, Configure{attribute.Name.ToPascalCase()})", attribute);
                if (!attribute.TypeReference.IsNullable)
                {
                    field.AddStatement($".Navigation(x => x.{attribute.Name.ToPascalCase()}).IsRequired()");
                }

                return field;
            }
        }

        private List<CSharpStatement> GetAttributeMappingStatements(AttributeModel attribute)
        {
            var statements = new List<CSharpStatement>();
            if (!attribute.Type.IsNullable)
            {
                statements.Add(".IsRequired()");
            }

            return statements;
        }

        private EFCoreConfigStatementBase GetAssociationMapping(AssociationEndModel associationEnd, CSharpClass @class)
        {
            if (associationEnd.Element.Id.Equals(associationEnd.OtherEnd().Element.Id)
                && associationEnd.Name.Equals(associationEnd.Element.Name))
            {
                Logging.Log.Warning(
                    $"Self referencing relationship detected using the same name for the Association as the Class: {associationEnd.Class.Name}. This might cause problems.");
            }

            switch (associationEnd.Association.GetRelationshipType())
            {
                case RelationshipType.OneToOne:
                    if (IsOwned(associationEnd.Element))
                    {
                        @class.AddMethod("void", $"Configure{associationEnd.Name.ToPascalCase()}", method =>
                        {
                            var sourceType = Model.IsSubclassOf(associationEnd.OtherEnd().Class) ? Model.InternalElement : (IElement)associationEnd.OtherEnd().Element;
                            method.AddMetadata("model", (IElement)associationEnd.Element);
                            method.AddParameter($"OwnedNavigationBuilder<{GetTypeName(sourceType)}, {GetTypeName((IElement)associationEnd.Element)}>", "builder");
                            method.AddStatement(new EFCoreAssociationConfigStatement(associationEnd.OtherEnd()));
                            method.AddStatements(GetTypeConfiguration((IElement)associationEnd.Element, @class).ToArray());
                            method.Statements.SeparateAll();
                        });

                        var field = new EFCoreFieldConfigStatement($"builder.OwnsOne(x => x.{associationEnd.Name.ToPascalCase()}, Configure{associationEnd.Name.ToPascalCase()})", associationEnd);
                        if (!associationEnd.TypeReference.IsNullable)
                        {
                            field.AddStatement($".Navigation(x => x.{associationEnd.Name.ToPascalCase()}).IsRequired()");
                        }

                        return field;
                    }

                    break;
                case RelationshipType.ManyToOne:
                    break;
                case RelationshipType.OneToMany:
                    {
                        if (IsOwned(associationEnd.Element))
                        {
                            @class.AddMethod("void", $"Configure{associationEnd.Name.ToPascalCase()}", method =>
                            {
                                var sourceType = Model.IsSubclassOf(associationEnd.OtherEnd().Class) ? Model.InternalElement : (IElement)associationEnd.OtherEnd().Element;
                                method.AddMetadata("model", (IElement)associationEnd.Element);
                                method.AddParameter($"OwnedNavigationBuilder<{GetTypeName(sourceType)}, {GetTypeName((IElement)associationEnd.Element)}>", "builder");
                                method.AddStatement(new EFCoreAssociationConfigStatement(associationEnd.OtherEnd()));
                                method.AddStatements(GetTypeConfiguration((IElement)associationEnd.Element, @class).ToArray());
                                method.Statements.SeparateAll();
                            });
                            var field = new EFCoreFieldConfigStatement($"builder.OwnsMany(x => x.{associationEnd.Name.ToPascalCase()}, Configure{associationEnd.Name.ToPascalCase()})", associationEnd);

                            return field;
                        }
                    }
                    break;
                case RelationshipType.ManyToMany:
                    EnsureColumnsOnEntity(associationEnd.Element, new RequiredColumn(_entityTemplate.GetTypeName(associationEnd.OtherEnd()), associationEnd.OtherEnd().Name.ToPascalCase(), IsPrivate: true));
                    break;
                default:
                    throw new Exception($"Relationship type for association [{Model.Name}.{associationEnd.Name}] could not be determined.");
            }
            return new EFCoreAssociationConfigStatement(associationEnd);
        }

        public void EnsureColumnsOnEntity(ICanBeReferencedType entityModel, params RequiredColumn[] columns)
        {
            if (TryGetTemplate<ICSharpFileBuilderTemplate>("Domain.Entity", entityModel.Id, out var template))
            {
                template.CSharpFile.OnBuild(file =>
                {
                    var associatedClass = file.Classes.First();
                    foreach (var column in columns)
                    {
                        if (!associatedClass.GetAllProperties().Any(x => x.Name.Equals(column.Name, StringComparison.InvariantCultureIgnoreCase)))
                        {
                            var associationProperty = associatedClass.Properties.SingleOrDefault(x => x.Name.Equals(column.Name.RemoveSuffix("Id")));

                            if (column.Order.HasValue)
                            {
                                associatedClass.InsertProperty(column.Order.Value, template.UseType(column.Type), column.Name, ConfigureProperty);
                            }
                            else if (associationProperty != null)
                            {
                                associatedClass.InsertProperty(associatedClass.Properties.IndexOf(associationProperty), template.UseType(column.Type), column.Name, ConfigureProperty);
                            }
                            else
                            {
                                associatedClass.AddProperty(template.UseType(column.Type), column.Name, ConfigureProperty);
                            }

                            void ConfigureProperty(CSharpProperty property)
                            {
                                if (column.IsPrivate)
                                {
                                    property.Private();
                                }
                            }
                        }
                    }
                });
            }
        }

        private IEnumerable<EFCoreConfigStatementBase> GetTypeConfiguration(IElement targetType, CSharpClass @class)
        {
            var statements = new List<EFCoreConfigStatementBase>();

            statements.AddRange(GetAttributes(targetType).Where(RequiresConfiguration).Select(x => GetAttributeMapping(x, @class)));

            statements.AddRange(GetAssociations(targetType).Where(RequiresConfiguration).Select(x => GetAssociationMapping(x, @class)));

            return statements.ToList();
        }

        private bool IsValueObject(ICanBeReferencedType type)
        {
            return TryGetTypeName("Domain.ValueObject", type.Id, out var typename);
        }

        private bool IsOwned(ICanBeReferencedType type)
        {
            if (type.IsClassModel())
            {
                return !type.AsClassModel().IsAggregateRoot();
            }

            return IsValueObject(type);
        }

        private IEnumerable<AttributeModel> GetAttributes(IElement model)
        {
            var attributes = new List<AttributeModel>();
            var @class = model.AsClassModel();
            if (@class?.ParentClass != null &&
                @class.ParentClass.IsAbstract &&
                ExecutionContext.Settings.GetDatabaseSettings().InheritanceStrategy().IsTPC())
            {
                attributes.AddRange(GetAttributes(@class.ParentClass.InternalElement));
            }

            attributes.AddRange(model.ChildElements
                .Where(x => x.IsAttributeModel() && RequiresConfiguration(x.AsAttributeModel()))
                .Select(x => x.AsAttributeModel())
                .ToList());
            return attributes;
        }

        private IEnumerable<AssociationEndModel> GetAssociations(IElement model)
        {
            var associations = new List<AssociationEndModel>();
            var @class = model.AsClassModel();
            if (@class?.ParentClass != null &&
                @class.ParentClass.IsAbstract &&
                ExecutionContext.Settings.GetDatabaseSettings().InheritanceStrategy().IsTPC())
            {
                associations.AddRange(GetAssociations(@class.ParentClass.InternalElement));
            }

            associations.AddRange(model.AssociatedElements
                .Where(x => x.IsAssociationEndModel() && RequiresConfiguration(x.AsAssociationEndModel()))
                .Select(x => x.AsAssociationEndModel())
                .ToList());
            return associations;
        }

        public record RequiredColumn(string Type, string Name, int? Order = null, bool IsPrivate = false);
    }

    public abstract class EFCoreConfigStatementBase : CSharpStatement
    {
        protected EFCoreConfigStatementBase() : base(null)
        {
        }
    }

    public class EFCoreAssociationConfigStatement : EFCoreConfigStatementBase
    {
        private readonly AssociationEndModel _associationEnd;
        public IList<CSharpStatement> RelationshipStatements { get; } = new List<CSharpStatement>();
        public IList<CSharpStatement> AdditionalStatements { get; } = new List<CSharpStatement>();
        public EFCoreAssociationConfigStatement(AssociationEndModel associationEnd)
        {
            _associationEnd = associationEnd;

            if (associationEnd.Element.Id.Equals(associationEnd.OtherEnd().Element.Id)
                && associationEnd.Name.Equals(associationEnd.Element.Name))
            {
                Logging.Log.Warning($"Self referencing relationship detected using the same name for the Association as the Class: {associationEnd.Class.Name}. This might cause problems.");
            }

            if (associationEnd.IsSourceEnd() && !associationEnd.IsNullable && !associationEnd.IsCollection)
            {
                AddMetadata("model", associationEnd.OtherEnd());
                RelationshipStatements.Add(@$"builder.WithOwner({(associationEnd.IsNavigable ? $"x => x.{associationEnd.Name.ToPascalCase()}" : "")})");
                return;
            }

            AddMetadata("model", associationEnd);
            switch (associationEnd.Association.GetRelationshipType())
            {
                case RelationshipType.OneToOne:
                    RelationshipStatements.Add($"builder.HasOne(x => x.{associationEnd.Name.ToPascalCase()})");
                    RelationshipStatements.Add($".WithOne({(associationEnd.OtherEnd().IsNavigable ? $"x => x.{associationEnd.OtherEnd().Name.ToPascalCase()}" : "")})");

                    if (!associationEnd.OtherEnd().IsNullable)
                    {
                        AdditionalStatements.Add($".IsRequired()");
                        AdditionalStatements.Add($".OnDelete(DeleteBehavior.Cascade)");
                    }
                    else
                    {
                        AdditionalStatements.Add($".OnDelete(DeleteBehavior.Restrict)");
                    }

                    break;
                case RelationshipType.ManyToOne:
                    {
                        RelationshipStatements.Add($"builder.HasOne(x => x.{associationEnd.Name.ToPascalCase()})");
                        RelationshipStatements.Add($".WithMany({(associationEnd.OtherEnd().IsNavigable ? "x => x." + associationEnd.OtherEnd().Name.ToPascalCase() : "")})");
                        AdditionalStatements.Add($".OnDelete(DeleteBehavior.Restrict)");
                        break;
                    }
                case RelationshipType.OneToMany:
                    {
                        RelationshipStatements.Add($"builder.HasMany(x => x.{associationEnd.Name.ToPascalCase()})");
                        RelationshipStatements.Add($".WithOne({(associationEnd.OtherEnd().IsNavigable ? $"x => x.{associationEnd.OtherEnd().Name.ToPascalCase()}" : $"")})");
                        if (!associationEnd.OtherEnd().IsNullable)
                        {
                            AdditionalStatements.Add($"    .IsRequired()");
                            AdditionalStatements.Add($"    .OnDelete(DeleteBehavior.Cascade)");
                        }
                    }
                    break;
                case RelationshipType.ManyToMany:

                    RelationshipStatements.Add($"builder.HasMany(x => x.{associationEnd.Name.ToPascalCase()})");
                    RelationshipStatements.Add($".WithMany({(associationEnd.OtherEnd().IsNavigable ? $"x => x.{associationEnd.OtherEnd().Name.ToPascalCase()}" : $"\"{associationEnd.OtherEnd().Name.ToPascalCase()}\"")})");
                    RelationshipStatements.Add($".UsingEntity(x => x.ToTable(\"{associationEnd.OtherEnd().Class.Name}{associationEnd.Class.Name.ToPluralName()}\"))");

                    break;
                default:
                    throw new Exception($"Relationship type for association [{associationEnd.OtherEnd().Element.Name}.{associationEnd.Name}] could not be determined.");
            }
        }

        public EFCoreAssociationConfigStatement AddStatement(CSharpStatement statement)
        {
            AdditionalStatements.Add(statement);
            return this;
        }

        public EFCoreAssociationConfigStatement AddStatements(IEnumerable<CSharpStatement> statements)
        {
            foreach (var statement in statements)
            {
                AdditionalStatements.Add(statement);
            }
            return this;
        }

        public EFCoreAssociationConfigStatement AddForeignKey(params string[] columns)
        {
            string genericType = null;
            if (_associationEnd.IsTargetEnd() &&
                _associationEnd.Association.GetRelationshipType() == RelationshipType.OneToOne)
            {
                if (_associationEnd.IsNullable)
                {
                    if (_associationEnd.OtherEnd().IsNullable)
                    {
                        genericType = _associationEnd.OtherEnd().Class.Name;
                    }
                    else
                    {
                        genericType = _associationEnd.Class.Name;
                    }
                }
                else
                {
                    genericType = _associationEnd.OtherEnd().Class.Name;
                }
            }

            if (columns?.Length == 1)
            {
                RelationshipStatements.Add($".HasForeignKey{(genericType != null ? $"<{genericType}>" : string.Empty)}(x => x.{columns.Single()})");
                return this;
            }

            RelationshipStatements.Add($".HasForeignKey{(genericType != null ? $"<{genericType}>" : string.Empty)}(x => new {{ {string.Join(", ", columns.Select(x => "x." + x))}}})");
            return this;
        }

        public override string GetText(string indentation)
        {
            var x = $@"{indentation}{string.Join(@$"
{indentation}    ", RelationshipStatements.Select(x => x.GetText(string.Empty)))}{(AdditionalStatements.Any() ? $@"
{indentation}    {string.Join(@$"
{indentation}    ", AdditionalStatements.Select(x => x.GetText(string.Empty)))}" : string.Empty)};";
            return x;
        }
    }
}