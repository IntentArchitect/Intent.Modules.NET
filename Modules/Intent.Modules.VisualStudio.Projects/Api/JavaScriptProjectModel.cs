using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Configuration;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.VisualStudio.Projects.Templates.JavaScriptProject;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModel", Version = "1.0")]

namespace Intent.Modules.VisualStudio.Projects.Api
{
    [IntentManaged(Mode.Merge)]
    public class JavaScriptProjectModel : IVisualStudioSolutionProject, IMetadataModel, IHasStereotypes, IHasName, IElementWrapper
    {
        public const string SpecializationType = "JavaScript Project";
        public const string SpecializationTypeId = "69af0b31-9d69-4b5a-a6a2-fc9314f11d92";
        protected readonly IElement _element;

        [IntentManaged(Mode.Fully)]
        public JavaScriptProjectModel(IElement element, string requiredType = SpecializationType)
        {
            if (!requiredType.Equals(element.SpecializationType, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new Exception($"Cannot create a '{GetType().Name}' from element with specialization type '{element.SpecializationType}'. Must be of type '{SpecializationType}'");
            }
            _element = element;
            // IntentIgnore
            ParentFolder = element.ParentElement?.SpecializationType == SolutionFolderModel.SpecializationType ? new SolutionFolderModel(element.ParentElement) : null;
        }

        public string Id => _element.Id;

        public string Name => _element.Name;
        public IOutputTargetConfig ToOutputTargetConfig() => new JavaScriptProjectConfig(this);

        public SolutionFolderModel ParentFolder { get; }
        public string FileExtension => "esproj";

        public string Comment => _element.Comment;

        public IEnumerable<IStereotype> Stereotypes => _element.Stereotypes;

        public IElement InternalElement => _element;


        public string ProjectTypeId => "54A90642-561A-4BB1-A94E-469ADEE60C69";
        public string RelativeLocation => this.GetJavaScriptProjectOptions()?.RelativeLocation();

        public VisualStudioSolutionModel Solution => new(InternalElement.Package);

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

        public bool Equals(JavaScriptProjectModel other)
        {
            return Equals(_element, other?._element);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((JavaScriptProjectModel)obj);
        }

        public override int GetHashCode()
        {
            return (_element != null ? _element.GetHashCode() : 0);
        }
    }

    [IntentManaged(Mode.Fully)]
    public static class JavaScriptProjectModelExtensions
    {

        public static bool IsJavaScriptProjectModel(this ICanBeReferencedType type)
        {
            return type != null && type is IElement element && element.SpecializationTypeId == JavaScriptProjectModel.SpecializationTypeId;
        }

        public static JavaScriptProjectModel AsJavaScriptProjectModel(this ICanBeReferencedType type)
        {
            return type.IsJavaScriptProjectModel() ? new JavaScriptProjectModel((IElement)type) : null;
        }
    }
}