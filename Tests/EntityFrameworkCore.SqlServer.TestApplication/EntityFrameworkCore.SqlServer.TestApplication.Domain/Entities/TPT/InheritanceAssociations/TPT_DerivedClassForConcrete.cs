using Intent.RoslynWeaver.Attributes;

namespace EntityFrameworkCore.SqlServer.TestApplication.Domain.Entities.TPT.InheritanceAssociations
{
    [DefaultIntentManaged(Mode.Fully, Targets = Targets.Methods, Body = Mode.Ignore, AccessModifiers = AccessModifiers.Public)]
    public class TPT_DerivedClassForConcrete : TPT_ConcreteBaseClass
    {
        public string DerivedAttribute { get; set; }
    }
}