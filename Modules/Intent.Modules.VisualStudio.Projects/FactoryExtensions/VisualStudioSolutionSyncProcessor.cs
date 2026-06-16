using System.Collections.Generic;
using System.IO;
using System.Linq;
using Intent.Engine;
using Intent.Eventing;
using Intent.Modules.Common;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using Intent.Modules.VisualStudio.Projects.Api;
using Intent.Modules.VisualStudio.Projects.Templates.VisualStudioSolution;
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
        private readonly ISoftwareFactoryEventDispatcher _sfEventDispatcher;
        private readonly IChanges _changes;
        private readonly Dictionary<string, List<SoftwareFactoryEvent>> _actions = new();
        private readonly Dictionary<string, IVsSolutionSyncTarget> _vsSolutionsById = new();
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

        private (string Content, System.Action<string> ChangeContent) GetChange(string filePath)
        {
            var change = _changes.FindChange(filePath);
            if (change != null)
            {
                return (change.Content, content => change.ChangeContent(content, content));
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
                        VsSolutionId = model.Solution.Id,
                        Model = model,
                        Events = events
                    };
                })
                .Where(x => x != null)
                .GroupBy(x => x.VsSolutionId);

            foreach (var solution in byVsSolutionId)
            {
                var target = _vsSolutionsById[solution.Key];
                var filePath = target.GetMetadata().GetFilePath();
                var change = GetChange(filePath);

                var actions = solution
                    .SelectMany(item => item.Events.Select(e =>
                    {
                        e.AdditionalInfo.TryGetValue("RelativeOutputPathPrefix", out var prefix);
                        return new SolutionItemAction
                        {
                            EventIdentifier = e.EventIdentifier,
                            PhysicalPath = e.GetValue("Path"),
                            FolderPath = GetPath(item.Model),
                            RelativeOutputPathPrefix = prefix,
                            HasMaterializedFolder = HasMaterializedFolder(item.Model)
                        };
                    }))
                    .ToList();

                var updated = target.ApplySolutionItems(change.Content, actions);
                if (updated != null)
                    change.ChangeContent(updated);
            }
        }

        private bool HasMaterializedFolder(SolutionFolderModel solutionFolderModel)
        {
            var path = GetPath(solutionFolderModel);
            return path.Any(x => x.GetFolderOptions()?.MaterializeFolder() == true);
        }

        private static IReadOnlyCollection<SolutionFolderModel> GetPath(SolutionFolderModel solutionFolderModel)
        {
            var stack = new Stack<SolutionFolderModel>();

            while (solutionFolderModel != null)
            {
                stack.Push(solutionFolderModel);
                solutionFolderModel = solutionFolderModel.ParentFolder;
            }

            return stack;
        }

        public void Handle(SoftwareFactoryEvent @event)
        {
            var outputTargetId = @event.GetValue("OutputTargetId");
            if (!_actions.ContainsKey(outputTargetId))
            {
                _actions[outputTargetId] = [];
            }
            _actions[outputTargetId].Add(@event);
        }

        public void PostConfiguration(ITemplate template) { }

        public void PostCreation(ITemplate template)
        {
            IVsSolutionSyncTarget target = template switch
            {
                VisualStudioSolutionTemplate sln => new SlnSyncTarget(sln.Model.Id, sln.GetMetadata),
                VisualStudioSolutionSlnxTemplate slnx => new SlnxSyncTarget(slnx.Model.Id, slnx.GetMetadata),
                _ => null
            };

            if (target != null)
                _vsSolutionsById.Add(target.SolutionModelId, target);
        }
    }
}
