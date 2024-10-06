using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModel", Version = "1.0")]

namespace Intent.Modules.VisualStudio.Projects.Api
{
    [IntentManaged(Mode.Fully, Signature = Mode.Fully)]
    public class CustomImplicitUsingsModel : IMetadataModel, IHasStereotypes, IHasName, IElementWrapper
    {
        public const string SpecializationType = "Custom Implicit Usings";
        public const string SpecializationTypeId = "f18c4808-ef93-4b1d-8bc1-7745579843cf";
        protected readonly IElement _element;

        [IntentManaged(Mode.Fully)]
        public CustomImplicitUsingsModel(IElement element, string requiredType = SpecializationType)
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

        public IList<ImplicitUsingModel> ImplicitUsings => _element.ChildElements
            .GetElementsOfType(ImplicitUsingModel.SpecializationTypeId)
            .Select(x => new ImplicitUsingModel(x))
            .ToList();

        public override string ToString()
        {
            return _element.ToString();
        }

        public bool Equals(CustomImplicitUsingsModel other)
        {
            return Equals(_element, other?._element);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((CustomImplicitUsingsModel)obj);
        }

        public override int GetHashCode()
        {
            return (_element != null ? _element.GetHashCode() : 0);
        }
    }

    [IntentManaged(Mode.Fully)]
    public static class CustomImplicitUsingsModelExtensions
    {

        public static bool IsCustomImplicitUsingsModel(this ICanBeReferencedType type)
        {
            return type != null && type is IElement element && element.SpecializationTypeId == CustomImplicitUsingsModel.SpecializationTypeId;
        }

        public static CustomImplicitUsingsModel AsCustomImplicitUsingsModel(this ICanBeReferencedType type)
        {
            return type.IsCustomImplicitUsingsModel() ? new CustomImplicitUsingsModel((IElement)type) : null;
        }
    }
}