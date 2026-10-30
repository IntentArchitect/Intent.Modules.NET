using Intent.Metadata.Models;

namespace Intent.Modules.Services.Guardrails.Contracts.Templates.IntegrationCommandContract
{
    // Hand-written partial (not generated): the designer-owned partial's base list and members are
    // regenerated, so the File Config members live here. See ContractFileGroups, GlobalNamespaceOutput
    // and OptionalModelerMetadata (raw element and stereotype ids, no Eventing modeler dependency).
    public partial class IntegrationCommandContractTemplate : IServicesContractTemplate
    {
        public IElement ContractElement => Model;

        public bool HasNoNamespace => Model.GetOptionalModelerFileConfig()?.NoNamespace() == true;

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
        /// No Namespace: removes the namespace wrapper CSharpFile.ToString() always writes.
        /// </summary>
        public override string RunTemplate()
        {
            var output = base.RunTemplate();
            return HasNoNamespace ? GlobalNamespaceOutput.UnwrapGlobalNamespace(output) : output;
        }
    }
}
