using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modelers.Services.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.Types.Api;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModel", Version = "1.0")]

namespace Intent.AzureFunctions.Api
{
    [IntentManaged(Mode.Fully, Signature = Mode.Fully)]
    public class AzureFunctionModel : IMetadataModel, IHasStereotypes, IHasName, IHasTypeReference, IHasFolder
    {
        public const string SpecializationType = "Azure Function";
        public const string SpecializationTypeId = "702f57ca-3b5a-413b-a084-9f2d154154e7";
        protected readonly IElement _element;

        [IntentManaged(Mode.Fully)]
        public AzureFunctionModel(IElement element, string requiredType = SpecializationType)
        {
            if (!requiredType.Equals(element.SpecializationType, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new Exception($"Cannot create a '{GetType().Name}' from element with specialization type '{element.SpecializationType}'. Must be of type '{SpecializationType}'");
            }
            _element = element;
            Folder = _element.ParentElement?.SpecializationTypeId == FolderModel.SpecializationTypeId ? new FolderModel(_element.ParentElement) : null;
        }

        public string Id => _element.Id;

        public string Name => _element.Name;

        public string Comment => _element.Comment;

        public IEnumerable<IStereotype> Stereotypes => _element.Stereotypes;

        public FolderModel Folder { get; }

        public IEnumerable<string> GenericTypes => _element.GenericTypes.Select(x => x.Name);

        public ITypeReference TypeReference => _element.TypeReference;

        public bool IsMapped => _element.IsMapped;

        public IElementMapping Mapping => _element.MappedElement;

        public IElement InternalElement => _element;

        public IList<ParameterModel> Parameters => _element.ChildElements
            .GetElementsOfType(ParameterModel.SpecializationTypeId)
            .Select(x => new ParameterModel(x))
            .ToList();

        public override string ToString()
        {
            return _element.ToString();
        }

        public bool Equals(AzureFunctionModel other)
        {
            return Equals(_element, other?._element);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((AzureFunctionModel)obj);
        }

        public override int GetHashCode()
        {
            return (_element != null ? _element.GetHashCode() : 0);
        }
    }

    [IntentManaged(Mode.Fully)]
    public static class AzureFunctionModelExtensions
    {

        public static bool IsAzureFunctionModel(this ICanBeReferencedType type)
        {
            return type != null && type is IElement element && element.SpecializationTypeId == AzureFunctionModel.SpecializationTypeId;
        }

        public static AzureFunctionModel AsAzureFunctionModel(this ICanBeReferencedType type)
        {
            return type.IsAzureFunctionModel() ? new AzureFunctionModel((IElement)type) : null;
        }

        public static bool HasMapToServiceOperationMapping(this AzureFunctionModel type)
        {
            return type.Mapping?.MappingSettingsId == "95923466-5e26-49ce-8824-db29d0d2d8b0";
        }

        public static IElementMapping GetMapToServiceOperationMapping(this AzureFunctionModel type)
        {
            return type.HasMapToServiceOperationMapping() ? type.Mapping : null;
        }

        public static bool HasMapToCommandMapping(this AzureFunctionModel type)
        {
            return type.Mapping?.MappingSettingsId == "6cc1e01d-75fa-48f7-ad4b-09a4c305daf1";
        }

        public static IElementMapping GetMapToCommandMapping(this AzureFunctionModel type)
        {
            return type.HasMapToCommandMapping() ? type.Mapping : null;
        }

        public static bool HasMapToQueryMapping(this AzureFunctionModel type)
        {
            return type.Mapping?.MappingSettingsId == "29493763-0166-48fb-b79e-7e9d6a9996f1";
        }

        public static IElementMapping GetMapToQueryMapping(this AzureFunctionModel type)
        {
            return type.HasMapToQueryMapping() ? type.Mapping : null;
        }
    }
}