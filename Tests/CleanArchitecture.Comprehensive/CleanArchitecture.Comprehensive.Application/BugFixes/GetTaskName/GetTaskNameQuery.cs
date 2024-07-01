using CleanArchitecture.Comprehensive.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryModels", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Application.BugFixes.GetTaskName
{
    public class GetTaskNameQuery : IRequest<TaskNameDto>, IQuery
    {
        public GetTaskNameQuery()
        {
        }
    }
}