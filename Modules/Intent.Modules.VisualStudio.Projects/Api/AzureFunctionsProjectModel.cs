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
    [IntentManaged(Mode.Merge)]
    public class AzureFunctionsProjectModel : IMetadataModel, IHasStereotypes, IHasName, IVisualStudioProject, IElementWrapper
    {
        public const string SpecializationType = "Azure Functions Project";
        public const string SpecializationTypeId = "73e51385-5e20-4e2c-aa0b-6eb2dc8de52e";
        protected readonly IElement _element;

        [IntentManaged(Mode.Ignore)]
        public AzureFunctionsProjectModel(IElement element, string requiredType = SpecializationType)
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
        public string ProjectTypeId => VisualStudioProjectTypeIds.AzureFunctionsProject;
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

        public string Comment => _element.Comment;

        public IList<FolderModel> Folders => _element.ChildElements
            .GetElementsOfType(FolderModel.SpecializationTypeId)
            .Select(x => new FolderModel(x))
            .ToList();

        public IList<TemplateOutputModel> TemplateOutputs => _element.ChildElements
            .GetElementsOfType(TemplateOutputModel.SpecializationTypeId)
            .Select(x => new TemplateOutputModel(x))
            .ToList();

        public IList<RoleModel> Roles => _element.ChildElements
            .GetElementsOfType(RoleModel.SpecializationTypeId)
            .Select(x => new RoleModel(x))
            .ToList();

        public override string ToString()
        {
            return _element.ToString();
        }

        public bool Equals(AzureFunctionsProjectModel other)
        {
            return Equals(_element, other?._element);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((AzureFunctionsProjectModel)obj);
        }

        public override int GetHashCode()
        {
            return (_element != null ? _element.GetHashCode() : 0);
        }
    }

    [IntentManaged(Mode.Fully)]
    public static class AzureFunctionsProjectModelExtensions
    {

        public static bool IsAzureFunctionsProjectModel(this ICanBeReferencedType type)
        {
            return type != null && type is IElement element && element.SpecializationTypeId == AzureFunctionsProjectModel.SpecializationTypeId;
        }

        public static AzureFunctionsProjectModel AsAzureFunctionsProjectModel(this ICanBeReferencedType type)
        {
            return type.IsAzureFunctionsProjectModel() ? new AzureFunctionsProjectModel((IElement)type) : null;
        }
    }
}