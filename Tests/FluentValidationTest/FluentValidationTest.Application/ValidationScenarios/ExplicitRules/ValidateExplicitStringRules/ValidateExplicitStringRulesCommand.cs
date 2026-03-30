using FluentValidationTest.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace FluentValidationTest.Application.ValidationScenarios.ExplicitRules.ValidateExplicitStringRules
{
    public class ValidateExplicitStringRulesCommand : IRequest, ICommand
    {
        public ValidateExplicitStringRulesCommand(string requiredText,
            string equalityText,
            string inequalityText,
            string minLengthText,
            string maxLengthText,
            string regexText,
            string emailText,
            string mustText,
            string customText)
        {
            RequiredText = requiredText;
            EqualityText = equalityText;
            InequalityText = inequalityText;
            MinLengthText = minLengthText;
            MaxLengthText = maxLengthText;
            RegexText = regexText;
            EmailText = emailText;
            MustText = mustText;
            CustomText = customText;
        }

        public string RequiredText { get; set; }
        public string EqualityText { get; set; }
        public string InequalityText { get; set; }
        public string MinLengthText { get; set; }
        public string MaxLengthText { get; set; }
        public string RegexText { get; set; }
        public string EmailText { get; set; }
        public string MustText { get; set; }
        public string CustomText { get; set; }
    }
}