using Intent.Domain.Guardrails.Contracts.Api;
using Intent.Metadata.Models;

namespace Intent.Modules.Domain.Guardrails.Contracts.Templates.RepositoryContract
{
    // Hand-written partial (not generated): the designer-owned partial's base list and members are
    // regenerated, so the File Config members live here. See ContractFileGroups and GlobalNamespaceOutput.
    public partial class RepositoryContractTemplate : IDomainContractTemplate
    {
        public IElement ContractElement => Model.InternalElement;

        public bool HasNoNamespace => Model.GetFileConfig()?.NoNamespace() == true;

        /// <summary>
        /// A template that shares its output file with other contract templates only runs when it
        /// owns that file; the owner writes every member's type. Discoverability is unchanged.
        /// </summary>
        public override bool CanRunTemplate()
        {
            return base.CanRunTemplate() && this.IsFileOwner();
        }

        /// <summary>
        /// No Namespace: the file config omits an empty namespace, so the base property would fall back
        /// to the project name. Returning empty makes references from other files unqualified with no using.
        /// </summary>
        public override string Namespace => HasNoNamespace ? string.Empty : base.Namespace;

        /// <summary>
        /// Drops the System.Collections.Generic qualifier that Intent adds when the file also imports System.Collections.
        /// See ContractTypeNames.
        /// </summary>
        public override string NormalizeNamespace(string foreignType) => this.WithoutArityOnlyQualifiers(base.NormalizeNamespace(foreignType));

        /// <summary>
        /// No Namespace: removes the namespace wrapper CSharpFile.ToString() always writes. Only the file's
        /// owner runs, and every member of a shared file must agree on No Namespace (see ContractFileGroups).
        /// </summary>
        public override string RunTemplate()
        {
            var output = base.RunTemplate();
            return HasNoNamespace ? GlobalNamespaceOutput.UnwrapGlobalNamespace(output) : output;
        }
    }
}
