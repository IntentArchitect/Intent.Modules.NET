using System;
using System.Collections.Generic;
using Entities.Constants.TestApplication.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryModels", Version = "1.0")]

namespace Entities.Constants.TestApplication.Application.TestClasses.GetTestClassById
{
    public class GetTestClassByIdQuery : IRequest<TestClassDto>, IQuery
    {
        public GetTestClassByIdQuery(Guid id)
        {
            Id = id;
        }
        public Guid Id { get; set; }

    }
}