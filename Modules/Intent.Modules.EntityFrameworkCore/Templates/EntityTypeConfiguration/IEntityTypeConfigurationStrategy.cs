using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.EntityFrameworkCore.Api;
using Intent.Metadata.RDBMS.Api;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using Intent.Modules.EntityFrameworkCore.Settings;
using Intent.Modules.Metadata.RDBMS.Settings;

namespace Intent.Modules.EntityFrameworkCore.Templates.EntityTypeConfiguration;

public interface IEntityTypeConfigurationStrategy
{
    CSharpStatement GetKeyMapping(ClassModel model);
    IEnumerable<CSharpStatement> GetTableMapping(ClassModel model);
}

public abstract class EntityTypeConfigurationStrategy : IEntityTypeConfigurationStrategy
{
    protected readonly EntityTypeConfigurationTemplate _template;
    protected EntityTypeConfigurationStrategy(EntityTypeConfigurationTemplate template)
    {
        _template = template;
        ExecutionContext = _template.ExecutionContext;
    }

    protected ISoftwareFactoryExecutionContext ExecutionContext { get; }

    public virtual IEnumerable<CSharpStatement> GetTableMapping(ClassModel model)
    {
        if (model.HasTable())
        {
            yield return $@"builder.ToTable(""{model.GetTable()?.Name() ?? model.Name}""{(!string.IsNullOrWhiteSpace(model.GetTable()?.Schema()) ? @$", ""{model.GetTable().Schema() ?? "dbo"}""" : "")});";
        }
        else if (ExecutionContext.Settings.GetDatabaseSettings().InheritanceStrategy().IsTPH() && model.ParentClass != null)
        {
            yield return $@"builder.HasBaseType<{_template.GetTypeName("Domain.Entity", model.ParentClass)}>();";
        }
        else if (ExecutionContext.Settings.GetDatabaseSettings().InheritanceStrategy().IsTPT())
        {
            yield return $@"builder.ToTable(""{model.Name}"");";
        }
        else if (ExecutionContext.Settings.GetDatabaseSettings().InheritanceStrategy().IsTPC() && !model.IsAbstract)
        {
            yield return $@"builder.ToTable(""{model.Name}"");";
        }
    }

    public virtual CSharpStatement GetKeyMapping(ClassModel model)
    {
        if (model.ParentClass != null && (!model.ParentClass.IsAbstract ||
                                          !ExecutionContext.Settings.GetDatabaseSettings().InheritanceStrategy().IsTPC()))
        {
            return null;
        }

        if (!model.GetExplicitPrimaryKey().Any())
        {
            var rootEntity = model;
            while (rootEntity.ParentClass != null)
            {
                rootEntity = rootEntity.ParentClass;
            }

            if (_template.TryGetTemplate<ICSharpFileBuilderTemplate>(TemplateFulfillingRoles.Domain.Entity.Primary, rootEntity.InternalElement, out var template))
            {
                template.CSharpFile.AfterBuild(file =>
                {
                    var @class = file.Classes.First();
                    @class.InsertProperty(0, template.UseType(GetDefaultSurrogateKeyType()), "Id", property =>
                    {
                        @class.AddMetadata("primary-keys", new[] { property });
                    });
                }, int.MinValue);
            }

            return GetKeyMappingStatement("Id");
        }
        else
        {
            if (_template.TryGetTemplate<ICSharpFileBuilderTemplate>(TemplateFulfillingRoles.Domain.Entity.Primary, model, out var template))
            {
                template.CSharpFile.AfterBuild(file =>
                {
                    var @class = file.Classes.First();
                    @class.AddMetadata("primary-keys", @class.GetAllProperties()
                        .Where(x => x.TryGetMetadata<AttributeModel>("model", out var attribute) && attribute.HasPrimaryKey())
                        .ToArray());
                }, int.MinValue);
            }

            return GetKeyMappingStatement(model.GetExplicitPrimaryKey().Select(x => x.Name.ToPascalCase()).ToArray());
        }
    }

    protected CSharpStatement GetKeyMappingStatement(params string[] keyColumns)
    {
        var keys = keyColumns.Count() == 1
            ? "x." + keyColumns[0]
            : $"new {{ {string.Join(", ", keyColumns.Select(key => $"x.{key}"))} }}";

        return $@"builder.HasKey(x => {keys});";
    }

    protected string GetDefaultSurrogateKeyType()
    {
        return GetDefaultSurrogateKeyType(_template.ExecutionContext);
    }

    protected static string GetDefaultSurrogateKeyType(ISoftwareFactoryExecutionContext executionContext)
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

public class CosmosEntityTypeConfiguration : EntityTypeConfigurationStrategy
{
    public CosmosEntityTypeConfiguration(EntityTypeConfigurationTemplate template) : base(template)
    {
    }

    //public override CSharpStatement GetKeyMapping(ClassModel model)
    //{
    //    if (model.ParentClass != null && (!model.ParentClass.IsAbstract ||
    //                                      !ExecutionContext.Settings.GetDatabaseSettings().InheritanceStrategy().IsTPC()))
    //    {
    //        return null;
    //    }

    //    if (!model.GetExplicitPrimaryKey().Any())
    //    {
    //        var rootEntity = model;
    //        while (rootEntity.ParentClass != null)
    //        {
    //            rootEntity = rootEntity.ParentClass;
    //        }
    //        _template.EnsureColumnsOnEntity(rootEntity.InternalElement, new EntityTypeConfigurationTemplate.RequiredColumn(Type: this.GetDefaultSurrogateKeyType(), Name: "Id", ConfigureProperty:
    //            property =>
    //            {
    //                property.MoveToFirst();
    //            }));

    //        //if (model.ChildClasses.Any())
    //        //{
    //        //    return GetKeyMappingStatement(GetPartitionKey(model).Name.ToPascalCase(), "Id");
    //        //}
    //        return GetKeyMappingStatement("Id");
    //    }
    //    else
    //    {
    //        if (_template.TryGetTemplate<ICSharpFileBuilderTemplate>(TemplateFulfillingRoles.Domain.Entity.Primary, model, out var template))
    //        {
    //            template.CSharpFile.AfterBuild(file =>
    //            {
    //                var @class = file.Classes.First();
    //                @class.AddMetadata("primary-keys", @class.GetAllProperties()
    //                    .Where(x => x.TryGetMetadata<AttributeModel>("model", out var attribute) && attribute.HasPrimaryKey())
    //                    .ToArray());
    //            }, int.MinValue);
    //        }

    //        return GetKeyMappingStatement(model.GetExplicitPrimaryKey().Select(x => x.Name.ToPascalCase()).ToArray());
    //    }
    //}

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

    public override IEnumerable<CSharpStatement> GetTableMapping(ClassModel model)
    {
        foreach (var statement in base.GetTableMapping(model))
        {
            yield return statement;
        }
        // Is there an easier way to get this?
        var domainPackage = new DomainPackageModel(model.InternalElement.Package);
        var cosmosSettings = domainPackage.GetCosmosDBContainerSettings();

        if (model.ParentClass == null)
        {
            var containerName = string.IsNullOrWhiteSpace(cosmosSettings?.ContainerName())
                ? _template.ExecutionContext.GetApplicationConfig().Name
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
}

public class RdbmsEntityTypeConfiguration : EntityTypeConfigurationStrategy
{
    public RdbmsEntityTypeConfiguration(EntityTypeConfigurationTemplate template) : base(template)
    {
    }

    public override CSharpStatement GetKeyMapping(ClassModel model)
    {
        if (model.ParentClass != null && (!model.ParentClass.IsAbstract ||
                                          !ExecutionContext.Settings.GetDatabaseSettings().InheritanceStrategy().IsTPC()))
        {
            return null;
        }

        if (!model.GetExplicitPrimaryKey().Any())
        {
            var rootEntity = model;
            while (rootEntity.ParentClass != null)
            {
                rootEntity = rootEntity.ParentClass;
            }

            if (_template.TryGetTemplate<ICSharpFileBuilderTemplate>(TemplateFulfillingRoles.Domain.Entity.Primary, rootEntity.InternalElement, out var template))
            {
                template.CSharpFile.AfterBuild(file =>
                {
                    var @class = file.Classes.First();
                    @class.InsertProperty(0, template.UseType(GetDefaultSurrogateKeyType()), "Id", property =>
                    {
                        @class.AddMetadata("primary-keys", new[] { property });
                    });
                }, int.MinValue);
            }

            return GetKeyMappingStatement("Id");
        }
        else
        {
            if (_template.TryGetTemplate<ICSharpFileBuilderTemplate>(TemplateFulfillingRoles.Domain.Entity.Primary, model, out var template))
            {
                template.CSharpFile.AfterBuild(file =>
                {
                    var @class = file.Classes.First();
                    @class.AddMetadata("primary-keys", @class.GetAllProperties()
                        .Where(x => x.TryGetMetadata<AttributeModel>("model", out var attribute) && attribute.HasPrimaryKey())
                        .ToArray());
                }, int.MinValue);
            }

            return GetKeyMappingStatement(model.GetExplicitPrimaryKey().Select(x => x.Name.ToPascalCase()).ToArray());
        }
    }
}