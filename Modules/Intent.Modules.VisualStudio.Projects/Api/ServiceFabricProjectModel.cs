using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Configuration;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Constants;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModel", Version = "1.0")]

namespace Intent.Modules.VisualStudio.Projects.Api
{
    [IntentManaged(Mode.Merge)]
    public class ServiceFabricProjectModel : IMetadataModel, IHasStereotypes, IHasName, IElementWrapper, IVisualStudioProject
    {
        public const string SpecializationType = "Service Fabric Project";
        public const string SpecializationTypeId = "0b49ae99-c1c2-46c7-a4e3-41591e0118e1";
        protected readonly IElement _element;

        [IntentManaged(Mode.Merge)]
        public ServiceFabricProjectModel(IElement element, string requiredType = SpecializationType)
        {
            if (!requiredType.Equals(element.SpecializationType, StringComparison.InvariantCultureIgnoreCase) && !requiredType.Equals(element.SpecializationTypeId, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new Exception($"Cannot create a '{GetType().Name}' from element with specialization type '{element.SpecializationType}'. Must be of type '{SpecializationType}'");
            }
            _element = element;
            ParentFolder = element.ParentElement?.SpecializationType == SolutionFolderModel.SpecializationType ? new SolutionFolderModel(element.ParentElement) : null;
        }

        public string Id => _element.Id;

        public string Name => _element.Name;

        public string Comment => _element.Comment;

        public IEnumerable<IStereotype> Stereotypes => _element.Stereotypes;

        public IElement InternalElement => _element;

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

        public ServicesModel Services => _element.ChildElements
            .GetElementsOfType(ServicesModel.SpecializationTypeId)
            .Select(x => new ServicesModel(x))
            .SingleOrDefault();

        #region IVisualStudioProject

        public IOutputTargetConfig ToOutputTargetConfig()
        {
            return new ProjectConfig(this);
        }

        public SolutionFolderModel ParentFolder { get; }
        public string FileExtension => "sfproj";
        public string ProjectTypeId => VisualStudioProjectTypeIds.ServiceFabricProject;
        public string RelativeLocation => string.Empty;
        public VisualStudioSolutionModel Solution => new(InternalElement.Package);
        public string Type => SpecializationType;
        public IEnumerable<string> TargetFrameworkVersion() =>
            [this.GetNETFrameworkSettings()?.TargetFramework().AsNETFrameworkVersionModel()?.GetNETFrameworkVersionSettings()?.LegacyVersionIdentifier() ?? "v4.8"];

        public string LanguageVersion => null;

        public bool NullableEnabled => false;

        #endregion

        [IntentIgnoreComments]
        public override string ToString()
        {
            return _element.ToString();
        }

        public bool Equals(ServiceFabricProjectModel other)
        {
            return Equals(_element, other?._element);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((ServiceFabricProjectModel)obj);
        }

        public override int GetHashCode()
        {
            return (_element != null ? _element.GetHashCode() : 0);
        }
    }

    [IntentManaged(Mode.Fully)]
    public static class ServiceFabricProjectModelExtensions
    {

        public static bool IsServiceFabricProjectModel(this ICanBeReferencedType type)
        {
            return type != null && type is IElement element && element.SpecializationTypeId == ServiceFabricProjectModel.SpecializationTypeId;
        }

        public static ServiceFabricProjectModel AsServiceFabricProjectModel(this ICanBeReferencedType type)
        {
            return type.IsServiceFabricProjectModel() ? new ServiceFabricProjectModel((IElement)type) : null;
        }
    }
}