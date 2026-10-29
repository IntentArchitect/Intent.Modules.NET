using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Modules.Common.Drift;
using Intent.Modules.Domain.Guardrails.Contracts.Drift;
using Intent.Modules.Services.Guardrails.Contracts.Drift;

namespace Intent.Modules.Guardrails.Contracts.Tests
{
    /// <summary>
    /// Both Guardrails designers' kinds of fact (<see cref="DomainDrift"/> and <see cref="ServicesDrift"/>), as the tests
    /// name them. The drift module knows neither designer, so a test that runs its engine passes the designer's kinds
    /// from here.
    /// </summary>
    internal static class FactKinds
    {
        public const string Domain = DomainDrift.Name;
        public const string Services = ServicesDrift.Name;

        public static readonly FactKind TableName = DomainDrift.TableName;
        public static readonly FactKind TableSchema = DomainDrift.TableSchema;
        public static readonly FactKind TableKind = DomainDrift.TableKind;
        public static readonly FactKind Key = DomainDrift.Key;
        public static readonly FactKind ColumnName = DomainDrift.ColumnName;
        public static readonly FactKind ColumnType = DomainDrift.ColumnType;
        public static readonly FactKind ColumnTextType = DomainDrift.ColumnTextType;
        public static readonly FactKind ColumnMaxLength = DomainDrift.ColumnMaxLength;
        public static readonly FactKind ColumnRequired = DomainDrift.ColumnRequired;
        public static readonly FactKind ColumnPrecision = DomainDrift.ColumnPrecision;
        public static readonly FactKind ColumnScale = DomainDrift.ColumnScale;
        public static readonly FactKind ColumnDefault = DomainDrift.ColumnDefault;
        public static readonly FactKind RelationshipMultiplicity = DomainDrift.RelationshipMultiplicity;
        public static readonly FactKind RelationshipForeignKey = DomainDrift.RelationshipForeignKey;
        public static readonly FactKind IndexColumns = DomainDrift.IndexColumns;
        public static readonly FactKind IndexUnique = DomainDrift.IndexUnique;
        public static readonly FactKind IntentRaises = DomainDrift.IntentRaises;

        public static readonly FactKind ServiceRoute = ServicesDrift.ServiceRoute;
        public static readonly FactKind OperationVerb = ServicesDrift.OperationVerb;
        public static readonly FactKind OperationRoute = ServicesDrift.OperationRoute;
        public static readonly FactKind Security = ServicesDrift.Security;
        public static readonly FactKind SecurityRoles = ServicesDrift.SecurityRoles;
        public static readonly FactKind SecurityPolicy = ServicesDrift.SecurityPolicy;
        public static readonly FactKind ParameterSource = ServicesDrift.ParameterSource;
        public static readonly FactKind IntentCreates = ServicesDrift.IntentCreates;
        public static readonly FactKind IntentUpdates = ServicesDrift.IntentUpdates;
        public static readonly FactKind IntentDeletes = ServicesDrift.IntentDeletes;
        public static readonly FactKind IntentQueries = ServicesDrift.IntentQueries;
        public static readonly FactKind IntentInvokes = ServicesDrift.IntentInvokes;
        public static readonly FactKind IntentPublishes = ServicesDrift.IntentPublishes;
        public static readonly FactKind IntentSends = ServicesDrift.IntentSends;

        public static readonly IReadOnlyList<FactKind> All = DomainDrift.Kinds.Concat(ServicesDrift.Kinds).ToList();

        public static IReadOnlyList<FactKind> For(string designer) => designer switch
        {
            Domain => DomainDrift.Kinds,
            Services => ServicesDrift.Kinds,
            _ => throw new ArgumentOutOfRangeException(nameof(designer), designer, "Not a Guardrails designer.")
        };
    }
}
