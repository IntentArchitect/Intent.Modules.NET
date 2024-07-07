using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModel", Version = "1.0")]

namespace Intent.Blazor.Components.MudBlazor.Api
{
    [IntentManaged(Mode.Fully, Signature = Mode.Fully)]
    public class DataGridToolbarModel : IMetadataModel, IHasStereotypes, IHasName, IElementWrapper
    {
        public const string SpecializationType = "Data Grid Toolbar";
        public const string SpecializationTypeId = "9af26915-0394-4cda-864d-0e7f9f9943dc";
        protected readonly IElement _element;

        [IntentManaged(Mode.Fully)]
        public DataGridToolbarModel(IElement element, string requiredType = SpecializationType)
        {
            if (!requiredType.Equals(element.SpecializationType, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new Exception($"Cannot create a '{GetType().Name}' from element with specialization type '{element.SpecializationType}'. Must be of type '{SpecializationType}'");
            }
            _element = element;
        }

        public string Id => _element.Id;

        public string Name => _element.Name;

        public string Comment => _element.Comment;

        public IEnumerable<IStereotype> Stereotypes => _element.Stereotypes;

        public IElement InternalElement => _element;

        public override string ToString()
        {
            return _element.ToString();
        }

        public bool Equals(DataGridToolbarModel other)
        {
            return Equals(_element, other?._element);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((DataGridToolbarModel)obj);
        }

        public override int GetHashCode()
        {
            return (_element != null ? _element.GetHashCode() : 0);
        }
    }

    [IntentManaged(Mode.Fully)]
    public static class DataGridToolbarModelExtensions
    {

        public static bool IsDataGridToolbarModel(this ICanBeReferencedType type)
        {
            return type != null && type is IElement element && element.SpecializationTypeId == DataGridToolbarModel.SpecializationTypeId;
        }

        public static DataGridToolbarModel AsDataGridToolbarModel(this ICanBeReferencedType type)
        {
            return type.IsDataGridToolbarModel() ? new DataGridToolbarModel((IElement)type) : null;
        }
    }
}