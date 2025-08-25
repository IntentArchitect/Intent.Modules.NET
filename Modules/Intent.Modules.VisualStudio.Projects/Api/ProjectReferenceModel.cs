using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiAssociationModel", Version = "1.0")]

namespace Intent.Modules.VisualStudio.Projects.Api
{
    [IntentManaged(Mode.Fully, Signature = Mode.Fully)]
    public class ProjectReferenceModel : IMetadataModel
    {
        public const string SpecializationType = "Project Reference";
        public const string SpecializationTypeId = "08ed7993-fa72-4359-aa1e-b4389a32edec";
        protected readonly IAssociation _association;
        protected ProjectReferenceSourceEndModel _sourceEnd;
        protected ProjectReferenceTargetEndModel _targetEnd;

        public ProjectReferenceModel(IAssociation association, string requiredType = SpecializationType)
        {
            if (!requiredType.Equals(association.SpecializationType, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new Exception($"Cannot create a '{GetType().Name}' from association with specialization type '{association.SpecializationType}'. Must be of type '{SpecializationType}'");
            }
            _association = association;
        }

        public static ProjectReferenceModel CreateFromEnd(IAssociationEnd associationEnd)
        {
            var association = new ProjectReferenceModel(associationEnd.Association);
            return association;
        }

        public string Id => _association.Id;

        public ProjectReferenceSourceEndModel SourceEnd => _sourceEnd ??= new ProjectReferenceSourceEndModel(_association.SourceEnd, this);

        public ProjectReferenceTargetEndModel TargetEnd => _targetEnd ??= new ProjectReferenceTargetEndModel(_association.TargetEnd, this);

        public IAssociation InternalAssociation => _association;

        public override string ToString()
        {
            return _association.ToString();
        }

        public bool Equals(ProjectReferenceModel other)
        {
            return Equals(_association, other?._association);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((ProjectReferenceModel)obj);
        }

        public override int GetHashCode()
        {
            return (_association != null ? _association.GetHashCode() : 0);
        }
    }

    [IntentManaged(Mode.Fully)]
    public class ProjectReferenceSourceEndModel : ProjectReferenceEndModel
    {
        public const string SpecializationTypeId = "bf6b54c8-e305-41d5-a08d-52d5325d05b2";
        public const string SpecializationType = "Project Reference Source End";

        public ProjectReferenceSourceEndModel(IAssociationEnd associationEnd, ProjectReferenceModel association) : base(associationEnd, association)
        {
        }
    }

    [IntentManaged(Mode.Fully)]
    public class ProjectReferenceTargetEndModel : ProjectReferenceEndModel
    {
        public const string SpecializationTypeId = "dea99762-17e6-41f7-acb5-6b3c28969e3d";
        public const string SpecializationType = "Project Reference Target End";

        public ProjectReferenceTargetEndModel(IAssociationEnd associationEnd, ProjectReferenceModel association) : base(associationEnd, association)
        {
        }
    }

    [IntentManaged(Mode.Fully)]
    public class ProjectReferenceEndModel : ITypeReference, IMetadataModel, IHasName, IHasStereotypes, IElementWrapper
    {
        protected readonly IAssociationEnd _associationEnd;
        private readonly ProjectReferenceModel _association;

        public ProjectReferenceEndModel(IAssociationEnd associationEnd, ProjectReferenceModel association)
        {
            _associationEnd = associationEnd;
            _association = association;
        }

        public static ProjectReferenceEndModel Create(IAssociationEnd associationEnd)
        {
            var association = new ProjectReferenceModel(associationEnd.Association);
            return association.TargetEnd.Id == associationEnd.Id ? (ProjectReferenceEndModel)association.TargetEnd : association.SourceEnd;
        }

        public string Id => _associationEnd.Id;
        public string SpecializationType => _associationEnd.SpecializationType;
        public string SpecializationTypeId => _associationEnd.SpecializationTypeId;
        public string Name => _associationEnd.Name;
        public ProjectReferenceModel Association => _association;
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

        public ProjectReferenceEndModel OtherEnd()
        {
            return this.Equals(_association.SourceEnd) ? (ProjectReferenceEndModel)_association.TargetEnd : (ProjectReferenceEndModel)_association.SourceEnd;
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

        public bool Equals(ProjectReferenceEndModel other)
        {
            return Equals(_associationEnd, other._associationEnd);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((ProjectReferenceEndModel)obj);
        }

        public override int GetHashCode()
        {
            return (_associationEnd != null ? _associationEnd.GetHashCode() : 0);
        }
    }

    [IntentManaged(Mode.Fully)]
    public static class ProjectReferenceEndModelExtensions
    {
        public static bool IsProjectReferenceEndModel(this ICanBeReferencedType type)
        {
            return IsProjectReferenceTargetEndModel(type) || IsProjectReferenceSourceEndModel(type);
        }

        public static ProjectReferenceEndModel AsProjectReferenceEndModel(this ICanBeReferencedType type)
        {
            return (ProjectReferenceEndModel)type.AsProjectReferenceTargetEndModel() ?? (ProjectReferenceEndModel)type.AsProjectReferenceSourceEndModel();
        }

        public static bool IsProjectReferenceTargetEndModel(this ICanBeReferencedType type)
        {
            return type != null && type is IAssociationEnd associationEnd && associationEnd.SpecializationTypeId == ProjectReferenceTargetEndModel.SpecializationTypeId;
        }

        public static ProjectReferenceTargetEndModel AsProjectReferenceTargetEndModel(this ICanBeReferencedType type)
        {
            return type.IsProjectReferenceTargetEndModel() ? new ProjectReferenceModel(((IAssociationEnd)type).Association).TargetEnd : null;
        }

        public static bool IsProjectReferenceSourceEndModel(this ICanBeReferencedType type)
        {
            return type != null && type is IAssociationEnd associationEnd && associationEnd.SpecializationTypeId == ProjectReferenceSourceEndModel.SpecializationTypeId;
        }

        public static ProjectReferenceSourceEndModel AsProjectReferenceSourceEndModel(this ICanBeReferencedType type)
        {
            return type.IsProjectReferenceSourceEndModel() ? new ProjectReferenceModel(((IAssociationEnd)type).Association).SourceEnd : null;
        }
    }
}