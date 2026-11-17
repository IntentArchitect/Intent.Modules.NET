using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Modules.Common.Drift;
using Shouldly;
using Xunit;

namespace Intent.Modules.Guardrails.Contracts.Tests
{
    /// <summary>
    /// Each Guardrails kind of fact names its own element's placeholders and describes its placeholders itself: the drift
    /// module has no catalog, and no rule of its own for which placeholders name a fact's element.
    /// </summary>
    public class FactKindTests
    {
        /// <summary>The intent kinds' own placeholders, which they named explicitly before the move (1.0.0-pre.3).</summary>
        private static readonly IReadOnlyDictionary<string, string[]> ExplicitOwn = new Dictionary<string, string[]>
        {
            ["intent.raises"] = new[] { "DomainEvent" },
            ["intent.creates"] = new[] { "Entity", "Member" },
            ["intent.updates"] = new[] { "Entity", "Member" },
            ["intent.deletes"] = new[] { "Entity" },
            ["intent.queries"] = new[] { "Entity" },
            ["intent.invokes"] = new[] { "Invoked", "InvokedOwner" },
            ["intent.publishes"] = new[] { "Message" },
            ["intent.sends"] = new[] { "Message" }
        };

        [Fact]
        public void Each_kind_owns_the_placeholders_the_drift_check_chose_for_it_before_the_move()
        {
            FactKinds.All.Count.ShouldBe(31);
            foreach (var kind in FactKinds.All)
            {
                kind.Own.ShouldBe(OwnBeforeTheMove(kind), kind.Key);
            }
        }

        /// <summary>
        /// The UI kinds as consumers' recipe files name them: key, label, placeholders in order, and the ones naming the fact's
        /// own element. They must survive the User Interface designer's overhaul, so only a deliberate change moves this.
        /// </summary>
        [Fact]
        public void The_UI_kinds_are_the_contract_the_recipe_files_were_written_against()
        {
            FactKinds.For(FactKinds.Ui).Select(x => $"{x.Key} | {x.Label} | {string.Join(",", x.Placeholders)} | {string.Join(",", x.Own)}").ShouldBe(new[]
            {
                "component.kind | kind | Folder,Component | Component",
                "page.route | route | Folder,Component | Component",
                "security | security | Folder,Component | Component",
                "security.roles | roles | Folder,Component | Component",
                "security.policy | policy | Folder,Component | Component",
                "intent.invokes | invokes | Folder,Component,Invoked,InvokedOwner | Invoked,InvokedOwner",
                "intent.navigates | navigates to | Folder,Component,Page,Route,RouteStart | Page,Route,RouteStart",
                "intent.opens | opens | Folder,Component,Dialog | Dialog",
                "intent.uses | uses | Folder,Component,Child | Child"
            });
        }

        [Fact]
        public void Each_designer_describes_each_placeholder_one_way()
        {
            foreach (var designer in new[] { FactKinds.Domain, FactKinds.Services, FactKinds.Ui })
            {
                foreach (var placeholder in FactKinds.For(designer).SelectMany(x => x.PlaceholderDescriptions).GroupBy(x => x.Name))
                {
                    placeholder.Select(x => x.Description).Distinct().Count().ShouldBe(1, $"{designer} {{{placeholder.Key}}}");
                }
            }
        }

        [Fact]
        public void A_kind_owns_some_of_its_own_placeholders()
        {
            var placeholders = new[] { new FactPlaceholder("Component", "the component's name") };

            Should.Throw<ArgumentException>(() => new FactKind("ui.label", "Ui", "label", placeholders, "v", "s", new[] { "Page" }));
            Should.Throw<ArgumentException>(() => new FactKind("ui.label", "Ui", "label", placeholders, "v", "s", Array.Empty<string>()));
        }

        /// <summary>
        /// The rule the drift check applied before each kind named its own element: an explicit list, else the first of
        /// parameter, operation, attribute and index the kind has, else a relationship's target and ends, else all of them.
        /// </summary>
        private static IReadOnlyList<string> OwnBeforeTheMove(FactKind kind)
        {
            if (ExplicitOwn.TryGetValue(kind.Key, out var own))
            {
                return own;
            }

            foreach (var name in new[] { "Parameter", "Operation", "Attribute", "Index" })
            {
                if (kind.Placeholders.Contains(name))
                {
                    return new[] { name };
                }
            }

            return kind.Placeholders.Contains("SourceEnd")
                ? new[] { "Target", "SourceEnd", "TargetEnd" }
                : kind.Placeholders;
        }
    }
}
