using System;
using System.Collections.Generic;
using Entities.Constants.TestApplication.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryModels", Version = "1.0")]

namespace Entities.Constants.TestApplication.Application.TestClasses.TestValidation
{
    public class TestValidationQuery : IRequest<int>, IQuery
    {
        public string Att100 { get; set; }

        public string VarChar200 { get; set; }

        public string NVarChar300 { get; set; }

        public string AttMax { get; set; }

        public string VarCharMax { get; set; }

        public string NVarCharMax { get; set; }

    }
}