using Intent.Exceptions;
using Intent.Metadata.Models;
using Intent.Modelers.Services.Api;
using Intent.Modelers.Services.CQRS.Api;
using Intent.Services.Guardrails.Contracts.Api;

namespace Intent.Modules.Services.Guardrails.Contracts.Templates
{
    /// <summary>
    /// The C# declaration kind a contract is emitted as, chosen by the "Type Configuration"
    /// stereotype. An absent stereotype or an empty Type means <see cref="Class"/>.
    /// </summary>
    public enum ContractTypeKind
    {
        Class,
        Record,

        /// <summary>
        /// A record whose fields are its primary constructor's positional parameters, e.g.
        /// <c>record CancelOrderCommand(int OrderNumber)</c>. Built as a <see cref="Record"/> with a primary constructor.
        /// </summary>
        PositionalRecord
    }

    public static class ContractTypeKindExtensions
    {
        /// <summary>True for <see cref="ContractTypeKind.Record"/> and <see cref="ContractTypeKind.PositionalRecord"/>.</summary>
        public static bool IsRecord(this ContractTypeKind kind)
        {
            return kind != ContractTypeKind.Class;
        }

        /// <summary>
        /// A positional record declares its fields as positional parameters, and a Service has none, so that Type is an
        /// error on the Service.
        /// </summary>
        public static ContractTypeKind GetContractTypeKind(this ServiceModel model)
        {
            var type = model.GetTypeConfiguration()?.Type();
            var kind = ToKind(type?.IsRecord() == true, type?.IsPositionalRecord() == true);
            if (kind == ContractTypeKind.PositionalRecord)
            {
                throw new ElementException(model.InternalElement,
                    $"Service '{model.Name}' has its Type Configuration Type set to positional record, but a positional record declares its fields as positional parameters and a Service has none. " +
                    "Set Type to class or record.");
            }

            return kind;
        }

        public static ContractTypeKind GetContractTypeKind(this CommandModel model)
        {
            var type = model.GetTypeConfiguration()?.Type();
            return ToKind(type?.IsRecord() == true, type?.IsPositionalRecord() == true);
        }

        public static ContractTypeKind GetContractTypeKind(this QueryModel model)
        {
            var type = model.GetTypeConfiguration()?.Type();
            return ToKind(type?.IsRecord() == true, type?.IsPositionalRecord() == true);
        }

        /// <summary>
        /// Also validates that a DTO and the DTO it inherits from are both records or both classes, since a record cannot
        /// inherit from a class and a class cannot inherit from a record. A record and a positional record can inherit from
        /// each other.
        /// </summary>
        public static ContractTypeKind GetContractTypeKind(this DTOModel model)
        {
            var kind = model.GetDeclaredContractTypeKind();

            var parentDto = model.ParentDtoTypeReference?.Element?.AsDTOModel();
            if (parentDto != null)
            {
                var parentKind = parentDto.GetDeclaredContractTypeKind();
                if (parentKind.IsRecord() != kind.IsRecord())
                {
                    throw new ElementException(model.InternalElement,
                        $"DTO '{model.Name}' is a {Describe(kind)} but inherits from DTO '{parentDto.Name}', which is a {Describe(parentKind)}. " +
                        "In C# a record cannot inherit from a class and a class cannot inherit from a record. " +
                        "Give both DTOs a record Type (record or positional record), or neither.");
                }
            }

            return kind;
        }

        /// <summary>The DTO's own Type Configuration kind, without the inheritance check.</summary>
        public static ContractTypeKind GetDeclaredContractTypeKind(this DTOModel model)
        {
            var type = model.GetTypeConfiguration()?.Type();
            return ToKind(type?.IsRecord() == true, type?.IsPositionalRecord() == true);
        }

        /// <summary>
        /// The kind of a Message, Integration Command or Eventing DTO (raw elements, see <see cref="OptionalModelerMetadata"/>),
        /// from this module's filter-targeted Type Configuration. Like <see cref="GetContractTypeKind(DTOModel)"/>, an Eventing DTO
        /// and the Eventing DTO it inherits from must both be records or both be classes.
        /// </summary>
        public static ContractTypeKind GetContractTypeKind(this IElement element)
        {
            var kind = element.GetDeclaredContractTypeKind();

            var parentDto = element.IsEventingDto() ? element.GetEventingParentDtoTypeReference()?.Element as IElement : null;
            if (parentDto != null && parentDto.IsEventingDto())
            {
                var parentKind = parentDto.GetDeclaredContractTypeKind();
                if (parentKind.IsRecord() != kind.IsRecord())
                {
                    throw new ElementException(element,
                        $"Eventing DTO '{element.Name}' is a {Describe(kind)} but inherits from Eventing DTO '{parentDto.Name}', which is a {Describe(parentKind)}. " +
                        "In C# a record cannot inherit from a class and a class cannot inherit from a record. " +
                        "Give both Eventing DTOs a record Type (record or positional record), or neither.");
                }
            }

            return kind;
        }

        /// <summary>The raw element's own Type Configuration kind, without the inheritance check.</summary>
        public static ContractTypeKind GetDeclaredContractTypeKind(this IElement element)
        {
            var configuration = element.GetOptionalModelerTypeConfiguration();
            return ToKind(configuration?.IsRecord() == true, configuration?.IsPositionalRecord() == true);
        }

        public static string Describe(this ContractTypeKind kind)
        {
            return kind switch
            {
                ContractTypeKind.Record => "record",
                ContractTypeKind.PositionalRecord => "positional record",
                _ => "class"
            };
        }

        private static ContractTypeKind ToKind(bool isRecord, bool isPositionalRecord)
        {
            if (isPositionalRecord)
            {
                return ContractTypeKind.PositionalRecord;
            }

            return isRecord ? ContractTypeKind.Record : ContractTypeKind.Class;
        }
    }
}
