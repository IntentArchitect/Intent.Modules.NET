using System;
using System.Collections.Generic;
using Entities.Constants.TestApplication.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryModels", Version = "1.0")]

namespace Entities.Constants.TestApplication.Application.TestClasses.GetTestClasses
{
    public class GetTestClassesQuery : IRequest<List<TestClassDto>>, IQuery
    {
    }
}