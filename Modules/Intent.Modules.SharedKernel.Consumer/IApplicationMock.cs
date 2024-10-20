using Intent.Configuration;
using Intent.Engine;
using Intent.Eventing;
using Intent.Metadata.Models;
using Intent.Templates;
using System;
using System.Collections.Generic;

namespace Intent.Modules.SharedKernel.Consumer
{

    /// <summary>
    /// Allows my to reuse Original Template Registrations so all the logic stays the same
    /// </summary>
    internal class ApplicationStub : IApplication
    {
        private readonly string _applicationId;
        public ApplicationStub(string applicationId)
        {
            _applicationId = applicationId;
        }

        public string Id => _applicationId;

        public string SolutionName => throw new NotImplementedException();

        public string Name => throw new NotImplementedException();

        public string RootLocation => throw new NotImplementedException();

        public string OutputRootDirectory => throw new NotImplementedException();

        public string Description => throw new NotImplementedException();

        public IIconModel Icon => throw new NotImplementedException();

        public IEnumerable<IProject> Projects => throw new NotImplementedException();

        public IPluginConfigurationReader Config => throw new NotImplementedException();

        public IApplicationEventDispatcher EventDispatcher => throw new NotImplementedException();

        public IEnumerable<IOutputTarget> OutputTargets => throw new NotImplementedException();

        public IChanges ChangeManager => throw new NotImplementedException();

        public IEnumerable<IIntentInstalledModule> InstalledModules => throw new NotImplementedException();

        public IApplicationSettingsProvider Settings => throw new NotImplementedException();

        public IMetadataManager MetadataManager => throw new NotImplementedException();

        public IOutputCache OutputCache => throw new NotImplementedException();

        public IOutputTarget FindOutputTargetWithTemplate(ITemplate template)
        {
            throw new NotImplementedException();
        }

        public IOutputTarget FindOutputTargetWithTemplate(string templateId, Func<ITemplate, bool> predicate = null)
        {
            throw new NotImplementedException();
        }

        public IOutputTarget FindOutputTargetWithTemplate(string templateId, string metadataModelId)
        {
            throw new NotImplementedException();
        }

        public IOutputTarget FindOutputTargetWithTemplate(string templateId, object model)
        {
            throw new NotImplementedException();
        }

        public ITemplate FindTemplateInstance(string templateIdOrRole, Func<ITemplate, bool> predicate = null)
        {
            throw new NotImplementedException();
        }

        public ITemplate FindTemplateInstance(string templateIdOrRole, string metadataModelId)
        {
            throw new NotImplementedException();
        }

        public ITemplate FindTemplateInstance(string templateIdOrRole, object model)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ITemplate> FindTemplateInstances(string templateIdOrRole, Func<ITemplate, bool> predicate = null)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ITemplate> FindTemplateInstances(string templateIdOrRole, string metadataModelId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ITemplate> FindTemplateInstances(string templateIdOrRole, object model)
        {
            throw new NotImplementedException();
        }

        public IApplicationConfig GetApplicationConfig()
        {
            throw new NotImplementedException();
        }

        public IApplicationConfig GetApplicationConfig(string applicationId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ITemplate> GetApplicationTemplates()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IIntentInstalledModule> GetInstalledModules()
        {
            throw new NotImplementedException();
        }

        public IIntentModule GetModule(object @object)
        {
            throw new NotImplementedException();
        }

        public IExecutionOutputLog GetPreviousExecutionLog()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<string> GetRolesForTemplate(ITemplate template)
        {
            throw new NotImplementedException();
        }

        public IApplicationSettingsProvider GetSettings()
        {
            throw new NotImplementedException();
        }

        public ISolutionConfig GetSolutionConfig()
        {
            throw new NotImplementedException();
        }

        public string GetSolutionPath()
        {
            throw new NotImplementedException();
        }

        public void RegisterInstance<TInstance>(object key, TInstance instance)
        {
            throw new NotImplementedException();
        }

        public void RegisterTemplateInRole(string role, ITemplate template)
        {
            throw new NotImplementedException();
        }

        public void RegisterTemplateInRoleForModel(string role, object model, ITemplate template)
        {
            throw new NotImplementedException();
        }

        public bool TemplateExists(string templateIdOrRole, Func<ITemplate, bool> predicate = null)
        {
            throw new NotImplementedException();
        }

        public bool TemplateExists(string templateIdOrRole, string metadataModelId)
        {
            throw new NotImplementedException();
        }

        public bool TemplateExists(string templateIdOrRole, object model)
        {
            throw new NotImplementedException();
        }

        public bool TryResolveInstance<TInstance>(object key, out TInstance instance)
        {
            throw new NotImplementedException();
        }
    }

}
