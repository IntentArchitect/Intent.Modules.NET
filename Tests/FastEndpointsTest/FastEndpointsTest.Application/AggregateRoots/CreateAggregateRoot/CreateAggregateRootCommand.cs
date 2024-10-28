using System;
using System.Collections.Generic;
using FastEndpointsTest.Application.Common.Interfaces;
using FastEndpointsTest.Domain.Enums;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace FastEndpointsTest.Application.AggregateRoots.CreateAggregateRoot
{
    public class CreateAggregateRootCommand : IRequest<Guid>, ICommand
    {
        public CreateAggregateRootCommand(string aggregateAttr,
            string limitedDomain,
            string limitedService,
            EnumWithoutValues enumType1,
            EnumWithDefaultLiteral enumType2,
            EnumWithoutDefaultLiteral enumType3,
            Guid? aggregateId,
            List<CreateAggregateRootCommandCompositesDto3> composites)
        {
            AggregateAttr = aggregateAttr;
            LimitedDomain = limitedDomain;
            LimitedService = limitedService;
            EnumType1 = enumType1;
            EnumType2 = enumType2;
            EnumType3 = enumType3;
            AggregateId = aggregateId;
            Composites = composites;
        }

        public string AggregateAttr { get; set; }
        public string LimitedDomain { get; set; }
        public string LimitedService { get; set; }
        public EnumWithoutValues EnumType1 { get; set; }
        public EnumWithDefaultLiteral EnumType2 { get; set; }
        public EnumWithoutDefaultLiteral EnumType3 { get; set; }
        public Guid? AggregateId { get; set; }
        public List<CreateAggregateRootCommandCompositesDto3> Composites { get; set; }
    }
}