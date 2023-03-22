using System;
using System.Collections.Generic;
using CleanArchitecture.TestApplication.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace CleanArchitecture.TestApplication.Application.VariantTypesClasses.UpdateVariantTypesClass
{
    public class UpdateVariantTypesClassCommand : IRequest, ICommand
    {
        public Guid Id { get; set; }

        public IEnumerable<string> StrCollection { get; set; }

        public IEnumerable<int> IntCollection { get; set; }

        public IEnumerable<string>? StrNullCollection { get; set; }

        public IEnumerable<int>? IntNullCollection { get; set; }

        public string? NullStr { get; set; }

        public int? NullInt { get; set; }

    }
}