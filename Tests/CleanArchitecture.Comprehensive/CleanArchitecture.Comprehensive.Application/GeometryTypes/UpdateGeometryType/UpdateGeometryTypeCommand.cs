using System;
using CleanArchitecture.Comprehensive.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using NetTopologySuite.Geometries;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Application.GeometryTypes.UpdateGeometryType
{
    public class UpdateGeometryTypeCommand : IRequest, ICommand
    {
        public UpdateGeometryTypeCommand(Point point, Guid id)
        {
            Point = point;
            Id = id;
        }

        public Point Point { get; set; }
        public Guid Id { get; set; }
    }
}