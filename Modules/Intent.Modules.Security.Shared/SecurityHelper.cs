using Intent.Engine;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;

namespace Intent.Modules.Security.Shared;

public  static partial class SecurityHelper
{
    /// <summary>
    /// The role keywords. If a role starts or ends with any of these words, then the role const name is not prefixed with "Role"
    /// </summary>
    public static readonly string[] RoleKeywords = ["role", "permission"];
    /// <summary>
    /// The policy keywords. If a policy starts or ends with any of these words, then the policy const name is not prefixed with "Policy"
    /// </summary>
    public static readonly string[] PolicyKeywords = ["policy", "permission"];

    /// <summary>
    /// Converts the list of roles to the equivilant, correctly formatted Permission constants (if available). If the Permission constants is not available, the roles are returned unmodified.
    /// </summary>
    /// <param name="roles">The list of roles to convert</param>
    /// <param name="template">The template on which the roles will be used. A "using" statement is automatically added to the template if required for the permissions</param>
    /// <param name="conversionOptions">Options for how the data is converted
    /// <returns>The string representing the roles as Permission constants, in the specified format</returns>
    public static string RolesToPermissionConstants(IReadOnlyCollection<string> roles, IIntentTemplate template, PermissionConversionOptions? conversionOptions = null)
    {
        conversionOptions ??= new PermissionConversionOptions();

        return SecurityNamesToPermissionConstants(roles, SecurityType.Role, template, conversionOptions);
    }

    /// <summary>
    /// Converts the list of policies to the equivilant, correctly formatted Permission constants (if available). If the Permission constants is not available, the policies are returned unmodified.
    /// </summary>
    /// <param name="policies">The list of policies to convert</param>
    /// <param name="template">The template on which the roles will be used. A "using" statement is automatically added to the template if required for the permissions</param>
    /// <param name="collectionFormat">The format for the collection string. If null, the default of "$\"{0}\"" is used.</param>
    /// <param name="nameFormat">The format for a specific policy string. If null, the default of {{{0}}} is used.</param>
    /// <returns>The string representing the policies as Permission constants, in the specified format</returns>
    public static string PoliciesToPermissionConstants(IReadOnlyCollection<string> policies, IIntentTemplate template, PermissionConversionOptions? conversionOptions = null)
    {
        conversionOptions ??= new PermissionConversionOptions();

        return SecurityNamesToPermissionConstants(policies, SecurityType.Policy, template, conversionOptions);
    }

    /// <summary>
    /// Converts the specific role to the equivilant, correctly formatted Permission constants (if available). If the Permission constants is not available, the role is returned unmodified. 
    /// </summary>
    /// <param name="role">The role name</param>
    /// <param name="template">The template on which the roles will be used. A "using" statement is automatically added to the template if required for the permissions</param>
    /// <param name="outputFullyQualifiedName">If set to true, the "using" will not be added to the template and the result will be the fully qualified name</param>
    /// <returns>The string representing the role as a Permission constant</returns>
    public static string RoleToPermissionConstant(string role, IIntentTemplate template, bool outputFullyQualifiedName)
    {
        if (template.ExecutionContext.FindTemplateInstance(Roles.Security.Permissions) is not ICSharpFileBuilderTemplate permissionsTemplate)
        {
            return role;
        }

        return !outputFullyQualifiedName ? $"{template!.GetTypeName(permissionsTemplate)}.{NormalizeRoleOrPolicyName(role, SecurityType.Role)}" :
            $"{permissionsTemplate.Namespace}.{permissionsTemplate.ClassName}.{NormalizeRoleOrPolicyName(role, SecurityType.Role)}";
    }

    public static bool TryRoleToPermissionConstant(string role, IIntentTemplate template, bool outputFullyQualifiedName, out string roleName)
    {
        if (template.ExecutionContext.FindTemplateInstance(Roles.Security.Permissions) is not ICSharpFileBuilderTemplate permissionsTemplate)
        {
            roleName = role;
            return false;
        }

        roleName = !outputFullyQualifiedName ? $"{template!.GetTypeName(permissionsTemplate)}.{NormalizeRoleOrPolicyName(role, SecurityType.Role)}" :
            $"{permissionsTemplate.Namespace}.{permissionsTemplate.ClassName}.{NormalizeRoleOrPolicyName(role, SecurityType.Role)}";
        return true;
    }

    /// <summary>
    /// Converts the specific policy to the equivilant, correctly formatted Permission constants (if available). If the Permission constants is not available, the policy is returned unmodified. 
    /// </summary>
    /// <param name="policy">The policy name</param>
    /// <param name="template">The template on which the roles will be used. A "using" statement is automatically added to the template if required for the permissions</param>
    /// <returns>The string representing the policy as a Permission constant</returns>
    public static string PolicyToPermissionConstant(string policy, IIntentTemplate template, bool outputFullyQualifiedName)
    {
        if (template.ExecutionContext.FindTemplateInstance(Roles.Security.Permissions) is not ICSharpFileBuilderTemplate permissionsTemplate)
        {
            return policy;
        }

        return !outputFullyQualifiedName ? $"{template!.GetTypeName(permissionsTemplate)}.{NormalizeRoleOrPolicyName(policy, SecurityType.Policy)}" :
            $"{permissionsTemplate.Namespace}.{permissionsTemplate.ClassName}.{NormalizeRoleOrPolicyName(policy, SecurityType.Policy)}";
    }

    public static bool TryPolicyToPermissionConstant(string policy, IIntentTemplate template, bool outputFullyQualifiedName, out string policyName)
    {
        if (template.ExecutionContext.FindTemplateInstance(Roles.Security.Permissions) is not ICSharpFileBuilderTemplate permissionsTemplate)
        {
            policyName = policy;
            return false;
        }

        policyName = !outputFullyQualifiedName ? $"{template!.GetTypeName(permissionsTemplate)}.{NormalizeRoleOrPolicyName(policy, SecurityType.Policy)}" :
            $"{permissionsTemplate.Namespace}.{permissionsTemplate.ClassName}.{NormalizeRoleOrPolicyName(policy, SecurityType.Policy)}";
        return true;
    }

    /// <summary>
    /// Convert the role or policy name to a normalized name suitable for a static constant.
    /// </summary>
    /// <param name="securityName">The name of the role or policy</param>
    /// <param name="securityType">Role or policy</param>
    /// <returns>The normalized name</returns>
    public static string NormalizeRoleOrPolicyName(string securityName, SecurityType securityType)
    {
        // replace any non letters and numbers with "-"
        securityName = SecurityNameSanitizeRegex().Replace(securityName, "-");
        securityName = securityName.ToPascalCase();

        if (securityType == SecurityType.Role && !RoleKeywords.Any(w => securityName.StartsWith(w, StringComparison.CurrentCultureIgnoreCase)
            || securityName.EndsWith(w, StringComparison.CurrentCultureIgnoreCase)))
        {
            securityName = $"Role{securityName}";
        }

        if (securityType == SecurityType.Policy && !PolicyKeywords.Any(w => securityName.StartsWith(w, StringComparison.CurrentCultureIgnoreCase)
            || securityName.EndsWith(w, StringComparison.CurrentCultureIgnoreCase)))
        {
            securityName = $"Policy{securityName}";
        }

        return securityName;
    }

    private static string SecurityNamesToPermissionConstants(IReadOnlyCollection<string> securityNames, SecurityType type, IIntentTemplate template,
        PermissionConversionOptions conversionOptions)
    {
        List<string>? normalizedNames = [];
        var conversionOccured = false;

        foreach (string name in securityNames)
        {
            if (type == SecurityType.Role)
            {
                var converted = TryRoleToPermissionConstant(name, template, conversionOptions.OuputFullyQualifiedName, out var convertedRole);
                conversionOccured = converted ? converted : conversionOccured;

                normalizedNames.Add(convertedRole);
            }
            else
            {
                var converted = TryPolicyToPermissionConstant(name, template, conversionOptions.OuputFullyQualifiedName, out var convertedRole);
                conversionOccured = converted ? converted : conversionOccured;

                normalizedNames.Add(PolicyToPermissionConstant(name, template, conversionOptions.OuputFullyQualifiedName));
            }
        }

        if (normalizedNames.Count == 1)
        {
            return conversionOccured ? normalizedNames.First() : $"\"{normalizedNames.First()}\"";
        }

        return conversionOccured ? string.Format(conversionOptions.ConvertedCollectionFormat, string.Join(",", normalizedNames.Select(n => string.Format(conversionOptions.ConvertedNameFormat, n)))) :
            string.Format(conversionOptions.UnconvertedCollectionFormat, string.Join(",", normalizedNames.Select(n => string.Format(conversionOptions.UnconvertedNameFormat, n))));
    }

    [GeneratedRegex(@"[^a-zA-Z0-9]")]
    private static partial Regex SecurityNameSanitizeRegex();
}
