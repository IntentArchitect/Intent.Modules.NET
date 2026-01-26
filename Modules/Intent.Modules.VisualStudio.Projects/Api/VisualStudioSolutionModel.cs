using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Intent.Exceptions;
using Intent.Metadata.Models;
using Intent.Modelers.CodebaseStructure.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.Types.Api;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModel", Version = "1.0")]

namespace Intent.Modules.VisualStudio.Projects.Api
{
    [IntentManaged(Mode.Merge)]
    public class VisualStudioSolutionModel : IHasStereotypes, IHasName, IElementWrapper, IHasFolder
, IMetadataModel
    {
        public const string SpecializationType = "Visual Studio Solution";
        public const string SpecializationTypeId = "769a45a2-119f-434f-8c27-bd4a399b915c";
        protected readonly IElement _element;

        [IntentManaged(Mode.Fully)]
        public VisualStudioSolutionModel(IElement element, string requiredType = SpecializationTypeId)
        {
            if (!requiredType.Equals(element.SpecializationType, StringComparison.InvariantCultureIgnoreCase) && !requiredType.Equals(element.SpecializationTypeId, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new Exception($"Cannot create a '{GetType().Name}' from element with specialization type '{element.SpecializationType}'. Must be of type '{SpecializationType}'");
            }
            _element = element;
            Folder = _element.ParentElement?.SpecializationTypeId == FolderModel.SpecializationTypeId ? new FolderModel(_element.ParentElement) : null;
        }

        public static VisualStudioSolutionModel GetVisualStudioProject(IElement element)
        {
            var currentElement = element;
            while (currentElement != null)
            {
                if (currentElement.SpecializationTypeId.Equals(SpecializationTypeId, StringComparison.InvariantCultureIgnoreCase))
                {
                    return new VisualStudioSolutionModel(currentElement);
                }

                currentElement = currentElement.ParentElement;
            }

            throw new ElementException(element, "Could not find Visual Studio Solution for element");
        }

        public string Id => _element.Id;
        public string Name => _element.Name;

        public string Comment => _element.Comment;
        public IEnumerable<IStereotype> Stereotypes => _element.Stereotypes;

        public FolderModel Folder { get; }

        public IElement InternalElement => _element;

        public IList<ASPNETCoreWebApplicationModel> ASPNETCoreWebApplications => _element.ChildElements
            .GetElementsOfType(ASPNETCoreWebApplicationModel.SpecializationTypeId)
            .Select(x => new ASPNETCoreWebApplicationModel(x))
            .ToList();

        public IList<ASPNETWebApplicationNETFrameworkModel> ASPNETWebApplicationNETFrameworks => _element.ChildElements
            .GetElementsOfType(ASPNETWebApplicationNETFrameworkModel.SpecializationTypeId)
            .Select(x => new ASPNETWebApplicationNETFrameworkModel(x))
            .ToList();

        public IList<WCFServiceApplicationModel> WCFServiceApplicationNETFrameworks => _element.ChildElements
            .GetElementsOfType(WCFServiceApplicationModel.SpecializationTypeId)
            .Select(x => new WCFServiceApplicationModel(x))
            .ToList();

        public IList<ClassLibraryNETCoreModel> ClassLibraryNETCores => _element.ChildElements
            .GetElementsOfType(ClassLibraryNETCoreModel.SpecializationTypeId)
            .Select(x => new ClassLibraryNETCoreModel(x))
            .ToList();

        public IList<ClassLibraryNETFrameworkModel> ClassLibraryNETFrameworks => _element.ChildElements
            .GetElementsOfType(ClassLibraryNETFrameworkModel.SpecializationTypeId)
            .Select(x => new ClassLibraryNETFrameworkModel(x))
            .ToList();

        public IList<SolutionFolderModel> Folders => _element.ChildElements
            .GetElementsOfType(SolutionFolderModel.SpecializationTypeId)
            .Select(x => new SolutionFolderModel(x))
            .ToList();

        public IList<NETCoreVersionModel> NETCoreVersions => _element.ChildElements
            .GetElementsOfType(NETCoreVersionModel.SpecializationTypeId)
            .Select(x => new NETCoreVersionModel(x))
            .ToList();

        public IList<NETFrameworkVersionModel> NETFrameworkVersions => _element.ChildElements
            .GetElementsOfType(NETFrameworkVersionModel.SpecializationTypeId)
            .Select(x => new NETFrameworkVersionModel(x))
            .ToList();

        public override string ToString()
        {
            return _element.ToString();
        }

        public bool Equals(VisualStudioSolutionModel other)
        {
            return Equals(_element, other?._element);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((VisualStudioSolutionModel)obj);
        }

        public override int GetHashCode()
        {
            return (_element != null ? _element.GetHashCode() : 0);
        }

        public IList<WCFServiceApplicationModel> WCFServiceApplications => _element.ChildElements
            .GetElementsOfType(WCFServiceApplicationModel.SpecializationTypeId)
            .Select(x => new WCFServiceApplicationModel(x))
            .ToList();

        public IList<TemplateOutputModel> TemplateOutputs => _element.ChildElements
            .GetElementsOfType(TemplateOutputModel.SpecializationTypeId)
            .Select(x => new TemplateOutputModel(x))
            .ToList();

        public IList<ConsoleAppNETFrameworkModel> ConsoleAppNETFrameworks => _element.ChildElements
            .GetElementsOfType(ConsoleAppNETFrameworkModel.SpecializationTypeId)
            .Select(x => new ConsoleAppNETFrameworkModel(x))
            .ToList();

        public IList<SQLServerDatabaseProjectModel> SQLServerDatabaseProjects => _element.ChildElements
            .GetElementsOfType(SQLServerDatabaseProjectModel.SpecializationTypeId)
            .Select(x => new SQLServerDatabaseProjectModel(x))
            .ToList();

        public IList<ServiceFabricProjectModel> ServiceFabricProjects => _element.ChildElements
            .GetElementsOfType(ServiceFabricProjectModel.SpecializationTypeId)
            .Select(x => new ServiceFabricProjectModel(x))
            .ToList();

        public IList<OutputAnchorModel> OutputAnchors => _element.ChildElements
            .GetElementsOfType(OutputAnchorModel.SpecializationTypeId)
            .Select(x => new OutputAnchorModel(x))
            .ToList();

        public IList<AzureFunctionsProjectModel> AzureFunctionsProjects => _element.ChildElements
            .GetElementsOfType(AzureFunctionsProjectModel.SpecializationTypeId)
            .Select(x => new AzureFunctionsProjectModel(x))
            .ToList();

        public IList<CSharpProjectNETModel> CSharpProjectNETs => _element.ChildElements
            .GetElementsOfType(CSharpProjectNETModel.SpecializationTypeId)
            .Select(x => new CSharpProjectNETModel(x))
            .ToList();

        public IList<JavaScriptProjectModel> JavaScriptProjects => _element.ChildElements
            .GetElementsOfType(JavaScriptProjectModel.SpecializationTypeId)
            .Select(x => new JavaScriptProjectModel(x))
            .ToList();

        public IList<ConsoleAppNETCoreModel> ConsoleAppNETCores => _element.ChildElements
            .GetElementsOfType(ConsoleAppNETCoreModel.SpecializationTypeId)
            .Select(x => new ConsoleAppNETCoreModel(x))
            .ToList();
    }

    [IntentManaged(Mode.Fully)]
    public static class VisualStudioSolutionModelExtensions
    {

        public static bool IsVisualStudioSolutionModel(this ICanBeReferencedType type)
        {
            return type != null && type is IElement element && element.SpecializationTypeId == VisualStudioSolutionModel.SpecializationTypeId;
        }

        public static VisualStudioSolutionModel AsVisualStudioSolutionModel(this ICanBeReferencedType type)
        {
            return type.IsVisualStudioSolutionModel() ? new VisualStudioSolutionModel((IElement)type) : null;
        }
    }
}