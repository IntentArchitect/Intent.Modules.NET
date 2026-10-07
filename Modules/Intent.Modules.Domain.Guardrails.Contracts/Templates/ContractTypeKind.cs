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
        Record
    }

    internal static class ContractTypeKindExtensions
    {
        public static ContractTypeKind GetContractTypeKind(this ClassModel model)
        {
            return model.TryGetTypeConfiguration(out var config) && config.Type().IsRecord()
                ? ContractTypeKind.Record
                : ContractTypeKind.Class;
        }

        public static ContractTypeKind GetContractTypeKind(this ValueObjectModel model)
        {
            return model.TryGetTypeConfiguration(out var config) && config.Type().IsRecord()
                ? ContractTypeKind.Record
                : ContractTypeKind.Class;
        }

        public static ContractTypeKind GetContractTypeKind(this RepositoryModel model)
        {
            return model.TryGetTypeConfiguration(out var config) && config.Type().IsRecord()
                ? ContractTypeKind.Record
                : ContractTypeKind.Class;
        }

        public static ContractTypeKind GetContractTypeKind(this DataContractModel model)
        {
            return model.TryGetTypeConfiguration(out var config) && config.Type().IsRecord()
                ? ContractTypeKind.Record
                : ContractTypeKind.Class;
        }

        public static ContractTypeKind GetContractTypeKind(this DomainServiceModel model)
        {
            return model.TryGetTypeConfiguration(out var config) && config.Type().IsRecord()
                ? ContractTypeKind.Record
                : ContractTypeKind.Class;
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
            if (kind == ContractTypeKind.Record)
            {
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

        private static void ValidateInheritance(IElement element, string name, ContractTypeKind kind, ContractTypeKind? baseKind)
        {
            if (baseKind.HasValue && baseKind.Value != kind)
            {
                throw new ElementException(element,
                    $"'{name}' is configured as a {ToText(kind)} but inherits from a type configured as a {ToText(baseKind.Value)}. " +
                    "In C# a record can only inherit from another record, and a class cannot inherit from a record. Give both types the same Type Configuration.");
            }
        }

        private static string ToText(ContractTypeKind kind)
        {
            return kind switch
            {
                ContractTypeKind.Class => "class",
                ContractTypeKind.Record => "record",
                _ => throw new InvalidOperationException($"Unsupported contract type kind: {kind}")
            };
        }
    }
}
