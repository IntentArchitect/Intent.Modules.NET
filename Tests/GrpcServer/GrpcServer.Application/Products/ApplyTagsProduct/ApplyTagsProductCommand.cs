using System;
using System.Collections.Generic;
using GrpcServer.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace GrpcServer.Application.Products.ApplyTagsProduct
{
    public class ApplyTagsProductCommand : IRequest, ICommand
    {
        public ApplyTagsProductCommand(Guid id, List<string> tagNames)
        {
            Id = id;
            TagNames = tagNames;
        }

        public Guid Id { get; set; }
        public List<string> TagNames { get; set; }
    }
}