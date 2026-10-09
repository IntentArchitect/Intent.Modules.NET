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
        Record
    }

    public static class ContractTypeKindExtensions
    {
        public static ContractTypeKind GetContractTypeKind(this ServiceModel model)
        {
            return ToKind(model.GetTypeConfiguration()?.Type()?.IsRecord() == true);
        }

        public static ContractTypeKind GetContractTypeKind(this CommandModel model)
        {
            return ToKind(model.GetTypeConfiguration()?.Type()?.IsRecord() == true);
        }

        public static ContractTypeKind GetContractTypeKind(this QueryModel model)
        {
            return ToKind(model.GetTypeConfiguration()?.Type()?.IsRecord() == true);
        }

        /// <summary>
        /// Also validates that a DTO and the DTO it inherits from share the same kind, since a
        /// record cannot inherit from a class and a class cannot inherit from a record.
        /// </summary>
        public static ContractTypeKind GetContractTypeKind(this DTOModel model)
        {
            var kind = GetDeclaredKind(model);

            var parentDto = model.ParentDtoTypeReference?.Element?.AsDTOModel();
            if (parentDto != null)
            {
                var parentKind = GetDeclaredKind(parentDto);
                if (parentKind != kind)
                {
                    throw new ElementException(model.InternalElement,
                        $"DTO '{model.Name}' is a {Describe(kind)} but inherits from DTO '{parentDto.Name}', which is a {Describe(parentKind)}. " +
                        "In C# a record cannot inherit from a class and a class cannot inherit from a record. " +
                        "Give both DTOs the same Type Configuration.");
                }
            }

            return kind;
        }

        private static ContractTypeKind GetDeclaredKind(DTOModel model)
        {
            return ToKind(model.GetTypeConfiguration()?.Type()?.IsRecord() == true);
        }

        /// <summary>
        /// The kind of a Message, Integration Command or Eventing DTO (raw elements, see <see cref="OptionalModelerMetadata"/>),
        /// from this module's filter-targeted Type Configuration. Like <see cref="GetContractTypeKind(DTOModel)"/>, an Eventing DTO
        /// must share the kind of the Eventing DTO it inherits from.
        /// </summary>
        public static ContractTypeKind GetContractTypeKind(this IElement element)
        {
            var kind = GetDeclaredKind(element);

            var parentDto = element.IsEventingDto() ? element.GetEventingParentDtoTypeReference()?.Element as IElement : null;
            if (parentDto != null && parentDto.IsEventingDto())
            {
                var parentKind = GetDeclaredKind(parentDto);
                if (parentKind != kind)
                {
                    throw new ElementException(element,
                        $"Eventing DTO '{element.Name}' is a {Describe(kind)} but inherits from Eventing DTO '{parentDto.Name}', which is a {Describe(parentKind)}. " +
                        "In C# a record cannot inherit from a class and a class cannot inherit from a record. " +
                        "Give both Eventing DTOs the same Type Configuration.");
                }
            }

            return kind;
        }

        private static ContractTypeKind GetDeclaredKind(IElement element)
        {
            return ToKind(element.GetOptionalModelerTypeConfiguration()?.IsRecord() == true);
        }

        private static ContractTypeKind ToKind(bool isRecord)
        {
            return isRecord ? ContractTypeKind.Record : ContractTypeKind.Class;
        }

        private static string Describe(ContractTypeKind kind)
        {
            return kind == ContractTypeKind.Record ? "record" : "class";
        }
    }
}
