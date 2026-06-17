using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Modelers.UI.Api;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModel", Version = "1.0")]

namespace Intent.Blazor.Components.MudBlazor.Api
{
    [IntentManaged(Mode.Fully, Signature = Mode.Fully)]
    public class UserMenuModel : IMetadataModel, IHasStereotypes, IHasName, IElementWrapper, IComponentModel
    {
        public const string SpecializationType = "UserMenu";
        public const string SpecializationTypeId = "ae3e1952-77cc-4e5d-8c74-5043efbe1750";
        protected readonly IElement _element;

        [IntentManaged(Mode.Fully)]
        public UserMenuModel(IElement element, string requiredType = SpecializationTypeId)
        {
            if (!requiredType.Equals(element.SpecializationType, StringComparison.InvariantCultureIgnoreCase) && !requiredType.Equals(element.SpecializationTypeId, StringComparison.InvariantCultureIgnoreCase))
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

        public bool Equals(UserMenuModel other)
        {
            return Equals(_element, other?._element);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((UserMenuModel)obj);
        }

        public override int GetHashCode()
        {
            return (_element != null ? _element.GetHashCode() : 0);
        }
    }

    [IntentManaged(Mode.Fully)]
    public static class UserMenuModelExtensions
    {

        public static bool IsUserMenuModel(this ICanBeReferencedType type)
        {
            return type != null && type is IElement element && element.SpecializationTypeId == UserMenuModel.SpecializationTypeId;
        }

        public static UserMenuModel AsUserMenuModel(this ICanBeReferencedType type)
        {
            return type.IsUserMenuModel() ? new UserMenuModel((IElement)type) : null;
        }
    }
}