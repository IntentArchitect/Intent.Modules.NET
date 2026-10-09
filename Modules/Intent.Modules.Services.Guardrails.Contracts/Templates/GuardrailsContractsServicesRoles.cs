namespace Intent.Modules.Services.Guardrails.Contracts.Templates
{
    /// <summary>
    /// Role strings used for cross-template (and cross-module) type resolution.
    /// The Domain roles are referenced by string so that Services contracts can resolve
    /// Domain contract types without an assembly reference to the Domain module.
    /// </summary>
    public static class GuardrailsContractsServicesRoles
    {
        public const string Service = "Guardrails.Contracts.Services.Service";
        public const string Command = "Guardrails.Contracts.Services.Command";
        public const string Query = "Guardrails.Contracts.Services.Query";
        public const string Dto = "Guardrails.Contracts.Services.Dto";
        public const string Enum = "Guardrails.Contracts.Services.Enum";
        public const string IntegrationEvent = "Guardrails.Contracts.Services.IntegrationEvent";
        public const string IntegrationCommand = "Guardrails.Contracts.Services.IntegrationCommand";
        public const string EventingDto = "Guardrails.Contracts.Services.EventingDto";
        public const string IntegrationEventHandler = "Guardrails.Contracts.Services.IntegrationEventHandler";

        public const string DomainEnum = "Guardrails.Contracts.Domain.Enum";
        public const string DomainEntity = "Guardrails.Contracts.Domain.Entity";
        public const string DomainValueObject = "Guardrails.Contracts.Domain.ValueObject";
        public const string DomainDataContract = "Guardrails.Contracts.Domain.DataContract";

        public static readonly string[] TypeSources =
            [
            Service,
            Command,
            Query,
            Dto,
            Enum,
            IntegrationEvent,
            IntegrationCommand,
            EventingDto,
            DomainEnum,
            DomainEntity,
            DomainValueObject,
            DomainDataContract
            ];
    }
}
