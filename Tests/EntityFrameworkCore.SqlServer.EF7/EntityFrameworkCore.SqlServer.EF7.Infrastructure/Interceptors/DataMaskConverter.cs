using System.Linq;
using EntityFrameworkCore.SqlServer.EF7.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Modules.EntityFrameworkCore.DataMasking.DataMaskConverter", Version = "1.0")]

namespace EntityFrameworkCore.SqlServer.EF7.Infrastructure.Interceptors
{
    public class DataMaskConverter : ValueConverter<string, string>
    {
        public DataMaskConverter(ICurrentUserService currentUserService,
            MaskDataType maskType,
            string maskCharacter = "*",
            int maskLength = 0,
            int unmaskedPrefixLength = 0,
            int unmaskedSuffixLength = 0,
            string[]? roles = default,
            string[]? policies = default) : base(v => v, v => MaskValue(currentUserService, maskType, v, maskCharacter, maskLength, unmaskedPrefixLength, unmaskedSuffixLength, roles, policies))
        {
        }

        public static bool UserAuthorized(
            ICurrentUserService currentUserService,
            string[]? roles = default,
            string[]? policies = default)
        {
            if ((roles is null && policies is null) || (roles?.Length == 0 && policies?.Length == 0))
            {
                return false;
            }

            if (roles != null && roles.Length > 0)
            {
                foreach (var role in roles)
                {
                    var isInRole = currentUserService.IsInRoleAsync(role).GetAwaiter().GetResult();

                    if (isInRole)
                    {
                        return true;
                    }
                }
            }

            if (policies != null && policies.Length > 0)
            {
                foreach (var policy in policies)
                {
                    var isAuthorized = currentUserService.AuthorizeAsync(policy).GetAwaiter().GetResult();

                    if (isAuthorized)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private static string MaskValue(
            ICurrentUserService currentUserService,
            MaskDataType maskType,
            string value,
            string maskCharacter,
            int maskLength,
            int unmaskedPrefixLength,
            int unmaskedSuffixLength,
            string[]? roles = default,
            string[]? policies = default)
        {
            if (UserAuthorized(currentUserService, roles, policies))
            {
                return value;
            }

            if (string.IsNullOrWhiteSpace(value))
            {
                return value;
            }

            if (maskType == MaskDataType.SetLength)
            {
                return string.Concat(Enumerable.Repeat(maskCharacter, maskLength));
            }

            if (maskType == MaskDataType.VariableLength)
            {
                return string.Concat(Enumerable.Repeat(maskCharacter, value.Length));
            }

            if (unmaskedPrefixLength + unmaskedSuffixLength >= value.Length)
            {
                return string.Concat(Enumerable.Repeat(maskCharacter, value.Length));
            }
            var prefix = value[..unmaskedPrefixLength];
            var suffix = value[^unmaskedSuffixLength..];
            var toMask = value.Substring(unmaskedPrefixLength, value.Length - unmaskedPrefixLength - unmaskedSuffixLength);
            var maskedPortion = string.Concat(Enumerable.Repeat(maskCharacter, toMask.Length));
            return $"{prefix}{maskedPortion}{suffix}";
        }
    }

    public enum MaskDataType
    {
        SetLength,

        VariableLength,

        PartialMask
    }
}