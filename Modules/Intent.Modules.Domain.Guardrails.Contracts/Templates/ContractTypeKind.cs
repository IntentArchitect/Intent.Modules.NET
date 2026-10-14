using System;
using Intent.Domain.Guardrails.Contracts.Api;
using Intent.Exceptions;
using Intent.Metadata.Models;
using Intent.Modelers.Domain.Api;
using Intent.Modelers.Domain.Repositories.Api;
using Intent.Modelers.Domain.Services.Api;
using Intent.Modelers.Domain.ValueObjects.Api;
using Intent.Modules.Common.CSharp.Builder;

namespace Intent.Modules.Domain.Guardrails.Contracts.Templates
{
    /// <summary>
    /// The C# type kind chosen by the "Type Configuration" stereotype. An absent stereotype or an empty
    /// Type property both mean <see cref="Class"/>.
    /// </summary>
    internal enum ContractTypeKind
    {
        Class,
        Record,

        /// <summary>
        /// A record whose attributes are its primary constructor's positional parameters, e.g.
        /// <c>record Money(decimal Amount, string Currency)</c>. Built as a <see cref="Record"/> with a primary constructor.
        /// </summary>
        PositionalRecord
    }

    internal static class ContractTypeKindExtensions
    {
        /// <summary>True for <see cref="ContractTypeKind.Record"/> and <see cref="ContractTypeKind.PositionalRecord"/>.</summary>
        public static bool IsRecord(this ContractTypeKind kind)
        {
            return kind != ContractTypeKind.Class;
        }

        public static ContractTypeKind GetContractTypeKind(this ClassModel model)
        {
            return model.TryGetTypeConfiguration(out var config)
                ? ToKind(config.Type().IsRecord(), config.Type().IsPositionalRecord())
                : ContractTypeKind.Class;
        }

        public static ContractTypeKind GetContractTypeKind(this ValueObjectModel model)
        {
            return model.TryGetTypeConfiguration(out var config)
                ? ToKind(config.Type().IsRecord(), config.Type().IsPositionalRecord())
                : ContractTypeKind.Class;
        }

        public static ContractTypeKind GetContractTypeKind(this RepositoryModel model)
        {
            var kind = model.TryGetTypeConfiguration(out var config)
                ? ToKind(config.Type().IsRecord(), config.Type().IsPositionalRecord())
                : ContractTypeKind.Class;
            return RejectPositionalWithoutAttributes(model.InternalElement, "Repository", kind);
        }

        public static ContractTypeKind GetContractTypeKind(this DataContractModel model)
        {
            return model.TryGetTypeConfiguration(out var config)
                ? ToKind(config.Type().IsRecord(), config.Type().IsPositionalRecord())
                : ContractTypeKind.Class;
        }

        public static ContractTypeKind GetContractTypeKind(this DomainServiceModel model)
        {
            var kind = model.TryGetTypeConfiguration(out var config)
                ? ToKind(config.Type().IsRecord(), config.Type().IsPositionalRecord())
                : ContractTypeKind.Class;
            return RejectPositionalWithoutAttributes(model.InternalElement, "Domain Service", kind);
        }

        public static ContractTypeKind ToKind(bool isRecord, bool isPositionalRecord)
        {
            if (isPositionalRecord)
            {
                return ContractTypeKind.PositionalRecord;
            }

            return isRecord ? ContractTypeKind.Record : ContractTypeKind.Class;
        }

        /// <summary>
        /// A positional record declares its attributes as positional parameters; a Repository or Domain Service has none.
        /// </summary>
        private static ContractTypeKind RejectPositionalWithoutAttributes(IElement element, string elementType, ContractTypeKind kind)
        {
            if (kind == ContractTypeKind.PositionalRecord)
            {
                throw new ElementException(element,
                    $"{elementType} '{element.Name}' has its Type Configuration Type set to positional record, but a positional record declares its attributes as positional parameters and a {elementType} has none. " +
                    "Set Type to class or record.");
            }

            return kind;
        }

        /// <summary>
        /// The single dispatch point from a <see cref="ContractTypeKind"/> to the builder type that gets populated.
        /// Records are built through <see cref="CSharpRecord"/> (a <see cref="CSharpClass"/>), so every member
        /// helper works unchanged. The inheritance shape is validated when the file is built, so a modelling
        /// error is reported against <paramref name="element"/>.
        /// </summary>
        public static CSharpFile AddContractType(
            this CSharpFile file,
            IElement element,
            string name,
            ContractTypeKind kind,
            Action<CSharpClass> configure,
            ContractTypeKind? baseKind = null)
        {
            if (kind.IsRecord())
            {
                // A positional record is a record whose attributes go on its primary constructor (see AddContractAttributes).
                return file.AddRecord(name, record =>
                {
                    ValidateInheritance(element, name, kind, baseKind);
                    configure(record);
                });
            }

            return file.AddClass(name, @class =>
            {
                ValidateInheritance(element, name, kind, baseKind);
                configure(@class);
            });
        }

        /// <summary>
        /// A record and a class can't inherit from each other. A record and a positional record can.
        /// </summary>
        private static void ValidateInheritance(IElement element, string name, ContractTypeKind kind, ContractTypeKind? baseKind)
        {
            if (baseKind.HasValue && baseKind.Value.IsRecord() != kind.IsRecord())
            {
                throw new ElementException(element,
                    $"'{name}' is configured as a {ToText(kind)} but inherits from a type configured as a {ToText(baseKind.Value)}. " +
                    "In C# a record can only inherit from another record, and a class cannot inherit from a record. " +
                    "Give both types a record Type (record or positional record), or neither.");
            }
        }

        public static string ToText(this ContractTypeKind kind)
        {
            return kind switch
            {
                ContractTypeKind.Class => "class",
                ContractTypeKind.Record => "record",
                ContractTypeKind.PositionalRecord => "positional record",
                _ => throw new InvalidOperationException($"Unsupported contract type kind: {kind}")
            };
        }
    }
}
