using System.Linq;
using Intent.Engine;
using Intent.Metadata.RDBMS.Api;
using Intent.Modelers.Domain.Api;
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

    public abstract CSharpStatement GetKeyMapping(ClassModel model);

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

    public override CSharpStatement GetKeyMapping(ClassModel model)
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
            return string.Empty;
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

            return $@"builder.HasKey(x => x.Id);";
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

            var keys = model.GetExplicitPrimaryKey().Count() == 1
                ? "x." + model.GetExplicitPrimaryKey().Single().Name.ToPascalCase()
                : $"new {{ {string.Join(", ", model.GetExplicitPrimaryKey().Select(x => "x." + x.Name.ToPascalCase()))} }}";

            return $@"builder.HasKey(x => {keys});";
        }
    }
}