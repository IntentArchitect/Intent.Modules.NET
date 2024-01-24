using System.Collections.Generic;
using Intent.Engine;
using Intent.Exceptions;
using Intent.Modules.VisualStudio.Projects.Api;
using Intent.Modules.VisualStudio.Projects.Templates.JavaScriptProject;
using Intent.Registrations;

namespace Intent.Modules.VisualStudio.Projects.OutputTargets
{
    public class OutputTargetRegistration : IOutputTargetRegistration
    {
        private readonly IMetadataManager _metadataManager;

        public OutputTargetRegistration(IMetadataManager metadataManager)
        {
            _metadataManager = metadataManager;
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

            var solutionFolders = _metadataManager.VisualStudio(application).GetSolutionFolderModels();
            foreach (var solutionFolder in solutionFolders)
            {
                registry.RegisterOutputTarget(solutionFolder.ToOutputTarget());
            }

            var javaScriptProjects = _metadataManager.VisualStudio(application).GetJavaScriptProjectModels();
            foreach (var model in javaScriptProjects)
            {
                registry.RegisterOutputTarget(model.ToOutputTargetConfig());
                foreach (var folder in model.Folders.DetectDuplicates())
                {
                    Register(registry, folder);
                }
            }
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
