using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Intent.Eventing;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modules.Common.Drift;
using Intent.Modules.Common.Drift.Templates;
using Intent.Modules.Common.DriftReporting.Api;
using Intent.Modules.Common.DriftReporting.FactoryExtensions;
using Intent.Plugins.FactoryExtensions;
using NSubstitute;

namespace Intent.Modules.Guardrails.Contracts.Tests
{
    /// <summary>
    /// What a designer's drift check adds to the drift report, one line per finding as
    /// <c>Severity Category | Subject | Against | Message</c>. A model element is
    /// <c>element:&lt;id&gt;</c>, a file its path relative to the code's root (with its line), and text <c>text:…</c>.
    /// </summary>
    internal static class DriftFindings
    {
        /// <summary>The code's root the in-memory checks' relative paths are under, for <see cref="HubMapping"/>.</summary>
        public static readonly string CodeRoot = Path.Combine(Path.GetTempPath(), "drift-code");

        /// <summary>A check's report as findings and notes, through the same mapping a drift reporter uses.</summary>
        public static IReadOnlyList<string> Of(DriftReport report, IReadOnlyDictionary<string, IElement> elements = null)
        {
            var area = new RecordingDriftArea(report.Designer);
            HubMapping.Map(report, area, elements ?? new Dictionary<string, IElement>(), CodeRoot, Path.Combine(CodeRoot, report.RecipePath));
            return area.Snapshot(CodeRoot);
        }

        /// <summary>Elements with the given ids, as a designer's <see cref="DriftDesigner.ReadElements"/> returns them.</summary>
        public static IReadOnlyDictionary<string, IElement> Elements(params string[] ids) =>
            ids.ToDictionary(x => x, x =>
            {
                var element = Substitute.For<IElement>();
                element.Id.Returns(x);
                element.Name.Returns(x);
                return element;
            });

        public static string Line(DriftFinding finding, string root)
        {
            var category = finding.Category switch
            {
                DriftCategory.ModelCode => "Model/Code",
                DriftCategory.CodeCode => "Code/Code",
                _ => "Code"
            };
            var against = finding.Against.Count == 0 ? "—" : string.Join(", ", finding.Against.Select(x => Anchor(x, root)));
            return $"{finding.Severity} {category} | {Anchor(finding.Subject, root)} | {against} | {finding.Message}";
        }

        private static string Anchor(DriftAnchor anchor, string root)
        {
            switch (anchor.Kind)
            {
                case DriftAnchorKind.Element:
                    return $"element:{anchor.ModelElement.Id}";
                case DriftAnchorKind.File:
                    var path = anchor.FilePath.Replace('\\', '/');
                    var prefix = root.Replace('\\', '/').TrimEnd('/') + "/";
                    path = path.StartsWith(prefix, StringComparison.OrdinalIgnoreCase) ? path[prefix.Length..] : path;
                    return anchor.Line is { } line ? $"{path}:{line}" : path;
                default:
                    return $"text:{anchor.Value}";
            }
        }
    }

    /// <summary>A drift context that records what reporters add, Area by Area.</summary>
    internal sealed class RecordingDriftContext : IDriftContext
    {
        public RecordingDriftContext(IApplication application)
        {
            Application = application;
        }

        public IApplication Application { get; }
        public List<RecordingDriftArea> Areas { get; } = new();

        public IDriftArea Area(string name, params DriftCategory[] checks)
        {
            var area = Areas.FirstOrDefault(x => x.Name == name);
            if (area == null)
            {
                Areas.Add(area = new RecordingDriftArea(name));
            }

            area.Checks.UnionWith(checks);
            return area;
        }
    }

    internal sealed class RecordingDriftArea : IDriftArea
    {
        public RecordingDriftArea(string name)
        {
            Name = name;
        }

        public string Name { get; }
        public HashSet<DriftCategory> Checks { get; } = new();
        public List<DriftFinding> Findings { get; } = new();

        public void Log(DriftFinding finding) => Findings.Add(finding);

        public IReadOnlyList<string> Snapshot(string root) =>
            Findings.Select(x => DriftFindings.Line(x, root)).ToList();
    }

    /// <summary>The run's drift checks (Intent.Common.DriftReporting) with reporters registered, as the Software Factory's steps do.</summary>
    internal static class DriftHub
    {
        /// <summary>
        /// Runs the start step of the drift checks and each reporter, then each reporter's registration step, against the
        /// application's event dispatcher, so the drift checks run them and <see cref="ModelSyncGuidance.Designers"/> names
        /// their designers.
        /// </summary>
        public static DriftChecks Register(IApplication application, params DriftReporterBase[] reporters)
        {
            var checks = new DriftChecks();
            foreach (var extension in reporters.Prepend<IExecutionLifeCycle>(checks))
            {
                extension.OnStep(application, ExecutionLifeCycleSteps.Start);
            }

            foreach (var reporter in reporters)
            {
                ((IExecutionLifeCycle)reporter).OnStep(application, ExecutionLifeCycleSteps.BeforeTemplateRegistrations);
            }

            return checks;
        }

        /// <summary>The Software Factory's event dispatcher, as it behaves: synchronous, and delivered only to existing subscribers.</summary>
        public static IApplicationEventDispatcher Dispatcher() => new EventDispatcher();

        private sealed class EventDispatcher : IApplicationEventDispatcher
        {
            private readonly Dictionary<string, List<Action<object>>> _subscriptions = new();

            public void Subscribe<TEvent>(string eventIdentifier, Action<TEvent> handler)
            {
                if (!_subscriptions.TryGetValue(eventIdentifier, out var handlers))
                {
                    _subscriptions[eventIdentifier] = handlers = new List<Action<object>>();
                }

                handlers.Add(o => handler((TEvent)o));
            }

            public void Subscribe(string eventIdentifier, Action<ApplicationEvent> handler) => Subscribe<ApplicationEvent>(eventIdentifier, handler);
            public void Subscribe<TEvent>(Action<TEvent> handler) => Subscribe(typeof(TEvent).FullName, handler);

            public void Publish<TEvent>(string eventIdentifier, TEvent payload)
            {
                foreach (var handler in _subscriptions.GetValueOrDefault(eventIdentifier)?.ToList() ?? new List<Action<object>>())
                {
                    handler(payload);
                }
            }

            public void Publish<TEvent>(TEvent payload) => Publish(typeof(TEvent).FullName, payload);
            public void Publish(ApplicationEvent @event) => Publish(@event.EventIdentifier, @event);
            public void Publish(string eventIdentifier, IDictionary<string, string> @event) => Publish(new ApplicationEvent(eventIdentifier, @event));
            public void Publish(string eventIdentifier, Dictionary<string, string> @event) => Publish(new ApplicationEvent(eventIdentifier, @event));
        }
    }
}
