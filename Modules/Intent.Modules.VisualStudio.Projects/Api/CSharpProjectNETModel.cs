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
    public class CSharpProjectNETModel : IMetadataModel, IHasStereotypes, IHasName, IVisualStudioProject, IElementWrapper
    {
        public const string SpecializationType = "C# Project (.NET)";
        public const string SpecializationTypeId = "8e9e6693-2888-4f48-a0d6-0f163baab740";
        protected readonly IElement _element;

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public CSharpProjectNETModel(IElement element, string requiredType = SpecializationType)
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

        public string Id => _element.Id;

        public string Name => _element.Name;

        public string Type => SpecializationType;

        [IntentIgnore]
        public string ProjectTypeId
        {
            get
            {
                /*
				var dotnetSettings = this.GetNETSettings();
				if (dotnetSettings.SDK().IsMicrosoftNETSdkWeb())
                {
                    return VisualStudioProjectTypeIds.CoreWebApp;
                }

                if (!string.IsNullOrWhiteSpace(dotnetSettings.AzureFunctionsVersion().Value))
                {
                    return VisualStudioProjectTypeIds.AzureFunctionsProject;
                }

                if (dotnetSettings.OutputType().IsConsoleApplication() || dotnetSettings.OutputType().IsWindowsApplication())
                {
                    return VisualStudioProjectTypeIds.CoreConsoleApp;
                }

                return VisualStudioProjectTypeIds.CoreCSharpLibrary;
                */
                return VisualStudioProjectTypeIds.SdkCSharpProject;
            }
        }

        public string RelativeLocation { get; }

        public IOutputTargetConfig ToOutputTargetConfig() => new ProjectConfig(this);

        public IEnumerable<string> TargetFrameworkVersion()
        {
            return !this.GetNETSettings().TargetMultipleFrameworks()
                ? new[] { this.GetNETSettings().TargetFramework().Value }
                : this.GetNETSettings().TargetFrameworks().Select(x => x.Value);
        }

        public SolutionFolderModel ParentFolder { get; }
        public VisualStudioSolutionModel Solution => new(InternalElement.Package);
        public string LanguageVersion { get; }
        public bool NullableEnabled { get; }
        public string FileExtension => "csproj";

        public string Comment => _element.Comment;

        public IEnumerable<IStereotype> Stereotypes => _element.Stereotypes;

        public IElement InternalElement => _element;

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

        public IList<RoleModel> Roles => _element.ChildElements
            .GetElementsOfType(RoleModel.SpecializationTypeId)
            .Select(x => new RoleModel(x))
            .ToList();

        public CustomImplicitUsingsModel CustomImplicitUsings => _element.ChildElements
            .GetElementsOfType(CustomImplicitUsingsModel.SpecializationTypeId)
            .Select(x => new CustomImplicitUsingsModel(x))
            .SingleOrDefault();

        public override string ToString()
        {
            return _element.ToString();
        }

        public bool Equals(CSharpProjectNETModel other)
        {
            return Equals(_element, other?._element);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((CSharpProjectNETModel)obj);
        }

        public override int GetHashCode()
        {
            return (_element != null ? _element.GetHashCode() : 0);
        }
    }

    [IntentManaged(Mode.Fully)]
    public static class CSharpProjectNETModelExtensions
    {

        public static bool IsCSharpProjectNETModel(this ICanBeReferencedType type)
        {
            return type != null && type is IElement element && element.SpecializationTypeId == CSharpProjectNETModel.SpecializationTypeId;
        }

        public static CSharpProjectNETModel AsCSharpProjectNETModel(this ICanBeReferencedType type)
        {
            return type.IsCSharpProjectNETModel() ? new CSharpProjectNETModel((IElement)type) : null;
        }
    }
}