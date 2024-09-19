using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModel", Version = "1.0")]

namespace Intent.Modules.Hangfire.Api
{
    [IntentManaged(Mode.Fully, Signature = Mode.Fully)]
    public class HangfireJobModel : IMetadataModel, IHasStereotypes, IHasName, IElementWrapper
    {
        public const string SpecializationType = "Hangfire Job";
        public const string SpecializationTypeId = "995f1902-4efb-42ba-b089-a5e856c48943";
        protected readonly IElement _element;

        [IntentManaged(Mode.Fully)]
        public HangfireJobModel(IElement element, string requiredType = SpecializationType)
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

        public bool Equals(HangfireJobModel other)
        {
            return Equals(_element, other?._element);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((HangfireJobModel)obj);
        }

        public override int GetHashCode()
        {
            return (_element != null ? _element.GetHashCode() : 0);
        }
    }

    [IntentManaged(Mode.Fully)]
    public static class HangfireJobModelExtensions
    {

        public static bool IsHangfireJobModel(this ICanBeReferencedType type)
        {
            return type != null && type is IElement element && element.SpecializationTypeId == HangfireJobModel.SpecializationTypeId;
        }

        public static HangfireJobModel AsHangfireJobModel(this ICanBeReferencedType type)
        {
            return type.IsHangfireJobModel() ? new HangfireJobModel((IElement)type) : null;
        }
    }
}