using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace CleanArchitecture.IdentityService.Application
{
    public class TwoFactorRequestDto
    {
        public TwoFactorRequestDto()
        {
        }

        public bool? Enable { get; set; }
        public string? TwoFactorCode { get; set; }
        public bool ResetSharedKey { get; set; }
        public bool ResetRecoveryCodes { get; set; }
        public bool ForgetMachine { get; set; }

        public static TwoFactorRequestDto Create(
            bool? enable,
            string? twoFactorCode,
            bool resetSharedKey,
            bool resetRecoveryCodes,
            bool forgetMachine)
        {
            return new TwoFactorRequestDto
            {
                Enable = enable,
                TwoFactorCode = twoFactorCode,
                ResetSharedKey = resetSharedKey,
                ResetRecoveryCodes = resetRecoveryCodes,
                ForgetMachine = forgetMachine
            };
        }
    }
}