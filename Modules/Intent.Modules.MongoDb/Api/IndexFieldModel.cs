using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModel", Version = "1.0")]

namespace Intent.MongoDb.Api
{
    [IntentManaged(Mode.Fully, Signature = Mode.Fully)]
    public class IndexFieldModel : IMetadataModel, IHasStereotypes, IHasName
    {
        public const string SpecializationType = "Index Field";
        public const string SpecializationTypeId = "e22a4f98-e62c-428c-8fc8-a6523f72d6e6";
        protected readonly IElement _element;

        [IntentManaged(Mode.Fully)]
        public IndexFieldModel(IElement element, string requiredType = SpecializationType)
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

        public bool Equals(IndexFieldModel other)
        {
            return Equals(_element, other?._element);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((IndexFieldModel)obj);
        }

        public override int GetHashCode()
        {
            return (_element != null ? _element.GetHashCode() : 0);
        }
    }

    [IntentManaged(Mode.Fully)]
    public static class IndexFieldModelExtensions
    {

        public static bool IsIndexFieldModel(this ICanBeReferencedType type)
        {
            return type != null && type is IElement element && element.SpecializationTypeId == IndexFieldModel.SpecializationTypeId;
        }

        public static IndexFieldModel AsIndexFieldModel(this ICanBeReferencedType type)
        {
            return type.IsIndexFieldModel() ? new IndexFieldModel((IElement)type) : null;
        }
    }
}