using System;
using System.Collections.Generic;
using System.Linq;

namespace Intent.Modules.Common.Drift.EngineMigration
{
    /// <summary>
    /// What the v1 drift check left in an application, and the Intent.Common.DriftEngine rows that replace it. Common.Drift
    /// can't reference the Guardrails modules or DriftEngine, so their ids are copied here; <c>V1FootprintTests</c> checks
    /// each copy against this branch and against <c>origin/master</c>, the published Guardrails source.
    /// </summary>
    internal static class V1Footprint
    {
        public const string DomainModuleId = "Intent.Domain.Guardrails.Contracts";
        public const string ServicesModuleId = "Intent.Services.Guardrails.Contracts";
        public const string UiModuleId = "Intent.UI.Guardrails.Contracts";

        /// <summary>The Guardrails Contracts modules. Each defines its stereotypes in a package named for its id.</summary>
        public static readonly IReadOnlyList<string> ModuleIds = new[] { DomainModuleId, ServicesModuleId, UiModuleId };

        /// <summary>The templates that merged contract code into hand-written files: 7 Domain, 11 Services.</summary>
        public static readonly IReadOnlyCollection<string> ContractTemplateIds = new HashSet<string>(StringComparer.Ordinal)
        {
            "Intent.Domain.Guardrails.Contracts.DomainContract",
            "Intent.Domain.Guardrails.Contracts.DomainEventContract",
            "Intent.Domain.Guardrails.Contracts.DomainServiceContract",
            "Intent.Domain.Guardrails.Contracts.EntityContract",
            "Intent.Domain.Guardrails.Contracts.EnumContract",
            "Intent.Domain.Guardrails.Contracts.RepositoryContract",
            "Intent.Domain.Guardrails.Contracts.ValueObjectContract",
            "Intent.Services.Guardrails.Contracts.CommandContract",
            "Intent.Services.Guardrails.Contracts.DomainEventHandlerContract",
            "Intent.Services.Guardrails.Contracts.DtoContract",
            "Intent.Services.Guardrails.Contracts.EnumContract",
            "Intent.Services.Guardrails.Contracts.EventingDtoContract",
            "Intent.Services.Guardrails.Contracts.IntegrationCommandContract",
            "Intent.Services.Guardrails.Contracts.IntegrationEventContract",
            "Intent.Services.Guardrails.Contracts.IntegrationEventHandlerContract",
            "Intent.Services.Guardrails.Contracts.QueryContract",
            "Intent.Services.Guardrails.Contracts.ServiceContract",
            "Intent.Services.Guardrails.Contracts.ServiceProxyContract",
        };

        /// <summary>
        /// Integration Event Subscription and Integration Command Subscription, association types the Services module defines
        /// on the Services designer's Eventing packages. Without the module, a package holding one fails to load.
        /// </summary>
        public static readonly IReadOnlyList<string> SubscriptionAssociationTypeIds = new[]
        {
            "75e953ed-9a87-47d5-9d45-cdca4232be54",
            "9073c828-0127-448e-be24-f2ebfc4d3238",
        };

        /// <summary>The Services module's <c>Guardrails Eventing Extension</c> designer settings.</summary>
        public const string EventingExtensionDesignerSettingsId = "2c15e6e8-7fb3-4b80-b038-46772482baf5";

        public const string CodebaseStructureDesignerId = "0701433c-36c0-4569-b1f4-9204986b587d";

        /// <summary>Intent.Metadata.RDBMS's <c>Table</c> stereotype on a Class, and its <c>Name</c> property.</summary>
        public static class TableStereotype
        {
            public const string DefinitionId = "dd205b32-b48b-4c77-98f5-faefb2c047ce";
            public const string Name = "2b3a9df7-65e1-4800-b919-bef1a6b8f5a9";
        }

        /// <summary>The folder beside the <c>.application.config</c> holding <c>&lt;designer&gt;.drift.json</c>.</summary>
        public const string RecipeFolder = "guardrails";

        public const string RecipeFilePattern = "*.drift.json";

        /// <summary>The designers whose contract templates reported unmodelled public members, by recipe file name.</summary>
        public static readonly IReadOnlyCollection<string> StructureCheckedRecipes = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "domain.drift.json",
            "services.drift.json",
        };

        /// <summary>Whether a Software Factory output log entry is a contract template's: its template id is the part before <c>#</c>.</summary>
        public static bool IsContractCorrelation(string correlationId)
        {
            if (string.IsNullOrWhiteSpace(correlationId))
            {
                return false;
            }

            var hash = correlationId.IndexOf('#');
            return ContractTemplateIds.Contains(hash < 0 ? correlationId : correlationId[..hash]);
        }

        /// <summary>
        /// A Template Output of a Guardrails template: one the module installed (its external reference is the module), or
        /// one placed by hand, named for the template id.
        /// </summary>
        public static bool IsGuardrailsTemplateOutput(string externalReference, string name)
        {
            return ModuleIds.Contains(externalReference) ||
                   name != null && ModuleIds.Any(x => name.StartsWith(x + ".", StringComparison.Ordinal));
        }

        /// <summary>Intent.Common.DriftEngine's metadata package and the stereotypes the migration writes, from its <c>Api</c> and metadata.</summary>
        public static class DriftEngine
        {
            public const string ModuleId = "Intent.Common.DriftEngine";
            public const string PackageId = "b4107910-4d30-42b3-a453-ccbfda74f7cc";
            public const string PackageName = "Intent.Common.DriftEngine";

            public static class DriftScope
            {
                public const string Name = "Drift Scope";
                public const string DefinitionId = "857eba81-5138-429c-a97c-2f4b613e8d8c";
                public const string Entries = "ea62db2f-b013-4ace-ac18-4233ca8bb4cb";
            }

            public static class DriftScopeEntry
            {
                public const string Name = "Drift Scope Entry";
                public const string DefinitionId = "fe20f2d9-4d95-44c8-96cb-9b129d9e56a5";
                public const string Glob = "1b5d6529-dc3f-46e1-8c84-ba216f986bcf";
                public const string Types = "3c11fc6d-e0f1-4749-88c1-e3145d347c40";
                public const string Scope = "34ebee0c-b472-4179-a5d1-f95bf154db78";
                public const string Reason = "cc06fdd0-1072-4601-8cc2-323e9b8fbe79";
            }

            public static class DriftCheck
            {
                public const string Name = "Drift Check";
                public const string DefinitionId = "baf16c68-1491-4c76-97ad-be1f3f4661ce";
                public const string OptInRules = "9917fe75-64c5-40ee-aa31-839289e3d343";
                public const string Exceptions = "dee85862-1e0a-422c-8bae-a2f8527bdb4b";
            }

            public static class OptInRule
            {
                public const string Name = "Drift Check Opt-in Rule";
                public const string DefinitionId = "8648e3bd-177b-4fc4-908d-96cc4ccd2009";
                public const string Rule = "84edffe8-0350-41e1-ac94-e030a296be1f";
                public const string Glob = "e715431b-d4c1-4400-96a6-84277257ca56";
                public const string Description = "5800fa36-431a-4d45-9beb-016d07b5ab9f";
            }

            public static class Exception
            {
                public const string Name = "Drift Check Exception";
                public const string DefinitionId = "890d0cac-4620-452c-a5de-8d569c367e8e";
                public const string Rule = "24c16665-0876-4132-bc00-b04a579e2c20";
                public const string Match = "6340b854-4902-45bf-bfd5-f544e431d603";
                public const string Reason = "b0388494-399d-4f06-9280-075bc6e3afee";
            }
        }
    }
}
