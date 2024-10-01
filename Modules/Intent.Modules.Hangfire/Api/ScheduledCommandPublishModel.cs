using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiAssociationModel", Version = "1.0")]

namespace Intent.Modules.Hangfire.Api
{
    [IntentManaged(Mode.Fully, Signature = Mode.Fully)]
    public class ScheduledCommandPublishModel : IMetadataModel
    {
        public const string SpecializationType = "Scheduled Command Publish";
        public const string SpecializationTypeId = "9fad2dc6-525f-408c-8981-abe958d65268";
        protected readonly IAssociation _association;
        protected ScheduledCommandPublishSourceEndModel _sourceEnd;
        protected ScheduledCommandPublishTargetEndModel _targetEnd;

        public ScheduledCommandPublishModel(IAssociation association, string requiredType = SpecializationType)
        {
            if (!requiredType.Equals(association.SpecializationType, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new Exception($"Cannot create a '{GetType().Name}' from association with specialization type '{association.SpecializationType}'. Must be of type '{SpecializationType}'");
            }
            _association = association;
        }

        public static ScheduledCommandPublishModel CreateFromEnd(IAssociationEnd associationEnd)
        {
            var association = new ScheduledCommandPublishModel(associationEnd.Association);
            return association;
        }

        public string Id => _association.Id;

        public ScheduledCommandPublishSourceEndModel SourceEnd => _sourceEnd ?? (_sourceEnd = new ScheduledCommandPublishSourceEndModel(_association.SourceEnd, this));

        public ScheduledCommandPublishTargetEndModel TargetEnd => _targetEnd ?? (_targetEnd = new ScheduledCommandPublishTargetEndModel(_association.TargetEnd, this));

        public IAssociation InternalAssociation => _association;

        public override string ToString()
        {
            return _association.ToString();
        }

        public bool Equals(ScheduledCommandPublishModel other)
        {
            return Equals(_association, other?._association);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((ScheduledCommandPublishModel)obj);
        }

        public override int GetHashCode()
        {
            return (_association != null ? _association.GetHashCode() : 0);
        }
    }

    [IntentManaged(Mode.Fully)]
    public class ScheduledCommandPublishSourceEndModel : ScheduledCommandPublishEndModel
    {
        public const string SpecializationTypeId = "8fb7e1f2-a7f1-441d-8189-2af97986d4ea";
        public const string SpecializationType = "Scheduled Command Publish Source End";

        public ScheduledCommandPublishSourceEndModel(IAssociationEnd associationEnd, ScheduledCommandPublishModel association) : base(associationEnd, association)
        {
        }
    }

    [IntentManaged(Mode.Fully)]
    public class ScheduledCommandPublishTargetEndModel : ScheduledCommandPublishEndModel
    {
        public const string SpecializationTypeId = "53b123fc-d85e-476a-ad62-d0fe68ed8c1b";
        public const string SpecializationType = "Scheduled Command Publish Target End";

        public ScheduledCommandPublishTargetEndModel(IAssociationEnd associationEnd, ScheduledCommandPublishModel association) : base(associationEnd, association)
        {
        }
    }

    [IntentManaged(Mode.Fully)]
    public class ScheduledCommandPublishEndModel : ITypeReference, IMetadataModel, IHasName, IHasStereotypes, IElementWrapper
    {
        protected readonly IAssociationEnd _associationEnd;
        private readonly ScheduledCommandPublishModel _association;

        public ScheduledCommandPublishEndModel(IAssociationEnd associationEnd, ScheduledCommandPublishModel association)
        {
            _associationEnd = associationEnd;
            _association = association;
        }

        public static ScheduledCommandPublishEndModel Create(IAssociationEnd associationEnd)
        {
            var association = new ScheduledCommandPublishModel(associationEnd.Association);
            return association.TargetEnd.Id == associationEnd.Id ? (ScheduledCommandPublishEndModel)association.TargetEnd : association.SourceEnd;
        }

        public string Id => _associationEnd.Id;
        public string SpecializationType => _associationEnd.SpecializationType;
        public string SpecializationTypeId => _associationEnd.SpecializationTypeId;
        public string Name => _associationEnd.Name;
        public ScheduledCommandPublishModel Association => _association;
        public IElement InternalElement => _associationEnd;
        public IAssociationEnd InternalAssociationEnd => _associationEnd;
        public IAssociation InternalAssociation => _association.InternalAssociation;
        public bool IsNavigable => _associationEnd.IsNavigable;
        public bool IsNullable => _associationEnd.TypeReference.IsNullable;
        public bool IsCollection => _associationEnd.TypeReference.IsCollection;
        public ICanBeReferencedType Element => _associationEnd.TypeReference.Element;
        public IEnumerable<ITypeReference> GenericTypeParameters => _associationEnd.TypeReference.GenericTypeParameters;
        public ITypeReference TypeReference => this;
        public IPackage Package => Element?.Package;
        public string Comment => _associationEnd.Comment;
        public IEnumerable<IStereotype> Stereotypes => _associationEnd.Stereotypes;

        public ScheduledCommandPublishEndModel OtherEnd()
        {
            return this.Equals(_association.SourceEnd) ? (ScheduledCommandPublishEndModel)_association.TargetEnd : (ScheduledCommandPublishEndModel)_association.SourceEnd;
        }

        public bool IsTargetEnd()
        {
            return _associationEnd.IsTargetEnd();
        }

        public bool IsSourceEnd()
        {
            return _associationEnd.IsSourceEnd();
        }

        public override string ToString()
        {
            return _associationEnd.ToString();
        }

        public bool Equals(ScheduledCommandPublishEndModel other)
        {
            return Equals(_associationEnd, other._associationEnd);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((ScheduledCommandPublishEndModel)obj);
        }

        public override int GetHashCode()
        {
            return (_associationEnd != null ? _associationEnd.GetHashCode() : 0);
        }
    }

    [IntentManaged(Mode.Fully)]
    public static class ScheduledCommandPublishEndModelExtensions
    {
        public static bool IsScheduledCommandPublishEndModel(this ICanBeReferencedType type)
        {
            return IsScheduledCommandPublishTargetEndModel(type) || IsScheduledCommandPublishSourceEndModel(type);
        }

        public static ScheduledCommandPublishEndModel AsScheduledCommandPublishEndModel(this ICanBeReferencedType type)
        {
            return (ScheduledCommandPublishEndModel)type.AsScheduledCommandPublishTargetEndModel() ?? (ScheduledCommandPublishEndModel)type.AsScheduledCommandPublishSourceEndModel();
        }

        public static bool IsScheduledCommandPublishTargetEndModel(this ICanBeReferencedType type)
        {
            return type != null && type is IAssociationEnd associationEnd && associationEnd.SpecializationTypeId == ScheduledCommandPublishTargetEndModel.SpecializationTypeId;
        }

        public static ScheduledCommandPublishTargetEndModel AsScheduledCommandPublishTargetEndModel(this ICanBeReferencedType type)
        {
            return type.IsScheduledCommandPublishTargetEndModel() ? new ScheduledCommandPublishModel(((IAssociationEnd)type).Association).TargetEnd : null;
        }

        public static bool IsScheduledCommandPublishSourceEndModel(this ICanBeReferencedType type)
        {
            return type != null && type is IAssociationEnd associationEnd && associationEnd.SpecializationTypeId == ScheduledCommandPublishSourceEndModel.SpecializationTypeId;
        }

        public static ScheduledCommandPublishSourceEndModel AsScheduledCommandPublishSourceEndModel(this ICanBeReferencedType type)
        {
            return type.IsScheduledCommandPublishSourceEndModel() ? new ScheduledCommandPublishModel(((IAssociationEnd)type).Association).SourceEnd : null;
        }
    }
}