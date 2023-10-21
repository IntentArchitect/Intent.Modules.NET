using System;
using Intent.RoslynWeaver.Attributes;

namespace EntityFrameworkCore.SqlServer.TestApplication.Domain.Entities.TPC.InheritanceAssociations
{
    [DefaultIntentManaged(Mode.Fully, Targets = Targets.Methods, Body = Mode.Ignore, AccessModifiers = AccessModifiers.Public)]
    public class TPC_DerivedClassForAbstract : TPC_AbstractBaseClass
    {
        public Guid Id { get; set; }

        public string DerivedAttribute { get; set; }
    }
}