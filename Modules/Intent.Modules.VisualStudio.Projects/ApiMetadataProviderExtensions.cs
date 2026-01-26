using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modelers.CodebaseStructure.Api;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiMetadataProviderExtensions", Version = "1.0")]

namespace Intent.Modules.VisualStudio.Projects.Api
{
    [IntentManaged(Mode.Merge)]
    public static class ApiMetadataProviderExtensions
    {
        [IntentManaged(Mode.Ignore)]
        public static IList<IVisualStudioProject> GetAllProjectModels(this IMetadataManager metadataManager, IApplication application)
        {
            return metadataManager.CodebaseStructure(application).GetASPNETCoreWebApplicationModels().Cast<IVisualStudioProject>()
                .Concat(metadataManager.CodebaseStructure(application).GetASPNETWebApplicationNETFrameworkModels())
                .Concat(metadataManager.CodebaseStructure(application).GetClassLibraryNETCoreModels())
                .Concat(metadataManager.CodebaseStructure(application).GetClassLibraryNETFrameworkModels())
                .Concat(metadataManager.CodebaseStructure(application).GetConsoleAppNETFrameworkModels())
                .Concat(metadataManager.CodebaseStructure(application).GetWCFServiceApplicationModels())
                .Concat(metadataManager.CodebaseStructure(application).GetSQLServerDatabaseProjectModels())
                .Concat(metadataManager.CodebaseStructure(application).GetAzureFunctionsProjectModels())
                .Concat(metadataManager.CodebaseStructure(application).GetConsoleAppNETCoreModels())
                .Concat(metadataManager.CodebaseStructure(application).GetCSharpProjectNETModels())
                .Concat(metadataManager.CodebaseStructure(application).GetServiceFabricProjectModels())
                .ToList();
        }


        public static IList<ASPNETCoreWebApplicationModel> GetASPNETCoreWebApplicationModels(this IDesigner designer)
        {
            return designer.GetElementsOfType(ASPNETCoreWebApplicationModel.SpecializationTypeId)
                .Select(x => new ASPNETCoreWebApplicationModel(x))
                .ToList();
        }

        public static IList<ASPNETWebApplicationNETFrameworkModel> GetASPNETWebApplicationNETFrameworkModels(this IDesigner designer)
        {
            return designer.GetElementsOfType(ASPNETWebApplicationNETFrameworkModel.SpecializationTypeId)
                .Select(x => new ASPNETWebApplicationNETFrameworkModel(x))
                .ToList();
        }

        public static IList<ClassLibraryNETCoreModel> GetClassLibraryNETCoreModels(this IDesigner designer)
        {
            return designer.GetElementsOfType(ClassLibraryNETCoreModel.SpecializationTypeId)
                .Select(x => new ClassLibraryNETCoreModel(x))
                .ToList();
        }

        public static IList<ClassLibraryNETFrameworkModel> GetClassLibraryNETFrameworkModels(this IDesigner designer)
        {
            return designer.GetElementsOfType(ClassLibraryNETFrameworkModel.SpecializationTypeId)
                .Select(x => new ClassLibraryNETFrameworkModel(x))
                .ToList();
        }

        public static IList<ConsoleAppNETFrameworkModel> GetConsoleAppNETFrameworkModels(this IDesigner designer)
        {
            return designer.GetElementsOfType(ConsoleAppNETFrameworkModel.SpecializationTypeId)
                .Select(x => new ConsoleAppNETFrameworkModel(x))
                .ToList();
        }

        public static IList<WCFServiceApplicationModel> GetWCFServiceApplicationModels(this IDesigner designer)
        {
            return designer.GetElementsOfType(WCFServiceApplicationModel.SpecializationTypeId)
                .Select(x => new WCFServiceApplicationModel(x))
                .ToList();
        }

        public static IList<NETCoreVersionModel> GetNETCoreVersionModels(this IDesigner designer)
        {
            return designer.GetElementsOfType(NETCoreVersionModel.SpecializationTypeId)
                .Select(x => new NETCoreVersionModel(x))
                .ToList();
        }

        public static IList<NETFrameworkVersionModel> GetNETFrameworkVersionModels(this IDesigner designer)
        {
            return designer.GetElementsOfType(NETFrameworkVersionModel.SpecializationTypeId)
                .Select(x => new NETFrameworkVersionModel(x))
                .ToList();
        }

        public static IList<SolutionFolderModel> GetSolutionFolderModels(this IDesigner designer)
        {
            return designer.GetElementsOfType(SolutionFolderModel.SpecializationTypeId)
                .Select(x => new SolutionFolderModel(x))
                .ToList();
        }

        public static IList<JavaScriptProjectModel> GetJavaScriptProjectModels(this IDesigner designer)
        {
            return designer.GetElementsOfType(JavaScriptProjectModel.SpecializationTypeId)
                .Select(x => new JavaScriptProjectModel(x))
                .ToList();
        }

        public static IList<RuntimeEnvironmentModel> GetRuntimeEnvironmentModels(this IDesigner designer)
        {
            return designer.GetElementsOfType(RuntimeEnvironmentModel.SpecializationTypeId)
                .Select(x => new RuntimeEnvironmentModel(x))
                .ToList();
        }

        public static IList<ServiceFabricProjectModel> GetServiceFabricProjectModels(this IDesigner designer)
        {
            return designer.GetElementsOfType(ServiceFabricProjectModel.SpecializationTypeId)
                .Select(x => new ServiceFabricProjectModel(x))
                .ToList();
        }

        public static IList<SQLServerDatabaseProjectModel> GetSQLServerDatabaseProjectModels(this IDesigner designer)
        {
            return designer.GetElementsOfType(SQLServerDatabaseProjectModel.SpecializationTypeId)
                .Select(x => new SQLServerDatabaseProjectModel(x))
                .ToList();
        }

        public static IList<VisualStudioSolutionModel> GetVisualStudioSolutionModels(this IDesigner designer)
        {
            return designer.GetElementsOfType(VisualStudioSolutionModel.SpecializationTypeId)
                .Select(x => new VisualStudioSolutionModel(x))
                .ToList();
        }

        public static IList<AzureFunctionsProjectModel> GetAzureFunctionsProjectModels(this IDesigner designer)
        {
            return designer.GetElementsOfType(AzureFunctionsProjectModel.SpecializationTypeId)
                .Select(x => new AzureFunctionsProjectModel(x))
                .ToList();
        }

        public static IList<CSharpProjectNETModel> GetCSharpProjectNETModels(this IDesigner designer)
        {
            return designer.GetElementsOfType(CSharpProjectNETModel.SpecializationTypeId)
                .Select(x => new CSharpProjectNETModel(x))
                .ToList();
        }

        public static IList<ConsoleAppNETCoreModel> GetConsoleAppNETCoreModels(this IDesigner designer)
        {
            return designer.GetElementsOfType(ConsoleAppNETCoreModel.SpecializationTypeId)
                .Select(x => new ConsoleAppNETCoreModel(x))
                .ToList();
        }

    }
}