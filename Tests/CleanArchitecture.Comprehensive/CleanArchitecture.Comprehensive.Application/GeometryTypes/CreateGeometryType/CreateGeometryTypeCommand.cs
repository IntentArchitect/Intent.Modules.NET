using System;
using CleanArchitecture.Comprehensive.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using NetTopologySuite.Geometries;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Application.GeometryTypes.CreateGeometryType
{
    public class CreateGeometryTypeCommand : IRequest<Guid>, ICommand
    {
        public CreateGeometryTypeCommand(Point point)
        {
            Point = point;
        }

        public Point Point { get; set; }
    }
}