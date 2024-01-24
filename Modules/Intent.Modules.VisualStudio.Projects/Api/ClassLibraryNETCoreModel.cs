using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Configuration;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Constants;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModel", Version = "1.0")]

namespace Intent.Modules.VisualStudio.Projects.Api
{
    [IntentManaged(Mode.Merge, Signature = Mode.Merge)]
    public class ClassLibraryNETCoreModel : IHasStereotypes, IMetadataModel, IVisualStudioProject, IHasName, IElementWrapper
    {
        public const string SpecializationType = "Class Library (.NET Core)";
        public const string SpecializationTypeId = "52B49DB5-3EA9-4095-B1A7-DF1AC22D7DAE";
        protected readonly IElement _element;

        [IntentManaged(Mode.Ignore)]
        public ClassLibraryNETCoreModel(IElement element, string requiredType = SpecializationType)
        {
            if (!requiredType.Equals(element.SpecializationType, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new Exception($"Cannot create a '{GetType().Name}' from element with specialization type '{element.SpecializationType}'. Must be of type '{SpecializationType}'");
            }
            _element = element;
            RelativeLocation = this.GetCSharpProjectOptions()?.RelativeLocation();
            ParentFolder = element.ParentElement?.SpecializationType == SolutionFolderModel.SpecializationType ? new SolutionFolderModel(element.ParentElement) : null;
            LanguageVersion = this.GetCSharpProjectOptions()?.LanguageVersion()?.Value;
            NullableEnabled = this.NullableIsEnabled();
        }

        public string RelativeLocation { get; }
        public string LanguageVersion { get; }
        public bool NullableEnabled { get; }
        public string FileExtension => "csproj";
        public string Type => SpecializationType;
        public string ProjectTypeId => VisualStudioProjectTypeIds.CoreCSharpLibrary;
        public SolutionFolderModel ParentFolder { get; }
        public VisualStudioSolutionModel Solution => new VisualStudioSolutionModel(InternalElement.Package);

        public IOutputTargetConfig ToOutputTargetConfig()
        {
            return new ProjectConfig(this);
        }

        public IEnumerable<string> TargetFrameworkVersion()
        {
            return !this.GetNETCoreSettings().TargetMultipleFrameworks()
                ? new[] { this.GetNETCoreSettings().TargetFramework().Value }
                : this.GetNETCoreSettings().TargetFrameworks().Select(x => x.Value);
        }

        [IntentManaged(Mode.Fully)]
        public string Id => _element.Id;


        [IntentManaged(Mode.Fully)]
        public string Name => _element.Name;

        [IntentManaged(Mode.Fully)]
        public IEnumerable<IStereotype> Stereotypes => _element.Stereotypes;

        [IntentManaged(Mode.Fully)]
        public IElement InternalElement => _element;

        [IntentManaged(Mode.Fully)]
        public IList<RoleModel> Roles => _element.ChildElements
            .GetElementsOfType(RoleModel.SpecializationTypeId)
            .Select(x => new RoleModel(x))
            .ToList();

        [IntentManaged(Mode.Fully)]
        public override string ToString()
        {
            return _element.ToString();
        }

        [IntentManaged(Mode.Fully)]
        public bool Equals(ClassLibraryNETCoreModel other)
        {
            return Equals(_element, other?._element);
        }

        [IntentManaged(Mode.Fully)]
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((ClassLibraryNETCoreModel)obj);
        }

        [IntentManaged(Mode.Fully)]
        public override int GetHashCode()
        {
            return (_element != null ? _element.GetHashCode() : 0);
        }

        [IntentManaged(Mode.Fully)]
        public IList<FolderModel> Folders => _element.ChildElements
            .GetElementsOfType(FolderModel.SpecializationTypeId)
            .Select(x => new FolderModel(x))
            .ToList();

        public IList<TemplateOutputModel> TemplateOutputs => _element.ChildElements
            .GetElementsOfType(TemplateOutputModel.SpecializationTypeId)
            .Select(x => new TemplateOutputModel(x))
            .ToList();

        public string Comment => _element.Comment;

    }

    [IntentManaged(Mode.Fully)]
    public static class ClassLibraryNETCoreModelExtensions
    {

        public static bool IsClassLibraryNETCoreModel(this ICanBeReferencedType type)
        {
            return type != null && type is IElement element && element.SpecializationTypeId == ClassLibraryNETCoreModel.SpecializationTypeId;
        }

        public static ClassLibraryNETCoreModel AsClassLibraryNETCoreModel(this ICanBeReferencedType type)
        {
            return type.IsClassLibraryNETCoreModel() ? new ClassLibraryNETCoreModel((IElement)type) : null;
        }
    }
}