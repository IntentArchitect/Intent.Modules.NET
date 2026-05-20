using System.Collections.Generic;
using System.Linq;
using NuGet.Versioning;
using Intent.Engine;
using Intent.Exceptions;
using Intent.Modelers.CodebaseStructure.Api;
using Intent.Modules.Common.Types.Api;
using Intent.Modules.VisualStudio.Projects.Api;
using Intent.Registrations;

namespace Intent.Modules.VisualStudio.Projects.OutputTargets
{
    public class OutputTargetRegistration : IOutputTargetRegistration
    {
        private readonly IMetadataManager _metadataManager;
        private readonly IApplicationConfigurationProvider _configurationProvider;

        public OutputTargetRegistration(IMetadataManager metadataManager, IApplicationConfigurationProvider configurationProvider)
        {
            _metadataManager = metadataManager;
            _configurationProvider = configurationProvider;
        }

        public void Register(IOutputTargetRegistry registry, IApplication application)
        {
            var models = _metadataManager.GetAllProjectModels(application);
            foreach (var model in models)
            {
                registry.RegisterOutputTarget(model.ToOutputTargetConfig());
                foreach (var folder in model.Folders.DetectDuplicates())
                {
                    Register(registry, folder);
                }
            }

            // the below block has been moved into the Codebase Structure designer module. However, cannot be removed from here due to 
            // dependency and build ordering conflicts.
            if (AddRootFolderModels())
            {
                var rootFolders = _metadataManager.CodebaseStructure(application).GetRootFolderModels();
                foreach (var rootFolder in rootFolders)
                {
                    registry.RegisterOutputTarget(rootFolder.ToOutputTargetConfig());
                    foreach (var f in rootFolder.Folders)
                    {
                        Register(registry, f);
                    }
                }
            }

            var solutionFolders = _metadataManager.CodebaseStructure(application).GetSolutionFolderModels();
            foreach (var solutionFolder in solutionFolders)
            {
                registry.RegisterOutputTarget(solutionFolder.ToOutputTarget());
            }

            var javaScriptProjects = _metadataManager.CodebaseStructure(application).GetJavaScriptProjectModels();
            foreach (var model in javaScriptProjects)
            {
                registry.RegisterOutputTarget(model.ToOutputTargetConfig());
                foreach (var folder in model.Folders.DetectDuplicates())
                {
                    Register(registry, folder);
                }
            }
        }

        private bool AddRootFolderModels()
        {
            var codebaseStructureDesigner = _configurationProvider.GetInstalledModules().FirstOrDefault(m => m.ModuleId == "Intent.Modelers.CodebaseStructure");

            if (codebaseStructureDesigner is null)
            {
                return true;
            }

            if (NuGetVersion.TryParse(codebaseStructureDesigner.Version, out var version) &&
                version >= NuGetVersion.Parse("1.0.1-pre.0"))
            {
                return false;
            }

            return true;
        }

        private static void Register(IOutputTargetRegistry registry, FolderModel folder)
        {
            var outputTargetConfig = folder.ToOutputTargetConfig();

            registry.RegisterOutputTarget(outputTargetConfig);
            foreach (var child in folder.Folders.DetectDuplicates())
            {
                Register(registry, child);
            }
        }


    }

    internal static class OutputTargetRegistrationExtensions
    {
        public static IEnumerable<FolderModel> DetectDuplicates(this IEnumerable<FolderModel> sequence)
        {
            var folderNameSet = new HashSet<string>();

            foreach (var folderModel in sequence)
            {
                if (!folderNameSet.Add(folderModel.Name))
                {
                    throw new ElementException(folderModel.InternalElement, $"Duplicate Folder found at same location.");
                }
                yield return folderModel;
            }
        }
    }
}
