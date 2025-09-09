using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModel", Version = "1.0")]

namespace Intent.MongoDb.MongoFramework.Api
{
    [IntentManaged(Mode.Fully, Signature = Mode.Fully)]
    public class DocumentStoreIndexModel : IMetadataModel, IHasStereotypes, IHasName, IElementWrapper
    {
        public const string SpecializationType = "Document Store Index";
        public const string SpecializationTypeId = "9f58fea3-953a-4d1b-af88-cd75249387a9";
        protected readonly IElement _element;

        [IntentManaged(Mode.Fully)]
        public DocumentStoreIndexModel(IElement element, string requiredType = SpecializationType)
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

        public bool IsMapped => _element.IsMapped;

        public IElementMapping Mapping => _element.MappedElement;

        public IElement InternalElement => _element;

        public IList<IndexFieldModel> Fields => _element.ChildElements
            .GetElementsOfType(IndexFieldModel.SpecializationTypeId)
            .Select(x => new IndexFieldModel(x))
            .ToList();

        public override string ToString()
        {
            return _element.ToString();
        }

        public bool Equals(DocumentStoreIndexModel other)
        {
            return Equals(_element, other?._element);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((DocumentStoreIndexModel)obj);
        }

        public override int GetHashCode()
        {
            return (_element != null ? _element.GetHashCode() : 0);
        }
    }

    [IntentManaged(Mode.Fully)]
    public static class DocumentStoreIndexModelExtensions
    {

        public static bool IsDocumentStoreIndexModel(this ICanBeReferencedType type)
        {
            return type != null && type is IElement element && element.SpecializationTypeId == DocumentStoreIndexModel.SpecializationTypeId;
        }

        public static DocumentStoreIndexModel AsDocumentStoreIndexModel(this ICanBeReferencedType type)
        {
            return type.IsDocumentStoreIndexModel() ? new DocumentStoreIndexModel((IElement)type) : null;
        }

        public static bool HasSelectFieldsMapping(this DocumentStoreIndexModel type)
        {
            return type.Mapping?.MappingSettingsId == "5ec90845-a65e-406b-9b85-f839e681742d";
        }

        public static IElementMapping GetSelectFieldsMapping(this DocumentStoreIndexModel type)
        {
            return type.HasSelectFieldsMapping() ? type.Mapping : null;
        }
    }
}