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
    public class SQLServerDatabaseProjectModel : IMetadataModel, IHasStereotypes, IHasName, IVisualStudioProject, IElementWrapper
    {
        public const string SpecializationType = "SQL Server Database Project";
        public const string SpecializationTypeId = "00D1A9C2-B5F0-4AF3-8072-F6C62B433612";
        protected readonly IElement _element;

        [IntentManaged(Mode.Ignore)]
        public SQLServerDatabaseProjectModel(IElement element, string requiredType = SpecializationType)
        {
            if (!requiredType.Equals(element.SpecializationType, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new Exception($"Cannot create a '{GetType().Name}' from element with specialization type '{element.SpecializationType}'. Must be of type '{SpecializationType}'");
            }
            _element = element;
            RelativeLocation = "";// this.GetCSharpProjectOptions()?.RelativeLocation();
            ParentFolder = element.ParentElement?.SpecializationType == SolutionFolderModel.SpecializationType ? new SolutionFolderModel(element.ParentElement) : null;
            LanguageVersion = null;
            NullableEnabled = false;
        }

        public string RelativeLocation { get; }
        public string LanguageVersion { get; }
        public bool NullableEnabled { get; }
        public string FileExtension => "sqlproj";
        public string Type => SpecializationType;
        public string ProjectTypeId => VisualStudioProjectTypeIds.SQLServerDatabaseProject;
        public SolutionFolderModel ParentFolder { get; }
        public VisualStudioSolutionModel Solution => new VisualStudioSolutionModel(InternalElement.Package);

        public IOutputTargetConfig ToOutputTargetConfig()
        {
            return new ProjectConfig(this);
        }

        public IEnumerable<string> TargetFrameworkVersion()
        {
            return new string[0];
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

        public bool Equals(SQLServerDatabaseProjectModel other)
        {
            return Equals(_element, other?._element);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((SQLServerDatabaseProjectModel)obj);
        }

        public override int GetHashCode()
        {
            return (_element != null ? _element.GetHashCode() : 0);
        }

        public IList<FolderModel> Folders => _element.ChildElements
            .GetElementsOfType(FolderModel.SpecializationTypeId)
            .Select(x => new FolderModel(x))
            .ToList();

        public IList<RoleModel> Roles => _element.ChildElements
            .GetElementsOfType(RoleModel.SpecializationTypeId)
            .Select(x => new RoleModel(x))
            .ToList();

        public IList<SQLCMDVariableModel> SQLCMDVariables => _element.ChildElements
            .GetElementsOfType(SQLCMDVariableModel.SpecializationTypeId)
            .Select(x => new SQLCMDVariableModel(x))
            .ToList();

        public IList<TemplateOutputModel> TemplateOutputs => _element.ChildElements
            .GetElementsOfType(TemplateOutputModel.SpecializationTypeId)
            .Select(x => new TemplateOutputModel(x))
            .ToList();
    }

    [IntentManaged(Mode.Fully)]
    public static class SQLServerDatabaseProjectModelExtensions
    {

        public static bool IsSQLServerDatabaseProjectModel(this ICanBeReferencedType type)
        {
            return type != null && type is IElement element && element.SpecializationTypeId == SQLServerDatabaseProjectModel.SpecializationTypeId;
        }

        public static SQLServerDatabaseProjectModel AsSQLServerDatabaseProjectModel(this ICanBeReferencedType type)
        {
            return type.IsSQLServerDatabaseProjectModel() ? new SQLServerDatabaseProjectModel((IElement)type) : null;
        }
    }
}