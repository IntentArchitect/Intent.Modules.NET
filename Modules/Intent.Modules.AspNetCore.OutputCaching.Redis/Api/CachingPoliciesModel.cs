using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModel", Version = "1.0")]

namespace Intent.AspNetCore.OutputCaching.Redis.Api
{
    [IntentManaged(Mode.Fully, Signature = Mode.Fully)]
    public class CachingPoliciesModel : IMetadataModel, IHasStereotypes, IHasName, IElementWrapper
    {
        public const string SpecializationType = "Caching Policies";
        public const string SpecializationTypeId = "4cd00683-9b6e-4340-a4be-6c6a4e36a274";
        protected readonly IElement _element;

        [IntentManaged(Mode.Fully)]
        public CachingPoliciesModel(IElement element, string requiredType = SpecializationType)
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

        public IList<CachingPolicyModel> CachingPolicies => _element.ChildElements
            .GetElementsOfType(CachingPolicyModel.SpecializationTypeId)
            .Select(x => new CachingPolicyModel(x))
            .ToList();

        public override string ToString()
        {
            return _element.ToString();
        }

        public bool Equals(CachingPoliciesModel other)
        {
            return Equals(_element, other?._element);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((CachingPoliciesModel)obj);
        }

        public override int GetHashCode()
        {
            return (_element != null ? _element.GetHashCode() : 0);
        }
    }

    [IntentManaged(Mode.Fully)]
    public static class CachingPoliciesModelExtensions
    {

        public static bool IsCachingPoliciesModel(this ICanBeReferencedType type)
        {
            return type != null && type is IElement element && element.SpecializationTypeId == CachingPoliciesModel.SpecializationTypeId;
        }

        public static CachingPoliciesModel AsCachingPoliciesModel(this ICanBeReferencedType type)
        {
            return type.IsCachingPoliciesModel() ? new CachingPoliciesModel((IElement)type) : null;
        }
    }
}