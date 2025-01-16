using System;
using EntityFrameworkCore.MySql.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.DataMasking.DataMaskConverter", Version = "1.0")]

namespace EntityFrameworkCore.MySql.Infrastructure.Interceptors
{
    public class DataMaskConverter : ValueConverter<string, string>
    {
        private readonly Func<bool> _isAuthorized;
        /// <summary>
        /// Creates a new instance of <see cref="DataMaskConverter"/>.
        /// </summary>
        /// <param name="isAuthorized">A function which returns whether the current user is authorized to see the unmasked version of the data.</param>
        /// <param name="maskData">A function which is used to mask the data when <paramref name="isAuthorized"/> returns <see langword="false"/>.</param>
        private DataMaskConverter(Func<bool> isAuthorized, Func<string, string> maskData) : base(originalValue => originalValue, originalValue => isAuthorized() ? originalValue : maskData(originalValue))
        {
            _isAuthorized = isAuthorized;
        }

        public bool IsMasked() => !_isAuthorized();

        public static ValueConverter<string, string> FixedLength(
            ICurrentUserService currentUserService,
            char maskCharacter,
            int maskLength,
            string[]? roles = null,
            string[]? policies = null)
        {
            roles ??= [];
            policies ??= [];
            var fixedLengthMask = string.Empty.PadLeft(maskLength, maskCharacter);
            return new DataMaskConverter(
                isAuthorized: () => IsAuthorized(currentUserService, roles, policies),
                maskData: _ => fixedLengthMask);
        }

        public static ValueConverter<string, string> VariableLength(
            ICurrentUserService currentUserService,
            char maskCharacter,
            string[]? roles = null,
            string[]? policies = null)
        {
            roles ??= [];
            policies ??= [];
            return new DataMaskConverter(
                isAuthorized: () => IsAuthorized(currentUserService, roles, policies),
                maskData: originalValue => string.Empty.PadLeft(originalValue.Length, maskCharacter));
        }

        public static ValueConverter<string, string> Partial(
            ICurrentUserService currentUserService,
            char maskCharacter,
            int unmaskedPrefixLength,
            int unmaskedSuffixLength,
            string[]? roles = null,
            string[]? policies = null)
        {
            roles ??= [];
            policies ??= [];
            return new DataMaskConverter(
                isAuthorized: () => IsAuthorized(currentUserService, roles, policies),
                maskData: originalValue =>
                {
                    if (unmaskedPrefixLength + unmaskedSuffixLength >= originalValue.Length)
                    {
                        return string.Empty.PadLeft(originalValue.Length, maskCharacter);
                    }
                    var prefix = originalValue[..unmaskedPrefixLength];
                    var suffix = originalValue[^unmaskedSuffixLength..];
                    var maskLength = originalValue.Substring(unmaskedPrefixLength, originalValue.Length - unmaskedPrefixLength - unmaskedSuffixLength);
                    return $"{prefix}{string.Empty.PadLeft(maskLength.Length, maskCharacter)}{suffix}";
                });
        }

        public static bool IsAuthorized(ICurrentUserService currentUserService, string[] roles, string[] policies)
        {
            // Must be an authenticated user
            if (currentUserService.UserId is null)
            {
                return false;
            }

            // Role-based authorization
            if (roles.Length > 0)
            {
                var authorized = false;

                foreach (var role in roles)
                {
                    var isInRole = currentUserService.IsInRoleAsync(role).GetAwaiter().GetResult();

                    if (isInRole)
                    {
                        authorized = true;
                        break;
                    }
                }

                // Must be a member of at least one role in roles
                if (!authorized)
                {
                    return false;
                }
            }

            // Policy-based authorization
            if (policies.Length > 0)
            {
                var authorized = false;

                foreach (var policy in policies)
                {
                    var isAuthorized = currentUserService.AuthorizeAsync(policy).GetAwaiter().GetResult();

                    if (isAuthorized)
                    {
                        authorized = true;
                        break;
                    }
                }

                // Must be authorized by at least one policy
                if (!authorized)
                {
                    return false;
                }
            }
            return true;
        }
    }
}