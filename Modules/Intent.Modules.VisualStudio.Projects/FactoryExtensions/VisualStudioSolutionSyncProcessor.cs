using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Intent.Engine;
using Intent.Eventing;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using Intent.Modules.VisualStudio.Projects.Api;
using Intent.Modules.VisualStudio.Projects.SolutionFile;
using Intent.Modules.VisualStudio.Projects.Templates.VisualStudio2015Solution;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.VisualStudio.Projects.FactoryExtensions
{
    [IntentManaged(Mode.Merge)]
    public class VisualStudioSolutionSyncProcessor : FactoryExtensionBase, IExecutionLifeCycle, ITemplateLifeCycle
    {
        private delegate void UpdateChanges(string filename, string content);

        private readonly ISoftwareFactoryEventDispatcher _sfEventDispatcher;
        private readonly IChanges _changes;
        private readonly Dictionary<string, List<SoftwareFactoryEvent>> _actions = new();
        private readonly Dictionary<string, VisualStudio2015SolutionTemplate> _vsSolutionsById = new();
        private IApplication _application;

        public VisualStudioSolutionSyncProcessor(
            ISoftwareFactoryEventDispatcher sfEventDispatcher,
            IChanges changes)
        {
            _sfEventDispatcher = sfEventDispatcher;
            _changes = changes;
            _sfEventDispatcher.Subscribe(SoftwareFactoryEvents.FileAddedEvent, Handle);
            _sfEventDispatcher.Subscribe(SoftwareFactoryEvents.FileRemovedEvent, Handle);
        }

        private (string Content, Action<string> ChangeContent) GetChange(string filePath)
        {
            var change = _changes.FindChange(filePath);
            if (change != null)
            {
                return (change.Content, change.ChangeContent);
            }

            var content = File.ReadAllText(filePath);
            void ChangeContent(string newContent)
            {
                _sfEventDispatcher.Publish(new SoftwareFactoryEvent(
                    eventIdentifier: SoftwareFactoryEvents.OverwriteFileCommand,
                    additionalInfo: new Dictionary<string, string>
                    {
                        ["FullFileName"] = filePath,
                        ["Content"] = newContent,
                    }));
            }

            return (content, ChangeContent);
        }

        public override string Id => "Intent.VisualStudio.Projects.VisualStudioSolutionItemExtension";
        public override int Order => 0;

        [IntentManaged(Mode.Ignore)]
        public void OnStep(IApplication application, string step)
        {
            _application ??= application;

            if (step == ExecutionLifeCycleSteps.AfterTemplateExecution)
            {
                Sync();
            }
        }

        private void Sync()
        {
            var byVsSolutionId = _actions
                .Select(element =>
                {
                    var (outputTargetId, events) = element;

                    var outputTarget = _application.OutputTargets.FirstOrDefault(x => x.Id == outputTargetId);
                    if (outputTarget == null)
                    {
                        //This scenario occurs when targets have been deleted
                        return null;
                    }

                    if (outputTarget.Metadata == null ||
                        !outputTarget.Metadata.TryGetValue(FolderConfig.MetadataKey.IsMatch, out var value) ||
                        value is not true)
                    {
                        return null;
                    }

                    var model = (SolutionFolderModel)outputTarget.Metadata[FolderConfig.MetadataKey.Model];

                    return new
                    {
                        VsSolutionId = model.InternalElement.Package.Id,
                        Model = model,
                        Events = events
                    };
                })
                .Where(x => x != null)
                .GroupBy(x => x.VsSolutionId);

            foreach (var solution in byVsSolutionId)
            {
                var filePath = _vsSolutionsById[solution.Key].GetMetadata().GetFilePath();
                var change = GetChange(filePath);
                var slnFile = new SlnFile(filePath, change.Content);

                foreach (var item in solution)
                {
                    foreach (var @event in item.Events)
                    {
                        switch (@event.EventIdentifier)
                        {
                            case SoftwareFactoryEvents.FileAddedEvent:
                                slnFile.AddSolutionItem(item.Model.Name, @event.GetValue("Path"));
                                break;
                            case SoftwareFactoryEvents.FileRemovedEvent:
                                slnFile.RemoveSolutionItem(item.Model.Name, @event.GetValue("Path"));
                                break;
                        }
                    }
                }

                change.ChangeContent(slnFile.ToString());
            }
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

        public void PostConfiguration(ITemplate template) { }

        public void PostCreation(ITemplate template)
        {
            if (template is VisualStudio2015SolutionTemplate vsSolutionTemplate)
            {
                _vsSolutionsById.Add(vsSolutionTemplate.Model.Id, vsSolutionTemplate);
            }
        }
    }
}