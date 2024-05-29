using System;
using CleanArchitecture.Comprehensive.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryModels", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Application.ClassWithEnums.GetClassWithEnumsById
{
    public class GetClassWithEnumsByIdQuery : IRequest<ClassWithEnumsDto>, IQuery
    {
        public GetClassWithEnumsByIdQuery(Guid id)
        {
            Id = id;
        }

        public Guid Id { get; set; }
    }
}