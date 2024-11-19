using System;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace CleanArchitecture.Comprehensive.Application.RequestSuffixQueriesWithType.MyRequest
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class MyRequestQueryValidator : AbstractValidator<MyRequestQuery>
    {
        [IntentManaged(Mode.Fully, Body = Mode.Merge, Signature = Mode.Merge)]
        public MyRequestQueryValidator()
        {

        }
    }
}