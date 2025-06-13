using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace CleanArchitecture.IdentityService.Application.Identity
{
    public class TwoFactorResponseDto
    {
        public TwoFactorResponseDto()
        {
            SharedKey = null!;
        }

        public string SharedKey { get; set; }
        public int RecoveryCodesLeft { get; set; }
        public List<string>? RecoveryCodes { get; set; }
        public bool IsTwoFactorEnabled { get; set; }
        public bool IsMachineRemembered { get; set; }

        public static TwoFactorResponseDto Create(
            string sharedKey,
            int recoveryCodesLeft,
            List<string>? recoveryCodes,
            bool isTwoFactorEnabled,
            bool isMachineRemembered)
        {
            return new TwoFactorResponseDto
            {
                SharedKey = sharedKey,
                RecoveryCodesLeft = recoveryCodesLeft,
                RecoveryCodes = recoveryCodes,
                IsTwoFactorEnabled = isTwoFactorEnabled,
                IsMachineRemembered = isMachineRemembered
            };
        }
    }
}