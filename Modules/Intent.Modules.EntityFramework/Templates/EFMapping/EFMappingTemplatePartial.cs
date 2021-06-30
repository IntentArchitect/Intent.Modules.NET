using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.RDBMS.Api;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.VisualStudio;
using Intent.Templates;

namespace Intent.Modules.EntityFramework.Templates.EFMapping
{
    partial class EFMappingTemplate : CSharpTemplateBase<ClassModel>, ITemplate, IHasTemplateDependencies, IHasNugetDependencies, IHasDecorators<IEFMappingTemplateDecorator>, ITemplatePostCreationHook
    {
        public const string Identifier = "Intent.EntityFramework.EFMapping";
        private readonly IList<IEFMappingTemplateDecorator> _decorators = new List<IEFMappingTemplateDecorator>();
        private readonly AttributeModel[] _explicitPrimaryKeys;

        public EFMappingTemplate(ClassModel model, IOutputTarget outputTarget)
            : base(Identifier, outputTarget, model)
        {
            AddNugetDependency(NugetPackages.EntityFramework);
            _explicitPrimaryKeys = Model.Attributes.Where(x => x.HasPrimaryKey()).ToArray();
        }

        public string GetEntityName(ClassModel model)
        {
            return GetTypeName(GetMetadata().CustomMetadata["Entity Template Id"], model);
        }

        public bool UseForeignKeys
        {
            get
            {
                if (GetMetadata().CustomMetadata.TryGetValue("Use Foreign Keys", out var useForeignKeysString) && bool.TryParse(useForeignKeysString, out var useForeignKeys))
                {
                    return useForeignKeys;
                }
                return true;
            }
        }

        public bool ImplicitSurrogateKey
        {
            get
            {
                if (GetMetadata().CustomMetadata.TryGetValue("Implicit Surrogate Key", out var useForeignKeysString) && bool.TryParse(useForeignKeysString, out var useForeignKeys))
                {
                    return useForeignKeys;
                }
                return true;
            }
        }

        protected override CSharpFileConfig DefineFileConfig()
        {
            return new CSharpFileConfig(
                className: $"{Model.Name}Mapping",
                @namespace: $"{OutputTarget.GetNamespace()}");
        }

        public void AddDecorator(IEFMappingTemplateDecorator decorator)
        {
            _decorators.Add(decorator);
        }

        public IEnumerable<IEFMappingTemplateDecorator> GetDecorators()
        {
            return _decorators;
        }

        public string PropertyMappings(ClassModel @class)
        {
            return GetDecorators().Aggregate(x => x.PropertyMappings(@class));
        }

        private bool HasTypeOverride(AttributeModel attribute)
        {
            var overrideAttributeStereotype = attribute.GetStereotype("EFMappingOptions");
            if (overrideAttributeStereotype != null)
            {
                var columnType = overrideAttributeStereotype.GetProperty<string>("ColumnType");
                if (!string.IsNullOrEmpty(columnType))
                {
                    return true;
                }
            }

            var overrideTypeStereotype = attribute.Type.Element.GetStereotype("EFMappingOptions");
            if (overrideTypeStereotype != null)
            {
                var columnType = overrideTypeStereotype.GetProperty<string>("ColumnType");
                if (!string.IsNullOrEmpty(columnType))
                {
                    return true;
                }
            }

            return false;
        }

        private string GetTypeOverride(AttributeModel attribute)
        {
            var overrideAttributeStereotype = attribute.GetStereotype("EFMappingOptions");
            if (overrideAttributeStereotype != null)
            {
                var columnType = overrideAttributeStereotype.GetProperty<string>("ColumnType");
                if (!string.IsNullOrEmpty(columnType))
                {
                    return columnType;
                }
            }

            var overrideTypeStereotype = attribute.Type.Element.GetStereotype("EFMappingOptions");
            if (overrideTypeStereotype != null)
            {
                var columnType = overrideTypeStereotype.GetProperty<string>("ColumnType");
                if (!string.IsNullOrEmpty(columnType))
                {
                    return columnType;
                }
            }

            return string.Empty;
        }

        private static string GetForeignKeyLambda(AssociationEndModel associationEnd)
        {
            var foreignKeys = GetForeignKeys(associationEnd);
            if (foreignKeys.Length == 1)
            {
                return $"x => x.{foreignKeys.Single()}";
            }
            return $"x => new {{ {string.Join(", ", foreignKeys.Select(x => "x." + x))}}}";
        }

        private static string[] GetForeignKeys(AssociationEndModel associationEnd)
        {
            return associationEnd.Element.GetStereotypeProperty("Foreign Key", "Column Name", associationEnd.OtherEnd().Name().ToPascalCase() + "Id")
                .Split(',')
                .Select(x => x.Trim())
                .ToArray();
        }

        private static bool RequiresForeignKeyOnAssociatedEnd(AssociationEndModel associationEnd)
        {
            return associationEnd.Multiplicity == Multiplicity.Many
                &&
                (associationEnd.Association.AssociationType == AssociationType.Composition || associationEnd.OtherEnd().IsNavigable);
        }

        private bool UsesImplicitId()
        {
            return Model.ParentClass == null && !_explicitPrimaryKeys.Any();
        }

        private string GetHasKeyLambda()
        {
            var primaryKeys = _explicitPrimaryKeys
                .Select(x => x.Name.ToPascalCase())
                .ToList();

            if (!primaryKeys.Any())
            {
                primaryKeys.Add("Id");
            }

            var compositionalParentAssociation = Model.AssociatedClasses
                .SingleOrDefault(associationEndModel =>
                    associationEndModel.Association.AssociationType == AssociationType.Composition &&
                    associationEndModel.OtherEnd().IsCollection &&
                    !associationEndModel.OtherEnd().IsNullable &&
                    ReferenceEquals(associationEndModel.Association.SourceEnd, associationEndModel));

            if (compositionalParentAssociation != null)
            {
                primaryKeys.AddRange(GetForeignKeys(compositionalParentAssociation.OtherEnd()));
            }

            if (primaryKeys.Count == 1)
            {
                return $"x => x.{primaryKeys.Single()}";
            }

            return $"x => new {{ {string.Join(", ", primaryKeys.Select(primaryKey => $"x.{primaryKey}"))} }}";
        }
    }

    public interface IEFMappingTemplateDecorator : ITemplateDecorator
    {
        string[] PropertyMappings(ClassModel @class);
    }
}
