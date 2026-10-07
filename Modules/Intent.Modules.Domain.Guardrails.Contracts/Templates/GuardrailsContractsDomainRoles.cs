using System.Collections.Generic;

namespace Intent.Modules.Domain.Guardrails.Contracts.Templates
{
    /// <summary>
    /// The Roles configured on this module's templates in the Module Builder designer. Cross-template
    /// type resolution goes through these strings rather than through TemplateId references.
    /// </summary>
    public static class GuardrailsContractsDomainRoles
    {
        public const string Entity = "Guardrails.Contracts.Domain.Entity";
        public const string Enum = "Guardrails.Contracts.Domain.Enum";
        public const string ValueObject = "Guardrails.Contracts.Domain.ValueObject";
        public const string Repository = "Guardrails.Contracts.Domain.Repository";
        public const string DataContract = "Guardrails.Contracts.Domain.DataContract";
        public const string DomainService = "Guardrails.Contracts.Domain.DomainService";

        public static IReadOnlyList<string> All { get; } = new[]
        {
            Entity,
            Enum,
            ValueObject,
            Repository,
            DataContract,
            DomainService
        };
    }
}
