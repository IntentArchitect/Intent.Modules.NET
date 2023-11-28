using System;
using Intent.RoslynWeaver.Attributes;

[assembly: IntentTagModeImplicit]

namespace Standard.AspNetCore.TestApplication.Domain.Entities
{
    public class Client
    {
        public Guid Id { get; set; }
    }
}