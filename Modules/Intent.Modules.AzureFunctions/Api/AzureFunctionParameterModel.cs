using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.AzureFunctions.Templates.AzureFunctionClass;
using Intent.Modules.Common;
using Intent.Modules.Metadata.WebApi.Models;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModel", Version = "1.0")]

namespace Intent.AzureFunctions.Api
{
    [IntentManaged(Mode.Fully, Signature = Mode.Merge)]
    public class AzureFunctionParameterModel : IMetadataModel, IHasStereotypes, IHasName, IHasTypeReference, IAzureFunctionParameterModel, IElementWrapper
    {
        public const string SpecializationType = "Azure Function Parameter";
        public const string SpecializationTypeId = "b6d4f537-eebe-4c56-8cb0-7687cf5bbe16";
        protected readonly IElement _element;

        [IntentManaged(Mode.Fully)]
        public AzureFunctionParameterModel(IElement element, string requiredType = SpecializationType)
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

        public ITypeReference TypeReference => _element.TypeReference;

        public ITypeReference Type => TypeReference?.Element != null ? TypeReference : null;

        [IntentManaged(Mode.Ignore)]
        public HttpInputSource? InputSource => HttpEndpointModelFactory.GetHttpInputSource(InternalElement);

        [IntentManaged(Mode.Ignore)]
        public bool IsMapped => InternalElement.IsMapped;

        [IntentManaged(Mode.Ignore)]
        public string MappedPath => InternalElement.MappedElement?.Element.Name;

        public IElement InternalElement => _element;

        public override string ToString()
        {
            return _element.ToString();
        }

        public bool Equals(AzureFunctionParameterModel other)
        {
            return Equals(_element, other?._element);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((AzureFunctionParameterModel)obj);
        }

        public override int GetHashCode()
        {
            return (_element != null ? _element.GetHashCode() : 0);
        }
    }

    [IntentManaged(Mode.Fully)]
    public static class AzureFunctionParameterModelExtensions
    {

        public static bool IsAzureFunctionParameterModel(this ICanBeReferencedType type)
        {
            return type != null && type is IElement element && element.SpecializationTypeId == AzureFunctionParameterModel.SpecializationTypeId;
        }

        public static AzureFunctionParameterModel AsAzureFunctionParameterModel(this ICanBeReferencedType type)
        {
            return type.IsAzureFunctionParameterModel() ? new AzureFunctionParameterModel((IElement)type) : null;
        }
    }
}