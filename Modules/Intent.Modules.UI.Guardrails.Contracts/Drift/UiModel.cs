using System;
using System.Collections.Generic;

namespace Intent.Modules.UI.Guardrails.Contracts.Drift
{
    /// <summary>
    /// The User Interface model as the drift check sees it, in no designer version's terms. A reader for one version of the
    /// designer (<see cref="UserInterfaceDesigner1"/>) fills it, and <see cref="UiDrift"/> states its facts. The designer is
    /// being overhauled: a reader for the next version fills the same shape, so the facts, their keys and consumers'
    /// recipe files stay as they are.
    /// </summary>
    internal sealed class UiModel
    {
        public IReadOnlyList<UiComponent> Components { get; init; } = Array.Empty<UiComponent>();
    }

    /// <summary>What a <see cref="UiComponent"/> is, as the <c>component.kind</c> fact states it.</summary>
    internal static class UiKind
    {
        public const string Page = "Page";
        public const string Dialog = "Dialog";
        public const string Component = "Component";
        public const string Layout = "Layout";
    }

    /// <summary>A page, dialog, component or layout, and what it calls, navigates to, opens and renders.</summary>
    internal sealed class UiComponent
    {
        public string Id { get; init; }
        public string Name { get; init; }

        /// <summary>The name of the folder it is in, empty at the package root.</summary>
        public string Folder { get; init; } = string.Empty;

        /// <summary>One of <see cref="UiKind"/>'s values.</summary>
        public string Kind { get; init; }

        /// <summary>A page's route, without a leading <c>/</c>; <see langword="null"/> for anything that isn't a page.</summary>
        public string Route { get; init; }

        public bool Secured { get; init; }

        /// <summary>The roles that secure it, in model order.</summary>
        public IReadOnlyList<string> Roles { get; init; } = Array.Empty<string>();

        /// <summary>The policies that secure it, in model order.</summary>
        public IReadOnlyList<string> Policies { get; init; } = Array.Empty<string>();

        /// <summary>The Commands, Queries and Service operations it calls, wherever in it the call is modelled.</summary>
        public IReadOnlyList<UiCall> Calls { get; init; } = Array.Empty<UiCall>();

        /// <summary>The pages it navigates to.</summary>
        public IReadOnlyList<UiLink> Navigations { get; init; } = Array.Empty<UiLink>();

        /// <summary>The dialogs it opens.</summary>
        public IReadOnlyList<UiLink> Dialogs { get; init; } = Array.Empty<UiLink>();

        /// <summary>The components it renders.</summary>
        public IReadOnlyList<UiLink> Children { get; init; } = Array.Empty<UiLink>();
    }

    /// <summary>A backend call: the Command's, Query's or operation's name, and the Service an operation belongs to (empty otherwise).</summary>
    internal sealed record UiCall(string Name, string Owner);

    /// <summary>Another page, dialog or component: its name and, for a page, its route without a leading <c>/</c>.</summary>
    internal sealed record UiLink(string Name, string Route = null);
}
