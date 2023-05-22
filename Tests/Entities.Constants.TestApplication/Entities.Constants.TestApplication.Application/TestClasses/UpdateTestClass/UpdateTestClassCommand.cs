using System;
using System.Collections.Generic;
using Entities.Constants.TestApplication.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace Entities.Constants.TestApplication.Application.TestClasses.UpdateTestClass
{
    public class UpdateTestClassCommand : IRequest, ICommand
    {
        public UpdateTestClassCommand(Guid id,
            string att100,
            string varChar200,
            string nVarChar300,
            string attMax,
            string varCharMax,
            string nVarCharMax)
        {
            Id = id;
            Att100 = att100;
            VarChar200 = varChar200;
            NVarChar300 = nVarChar300;
            AttMax = attMax;
            VarCharMax = varCharMax;
            NVarCharMax = nVarCharMax;
        }
        public Guid Id { get; set; }

        public string Att100 { get; set; }

        public string VarChar200 { get; set; }

        public string NVarChar300 { get; set; }

        public string AttMax { get; set; }

        public string VarCharMax { get; set; }

        public string NVarCharMax { get; set; }

    }
}