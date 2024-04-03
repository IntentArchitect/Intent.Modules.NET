using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Intent.Engine;
using Intent.Eventing;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Constants;
using Intent.Modules.VisualStudio.Projects.Events;
using Intent.Modules.VisualStudio.Projects.Templates;
using Intent.Plugins.FactoryExtensions;
using Intent.Utils;

namespace Intent.Modules.VisualStudio.Projects.Sync
{
    [Description("Visual Studio Project File Syncer")]
    public class ApplicationSyncProcessor : FactoryExtensionBase, IExecutionLifeCycle
    {
        private readonly ISoftwareFactoryEventDispatcher _sfEventDispatcher;
        private readonly IChanges _changeManager;
        private readonly Dictionary<string, List<SoftwareFactoryEvent>> _actions;
        private readonly Dictionary<string, IVisualStudioProjectTemplate> _projectTemplatesById = new();

        public override string Id => "Intent.ApplicationSyncProcessor";

        public override int Order { get; set; } = 90;

        public ApplicationSyncProcessor(ISoftwareFactoryEventDispatcher sfEventDispatcher, IChanges changeManager)
        {
            _changeManager = changeManager;
            _actions = new Dictionary<string, List<SoftwareFactoryEvent>>();
            _sfEventDispatcher = sfEventDispatcher;
            //Subscribe to all the project change events
            _sfEventDispatcher.Subscribe(SoftwareFactoryEvents.FileAddedEvent, Handle);
            _sfEventDispatcher.Subscribe(SoftwareFactoryEvents.FileRemovedEvent, Handle);
            _sfEventDispatcher.Subscribe(SoftwareFactoryEvents.AddTargetEvent, Handle);
            _sfEventDispatcher.Subscribe(SoftwareFactoryEvents.AddTaskEvent, Handle);
            _sfEventDispatcher.Subscribe(SoftwareFactoryEvents.ChangeProjectItemTypeEvent, Handle);

            _sfEventDispatcher.Subscribe(CsProjectEvents.AddImport, Handle);
            _sfEventDispatcher.Subscribe(CsProjectEvents.AddCompileDependsOn, Handle);
            _sfEventDispatcher.Subscribe(CsProjectEvents.AddBeforeBuild, Handle);
            _sfEventDispatcher.Subscribe(CsProjectEvents.AddContentFile, Handle);

        }

        protected override void OnBeforeTemplateRegistrations(IApplication application)
        {
            application.EventDispatcher.Subscribe<VisualStudioProjectCreatedEvent>(Handle);
            base.OnBeforeTemplateRegistrations(application);
        }

        protected override void OnAfterTemplateExecution(IApplication application)
        {
            SyncProjectFiles(application);
        }

        public void SyncProjectFiles(IApplication application)
        {
            foreach (var (outputTargetId, events) in _actions)
            {
                var outputTarget = application.OutputTargets.FirstOrDefault(x => x.Id == outputTargetId);
                if (outputTarget == null)
                {
                    //This scenario occurs when targets have been deleted
                    continue;
                }

                var projectId = outputTarget.GetTargetPath()[0].Id;
                if (!_projectTemplatesById.TryGetValue(projectId, out var templateInstance))
                {
                    // This scenario occurs when a "Template Output" is inside a solution folder
                    // for example for solution items.
                    continue;
                }

                switch (templateInstance.Project.ProjectTypeId)
                {
                    case VisualStudioProjectTypeIds.CSharpLibrary:
                    case VisualStudioProjectTypeIds.ConsoleAppNetFramework:
                    case VisualStudioProjectTypeIds.NodeJsConsoleApplication:
                    case VisualStudioProjectTypeIds.WcfApplication:
                    case VisualStudioProjectTypeIds.WebApiApplication:
                        new FrameworkProjectSyncProcessor(templateInstance, _sfEventDispatcher, _changeManager).Process(events);
                        break;
                    case VisualStudioProjectTypeIds.SdkCSharpProject:
					case VisualStudioProjectTypeIds.CoreCSharpLibrary:
                    case VisualStudioProjectTypeIds.CoreWebApp:
                    case VisualStudioProjectTypeIds.CoreConsoleApp:
                    case VisualStudioProjectTypeIds.AzureFunctionsProject:
                        new CoreProjectSyncProcessor(templateInstance, _sfEventDispatcher, _changeManager).Process(events);
                        break;
                    case VisualStudioProjectTypeIds.SQLServerDatabaseProject:
                        new SqlProjectSyncProcessor(templateInstance, _sfEventDispatcher, _changeManager).Process(events);
                        break;
                    default:
                        Logging.Log.Warning("No project synchronizer could be found for project: " + templateInstance.Name);
                        continue;
                }
            }

            _actions.Clear();
        }

        private void Handle(VisualStudioProjectCreatedEvent @event)
        {
            var outputTargetId = @event.TemplateInstance.OutputTarget.Id;

            if (_projectTemplatesById.ContainsKey(outputTargetId))
            {
                throw new Exception($"Attempted to add project with same project Id [{outputTargetId}] (name: {@event.TemplateInstance.Name})");
            }

            _projectTemplatesById.Add(outputTargetId, @event.TemplateInstance);
        }

        public void Handle(SoftwareFactoryEvent @event)
        {
            var outputTargetId = @event.GetValue("OutputTargetId");
            if (!_actions.ContainsKey(outputTargetId))
            {
                _actions[outputTargetId] = new List<SoftwareFactoryEvent>();
            }
            _actions[outputTargetId].Add(@event);
        }
    }
}
