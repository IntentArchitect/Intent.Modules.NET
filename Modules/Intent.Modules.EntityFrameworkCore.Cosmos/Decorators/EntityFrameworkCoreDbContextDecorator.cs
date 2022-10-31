using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Intent.Engine;
using Intent.EntityFrameworkCore.Api;
using Intent.Metadata.Models;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.EntityFrameworkCore.Cosmos.Settings;
using Intent.Modules.EntityFrameworkCore.Settings;
using Intent.Modules.EntityFrameworkCore.Templates;
using Intent.Modules.EntityFrameworkCore.Templates.EntityTypeConfiguration;
using Intent.Modules.Metadata.RDBMS.Settings;
using Intent.RoslynWeaver.Attributes;
using Intent.Utils;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateDecorator", Version = "1.0")]

namespace Intent.Modules.EntityFrameworkCore.Cosmos.Decorators
{
    [IntentManaged(Mode.Merge)]
    public class EntityFrameworkCoreDbContextDecorator : DecoratorBase
    {
        private readonly EntityTypeConfigurationTemplate _template;

        [IntentManaged(Mode.Fully)]
        public const string DecoratorId = "Intent.EntityFrameworkCore.Cosmos.EntityTypeConfigurationTemplateDecorator";

        public ISoftwareFactoryExecutionContext ExecutionContext { get; }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public EntityFrameworkCoreDbContextDecorator(EntityTypeConfigurationTemplate template, IApplication application)
        {
            _template = template;
            ExecutionContext = application;
            template.CSharpFile.OnBuild(file =>
            {
                var @class = file.Classes.First();
                foreach (var method in @class.Methods)
                {
                    if (method.TryGetMetadata<IElement>("model", out var model))
                    {
                        if (model.IsClassModel())
                        {
                            var statements = new List<CSharpStatement>();
                            //if (method.Name.Equals("Configure"))
                            //{
                            //    statements.AddRange(GetTableMapping(model.AsClassModel()));
                            //}

                            //if (model.AsClassModel().ParentClass == null)
                            //{
                            //    statements.Add(GetKeyMapping(model.AsClassModel()));
                            //}

                            method.InsertStatements(method.Statements.FindIndex(x => x.ToString().Trim().StartsWith("builder"), -1) + 1, statements, s =>
                            {
                                foreach (var cSharpStatement in s)
                                {
                                    cSharpStatement.SeparatedFromPrevious();
                                }
                            });

                            //method.AddStatements(GetIndexes(model.AsClassModel()));
                        }
                    }
                    foreach (var statement in method.Statements.OfType<EfCoreFieldConfigStatement>().ToList())
                    {
                        if (statement.TryGetMetadata<AttributeModel>("model", out var attribute))
                        {
                            //if (model?.AsClassModel()?.GetExplicitPrimaryKey().Any(x => x.Equals(attribute)) == true)
                            //{
                            //    statement.Remove();
                            //}
                            //else
                            {
                                statement.AddStatements(GetAttributeMappingStatements(attribute));
                            }
                        }
                    }

                    foreach (var statement in method.Statements.OfType<EfCoreAssociationConfigStatement>())
                    {
                        if (statement.TryGetMetadata<AssociationEndModel>("model", out var associationEnd))
                        {
                            EntityTypeConfigurationTemplate.RequiredColumn[] fks = null;
                            IElement domainElement = null;
                            switch (associationEnd.Association.GetRelationshipType())
                            {
                                case RelationshipType.OneToMany:
                                    if (associationEnd.IsSourceEnd() || associationEnd.OtherEnd().IsNullable)
                                    {
                                        fks = GetForeignColumns(associationEnd.OtherEnd());
                                        domainElement = (IElement)associationEnd.Element;
                                    }

                                    break;
                                case RelationshipType.OneToOne:
                                    if (associationEnd.IsSourceEnd() || associationEnd.OtherEnd().IsNullable)
                                    {
                                        fks = GetForeignColumns(associationEnd);
                                        domainElement = (IElement)associationEnd.OtherEnd().Element;
                                    }
                                    else
                                    {
                                        break;
                                        fks = GetForeignColumns(associationEnd);
                                        domainElement = (IElement)associationEnd.Element;
                                    }
                                    break;
                                case RelationshipType.ManyToOne:
                                    fks = GetForeignColumns(associationEnd);
                                    domainElement = (IElement)associationEnd.OtherEnd().Element;
                                    break;
                                case RelationshipType.ManyToMany:
                                    break;
                                default:
                                    throw new ArgumentOutOfRangeException();
                            }

                            if (fks?.Any() == true)
                            {
                                statement.AddForeignKey(fks.Select(x => x.Name).ToArray());
                                _template.EnsureColumnsOnEntity(domainElement, fks);
                            }
                        }
                    }
                }
            });
        }

        private IEnumerable<CSharpStatement> GetTableMapping(ClassModel model)
        {
            // Is there an easier way to get this?
            var domainPackage = new DomainPackageModel(model.InternalElement.Package);
            var cosmosSettings = domainPackage.GetCosmosDBContainerSettings();

            if (model.ParentClass == null)
            {
                var containerName = string.IsNullOrWhiteSpace(cosmosSettings?.ContainerName())
                    ? _template.OutputTarget.ApplicationName()
                    : cosmosSettings.ContainerName();

                yield return $@"builder.ToContainer(""{containerName}"");";
            }
            //else
            //{
            //    yield return $"builder.HasBaseType<{_template.GetTypeName(model.ParentClass.InternalElement)}>();";
            //}

            if (GetPartitionKey(model) != null)
            {
                yield return $@"builder.HasPartitionKey(x => x.{GetPartitionKey(model).Name.ToPascalCase()});";

                if (model.ParentClass != null)
                {
                    yield return $@"builder.Property(x => x.PartitionKey)
                .IsRequired();";
                }
            }
        }

        private AttributeModel GetPartitionKey(ClassModel model)
        {
            // Is there an easier way to get this?
            var domainPackage = new DomainPackageModel(model.InternalElement.Package);
            var cosmosSettings = domainPackage.GetCosmosDBContainerSettings();

            var partitionKey = cosmosSettings?.PartitionKey()?.ToPascalCase();
            if (string.IsNullOrEmpty(partitionKey))
            {
                partitionKey = "PartitionKey";
            }

            return model.GetTypesInHierarchy().SelectMany(x => x.Attributes).SingleOrDefault(p => p.Name.ToPascalCase().Equals(partitionKey) && p.HasPartitionKey());
        }

        private CSharpStatement GetKeyMapping(ClassModel model)
        {
            var rootEntity = model;
            while (rootEntity.ParentClass != null)
            {
                rootEntity = rootEntity.ParentClass;
            }
            _template.EnsureColumnsOnEntity(rootEntity.InternalElement, new EntityTypeConfigurationTemplate.RequiredColumn(Type: this.GetDefaultSurrogateKeyType(), Name: "Id"));

            if (model.ChildClasses.Any())
            {
                return @"builder.HasKey(x => new { x.PartitionKey, x.Id });";
            }
            return $@"builder.HasKey(x => x.Id);";
        }

        private List<CSharpStatement> GetAttributeMappingStatements(AttributeModel attribute)
        {
            var statements = new List<CSharpStatement>();

            return statements;
        }

        private EntityTypeConfigurationTemplate.RequiredColumn[] GetForeignColumns(AssociationEndModel associationEnd)
        {
            if ((associationEnd.Class?.ParentClass != null || associationEnd.Class?.ChildClasses.Any() == true) &&
                GetPartitionKey(associationEnd.Class) != null)
            {
                return new[] {
                    //new EntityTypeConfigurationTemplate.RequiredColumn(
                    //    Type: null,
                    //    Name: GetPartitionKey(associationEnd.Class).Name.ToPascalCase()),
                    new EntityTypeConfigurationTemplate.RequiredColumn(
                        Type: this.GetDefaultSurrogateKeyType() + (associationEnd.IsNullable ? "?" : ""),
                        Name: $"{(associationEnd.Association.GetRelationshipType() != RelationshipType.OneToOne || associationEnd.IsNullable ? associationEnd.Name.ToPascalCase() : string.Empty)}Id")
                };
            }

            return new[] { new EntityTypeConfigurationTemplate.RequiredColumn(
                    Type: this.GetDefaultSurrogateKeyType() + (associationEnd.IsNullable ? "?" : ""),
                    Name: $"{(associationEnd.Association.GetRelationshipType() != RelationshipType.OneToOne || associationEnd.IsNullable ? associationEnd.Name.ToPascalCase() : string.Empty)}Id") };
        }

        //private void EnsureColumnsOnEntity(ICanBeReferencedType entityModel, params EntityTypeConfigurationTemplate.RequiredColumn[] columns)
        //{
        //    if (_template.TryGetTemplate<ICSharpFileBuilderTemplate>("Domain.Entity", entityModel.Id, out var template))
        //    {
        //        template.CSharpFile.OnBuild(file =>
        //        {
        //            var associatedClass = file.Classes.First();
        //            foreach (var column in columns)
        //            {
        //                if (!associatedClass.GetAllProperties().Any(x => x.Name.Equals(column.Name, StringComparison.InvariantCultureIgnoreCase)))
        //                {
        //                    var associationProperty = associatedClass.Properties.SingleOrDefault(x => x.Name.Equals(column.Name.RemoveSuffix("Id")));

        //                    if (column.Order.HasValue)
        //                    {
        //                        associatedClass.InsertProperty(column.Order.Value, template.UseType(column.Type), column.Name, ConfigureProperty);
        //                    }
        //                    else if (associationProperty != null)
        //                    {
        //                        associatedClass.InsertProperty(associatedClass.Properties.IndexOf(associationProperty), template.UseType(column.Type), column.Name, ConfigureProperty);
        //                    }
        //                    else
        //                    {
        //                        associatedClass.AddProperty(template.UseType(column.Type), column.Name, ConfigureProperty);
        //                    }

        //                    void ConfigureProperty(CSharpProperty property)
        //                    {
        //                        if (column.IsPrivate)
        //                        {
        //                            property.Private();
        //                        }
        //                    }
        //                }
        //            }
        //        });
        //    }
        //}

        public string GetDefaultSurrogateKeyType()
        {
            return GetDefaultSurrogateKeyType(_template.ExecutionContext);
        }

        public static string GetDefaultSurrogateKeyType(ISoftwareFactoryExecutionContext executionContext)
        {
            var settingType = executionContext.Settings.GetDatabaseSettings()?.KeyType().Value ?? "guid";
            switch (settingType)
            {
                case "guid":
                    return "System.Guid";
                case "int":
                    return "int";
                case "long":
                    return "long";
                default:
                    return settingType;
            }
        }
    }

    internal static class Extensions
    {
        // GCB - this is a bit dirty
        public static int FindIndex(this IEnumerable<CSharpStatement> statements, Func<CSharpStatement, bool> matchFunc, int defaultIfNotFound = 0)
        {
            var found = statements.FirstOrDefault(matchFunc);
            if (found != null)
            {
                return statements.ToList().IndexOf(found);
            }

            return defaultIfNotFound;
        }
    }
}