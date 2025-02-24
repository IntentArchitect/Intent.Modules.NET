using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using Intent.Modules.Security.Shared;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.AspNetCore.Templates.Permissions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class PermissionsTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.AspNetCore.Permissions";

        
        private readonly IEnumerable<SecurityConstant> _securityConstants;

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public PermissionsTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            _securityConstants = GetSecurityConstants(outputTarget);
            this.FulfillsRole(Roles.Security.Permissions);

            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddClass("Permissions", @class =>
                {
                    @class.Static();

                    foreach (var @const in _securityConstants)
                    {
                        @class.AddField("string", @const.Name, @field =>
                        {
                            @field.Constant($"\"{@const.Value}\"");
                        });
                    }

                    @class.AddMethod(UseType("System.Collections.Generic.IEnumerable<string>"), "All", method =>
                    {
                        method.Static();

                        foreach (var @const in _securityConstants)
                        {
                            method.AddStatement($"yield return {@const.Name};");
                        }

                        if (!_securityConstants.Any())
                        {
                            method.AddStatement($"yield break;");
                        }
                    });

                    @class.AddMethod(UseType("System.Collections.Generic.IEnumerable<string>"), "Roles", method =>
                    {
                        method.Static();

                        foreach (var @const in _securityConstants.Where(c => c.Type == SecurityType.Role))
                        {
                            method.AddStatement($"yield return {@const.Name};");
                        }

                        if (!_securityConstants.Where(c => c.Type == SecurityType.Role).Any())
                        {
                            method.AddStatement($"yield break;");
                        }
                    });

                    @class.AddMethod(UseType("System.Collections.Generic.IEnumerable<string>"), "Policies", method =>
                    {
                        method.Static();

                        foreach (var @const in _securityConstants.Where(c => c.Type == SecurityType.Policy))
                        {
                            method.AddStatement($"yield return {@const.Name};");
                        }

                        if (!_securityConstants.Where(c => c.Type == SecurityType.Policy).Any())
                        {
                            method.AddStatement($"yield break;");
                        }
                    });
                });
        }

        public int Order => 0;

        public override bool CanRunTemplate()
        {
            return _securityConstants.Any();
        }

        private IEnumerable<SecurityConstant> GetSecurityConstants(IOutputTarget outputTarget)
        {
            List<SecurityConstant> securityModels = [];

            var servicesDesigner = outputTarget.ExecutionContext.MetadataManager.GetDesigner(outputTarget.ExecutionContext.GetApplicationConfig().Id, Designers.Services);
            var domainDesigner = outputTarget.ExecutionContext.MetadataManager.GetDesigner(outputTarget.ExecutionContext.GetApplicationConfig().Id, Designers.Domain);

            securityModels.AddRange(GetDesignerSecurityConstants(servicesDesigner));
            securityModels.AddRange(GetDesignerSecurityConstants(domainDesigner));

            return securityModels;
        }

        private IEnumerable<SecurityConstant> GetDesignerSecurityConstants(IDesigner designer)
        {
            List<SecurityConstant> securityModels = [];

            // Security "Roles" and "Policies", defined in Intent.Modules.Metadata.Security
            var roles = designer.GetElementsOfType(Constants.Security.Role)
                .Select(r => new SecurityConstant(SecurityHelper.NormalizeRoleOrPolicyName(r.Name, SecurityType.Role), r.Name, SecurityType.Role));
            var policies = designer.GetElementsOfType(Constants.Security.Policy)
                .Select(r => new SecurityConstant(SecurityHelper.NormalizeRoleOrPolicyName(r.Name, SecurityType.Policy), r.Name, SecurityType.Policy));

            securityModels.AddRange(roles);
            securityModels.AddRange(policies);

            // "Secured" Stereotype
            var securedElements = designer.Elements.Where(e => e.HasStereotype(Constants.Security.Secured.Id));

            var rolesList = securedElements.SelectMany(e =>
            {
                List<string> roles = [];
                var securedStereo = e.GetStereotypes(Constants.Security.Secured.Id);

                foreach(IStereotype secured in securedStereo)
                {
                    var tempRoles = secured?.GetProperty<string?>(Constants.Security.Secured.Properties.CommaSeparatedRoles) ?? string.Empty;
                    roles.AddRange(tempRoles.Split(","));
                }

                return roles;
                
            }).Where(r => !string.IsNullOrWhiteSpace(r));

            var policiesList = securedElements.SelectMany(e =>
            {
                List<string> policies = [];
                var securedStereo = e.GetStereotypes(Constants.Security.Secured.Id);

                foreach (IStereotype secured in securedStereo)
                {
                    var tempPolicies = secured?.GetProperty<string?>(Constants.Security.Secured.Properties.CommaSeparatedPolicies) ?? string.Empty;
                    policies.AddRange(tempPolicies.Split(","));
                }
                return policies;
            }).Where(r => !string.IsNullOrWhiteSpace(r));

            securityModels.AddRange(rolesList.Select(r => new SecurityConstant(SecurityHelper.NormalizeRoleOrPolicyName(r, SecurityType.Role), r, SecurityType.Role)));
            securityModels.AddRange(policiesList.Select(r => new SecurityConstant(SecurityHelper.NormalizeRoleOrPolicyName(r, SecurityType.Policy), r, SecurityType.Policy)));

            // datamasking - should really be using secured stereotype as well
            var maskedElements = designer.Elements.Where(e => e.HasStereotype(Constants.Security.DataMasking.Id));

            var maskedRolesList = maskedElements.SelectMany(e =>
            {
                var roles = e.GetStereotype(Constants.Security.DataMasking.Id)?.GetProperty<string?>(Constants.Security.DataMasking.Properties.CommaSeparatedRoles) ?? string.Empty;
                return roles.Split(",");
            }).Where(r => !string.IsNullOrWhiteSpace(r));

            var maskedPoliciesList = maskedElements.SelectMany(e =>
            {
                var policies = e.GetStereotype(Constants.Security.DataMasking.Id)?.GetProperty<string?>(Constants.Security.DataMasking.Properties.CommaSeparatedPolicies) ?? string.Empty;
                return policies.Split(",");
            }).Where(r => !string.IsNullOrWhiteSpace(r));

            securityModels.AddRange(maskedRolesList.Select(r => new SecurityConstant(SecurityHelper.NormalizeRoleOrPolicyName(r, SecurityType.Role), r, SecurityType.Role)));
            securityModels.AddRange(maskedPoliciesList.Select(r => new SecurityConstant(SecurityHelper.NormalizeRoleOrPolicyName(r, SecurityType.Policy), r, SecurityType.Policy)));

            return securityModels.GroupBy(c => c.Name).Select(c => c.First());
        }
        
        [IntentManaged(Mode.Fully)]
        public CSharpFile CSharpFile { get; }

        [IntentManaged(Mode.Fully)]
        protected override CSharpFileConfig DefineFileConfig()
        {
            return CSharpFile.GetConfig();
        }

        [IntentManaged(Mode.Fully)]
        public override string TransformText()
        {
            return CSharpFile.ToString();
        }

        private record SecurityConstant(string Name, string Value, SecurityType Type);

        
    }
}