using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiPackageModel", Version = "1.0")]

namespace Intent.Modules.VisualStudio.Projects.Api
{
    [IntentManaged(Mode.Merge)]
    public class VisualStudioSolutionModel : IHasStereotypes, IMetadataModel
    {
        public const string SpecializationType = "Visual Studio Solution";
        public const string SpecializationTypeId = "07e7b690-a59d-4b72-8440-4308a121d32c";

        [IntentManaged(Mode.Ignore)]
        public VisualStudioSolutionModel(IPackage package)
        {
            if (!SpecializationTypeId.Equals(package.SpecializationTypeId, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new Exception($"Cannot create a '{GetType().Name}' from package with specialization type '{package.SpecializationType}'. Must be of type '{SpecializationType}'");
            }

            UnderlyingPackage = package;
        }

        public IPackage UnderlyingPackage { get; }
        public string Id => UnderlyingPackage.Id;
        public string Name => UnderlyingPackage.Name;
        public IEnumerable<IStereotype> Stereotypes => UnderlyingPackage.Stereotypes;

        public string FileLocation => UnderlyingPackage.FileLocation;

        public IList<ASPNETCoreWebApplicationModel> ASPNETCoreWebApplications => UnderlyingPackage.ChildElements
            .GetElementsOfType(ASPNETCoreWebApplicationModel.SpecializationTypeId)
            .Select(x => new ASPNETCoreWebApplicationModel(x))
            .ToList();

        public IList<ASPNETWebApplicationNETFrameworkModel> ASPNETWebApplicationNETFrameworks => UnderlyingPackage.ChildElements
            .GetElementsOfType(ASPNETWebApplicationNETFrameworkModel.SpecializationTypeId)
            .Select(x => new ASPNETWebApplicationNETFrameworkModel(x))
            .ToList();

        public IList<WCFServiceApplicationModel> WCFServiceApplicationNETFrameworks => UnderlyingPackage.ChildElements
            .GetElementsOfType(WCFServiceApplicationModel.SpecializationTypeId)
            .Select(x => new WCFServiceApplicationModel(x))
            .ToList();

        public IList<ClassLibraryNETCoreModel> ClassLibraryNETCores => UnderlyingPackage.ChildElements
            .GetElementsOfType(ClassLibraryNETCoreModel.SpecializationTypeId)
            .Select(x => new ClassLibraryNETCoreModel(x))
            .ToList();

        public IList<ClassLibraryNETFrameworkModel> ClassLibraryNETFrameworks => UnderlyingPackage.ChildElements
            .GetElementsOfType(ClassLibraryNETFrameworkModel.SpecializationTypeId)
            .Select(x => new ClassLibraryNETFrameworkModel(x))
            .ToList();

        public IList<SolutionFolderModel> Folders => UnderlyingPackage.ChildElements
            .GetElementsOfType(SolutionFolderModel.SpecializationTypeId)
            .Select(x => new SolutionFolderModel(x))
            .ToList();

        public IList<NETCoreVersionModel> NETCoreVersions => UnderlyingPackage.ChildElements
            .GetElementsOfType(NETCoreVersionModel.SpecializationTypeId)
            .Select(x => new NETCoreVersionModel(x))
            .ToList();

        public IList<NETFrameworkVersionModel> NETFrameworkVersions => UnderlyingPackage.ChildElements
            .GetElementsOfType(NETFrameworkVersionModel.SpecializationTypeId)
            .Select(x => new NETFrameworkVersionModel(x))
            .ToList();

        public IList<RoleModel> Roles => UnderlyingPackage.ChildElements
            .GetElementsOfType(RoleModel.SpecializationTypeId)
            .Select(x => new RoleModel(x))
            .ToList();

        public IList<WCFServiceApplicationModel> WCFServiceApplications => UnderlyingPackage.ChildElements
            .GetElementsOfType(WCFServiceApplicationModel.SpecializationTypeId)
            .Select(x => new WCFServiceApplicationModel(x))
            .ToList();

        public IList<TemplateOutputModel> TemplateOutputs => UnderlyingPackage.ChildElements
            .GetElementsOfType(TemplateOutputModel.SpecializationTypeId)
            .Select(x => new TemplateOutputModel(x))
            .ToList();

        public IList<ConsoleAppNETFrameworkModel> ConsoleAppNETFrameworks => UnderlyingPackage.ChildElements
            .GetElementsOfType(ConsoleAppNETFrameworkModel.SpecializationTypeId)
            .Select(x => new ConsoleAppNETFrameworkModel(x))
            .ToList();

        public IList<SQLServerDatabaseProjectModel> SQLServerDatabaseProjects => UnderlyingPackage.ChildElements
            .GetElementsOfType(SQLServerDatabaseProjectModel.SpecializationTypeId)
            .Select(x => new SQLServerDatabaseProjectModel(x))
            .ToList();

        public IList<AzureFunctionsProjectModel> AzureFunctionsProjects => UnderlyingPackage.ChildElements
            .GetElementsOfType(AzureFunctionsProjectModel.SpecializationTypeId)
            .Select(x => new AzureFunctionsProjectModel(x))
            .ToList();

        public IList<CSharpProjectNETModel> CSharpProjectNETs => UnderlyingPackage.ChildElements
            .GetElementsOfType(CSharpProjectNETModel.SpecializationTypeId)
            .Select(x => new CSharpProjectNETModel(x))
            .ToList();

        public IList<JavaScriptProjectModel> JavaScriptProjects => UnderlyingPackage.ChildElements
            .GetElementsOfType(JavaScriptProjectModel.SpecializationTypeId)
            .Select(x => new JavaScriptProjectModel(x))
            .ToList();

        public IList<ConsoleAppNETCoreModel> ConsoleAppNETCores => UnderlyingPackage.ChildElements
            .GetElementsOfType(ConsoleAppNETCoreModel.SpecializationTypeId)
            .Select(x => new ConsoleAppNETCoreModel(x))
            .ToList();
    }
}