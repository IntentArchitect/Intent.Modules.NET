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
    public class ASPNETWebApplicationNETFrameworkModel : IHasStereotypes, IMetadataModel, IVisualStudioProject, IHasName, IElementWrapper
    {
        public const string SpecializationType = "ASP.NET Web Application (.NET Framework)";
        public const string SpecializationTypeId = "8AF747CF-58F0-449C-8B95-46080FEFC8C0";
        protected readonly IElement _element;

        [IntentManaged(Mode.Ignore)]
        public ASPNETWebApplicationNETFrameworkModel(IElement element, string requiredType = SpecializationType)
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
        public string ProjectTypeId => VisualStudioProjectTypeIds.WebApiApplication;
        public SolutionFolderModel ParentFolder { get; }
        public VisualStudioSolutionModel Solution => new VisualStudioSolutionModel(InternalElement.Package);

        public IOutputTargetConfig ToOutputTargetConfig()
        {
            return new ProjectConfig(this);
        }

        public IEnumerable<string> TargetFrameworkVersion()
        {
            return new[] { new NETFrameworkVersionModel(this.GetNETFrameworkSettings().TargetFramework()).GetNETFrameworkVersionSettings().LegacyVersionIdentifier() };
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
        public bool Equals(ASPNETWebApplicationNETFrameworkModel other)
        {
            return Equals(_element, other?._element);
        }

        [IntentManaged(Mode.Fully)]
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((ASPNETWebApplicationNETFrameworkModel)obj);
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

        public IList<RuntimeEnvironmentModel> RuntimeEnvironments => _element.ChildElements
            .GetElementsOfType(RuntimeEnvironmentModel.SpecializationTypeId)
            .Select(x => new RuntimeEnvironmentModel(x))
            .ToList();

        public IList<TemplateOutputModel> TemplateOutputs => _element.ChildElements
            .GetElementsOfType(TemplateOutputModel.SpecializationTypeId)
            .Select(x => new TemplateOutputModel(x))
            .ToList();

        public string Comment => _element.Comment;
    }

    [IntentManaged(Mode.Fully)]
    public static class ASPNETWebApplicationNETFrameworkModelExtensions
    {

        public static bool IsASPNETWebApplicationNETFrameworkModel(this ICanBeReferencedType type)
        {
            return type != null && type is IElement element && element.SpecializationTypeId == ASPNETWebApplicationNETFrameworkModel.SpecializationTypeId;
        }

        public static ASPNETWebApplicationNETFrameworkModel AsASPNETWebApplicationNETFrameworkModel(this ICanBeReferencedType type)
        {
            return type.IsASPNETWebApplicationNETFrameworkModel() ? new ASPNETWebApplicationNETFrameworkModel((IElement)type) : null;
        }
    }
}