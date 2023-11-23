using Intent.RoslynWeaver.Attributes;

[assembly: IntentTagModeImplicit]

namespace EntityFrameworkCore.SqlServer.TestApplication.Domain.Entities.TPT.InheritanceAssociations
{
    [DefaultIntentManaged(Mode.Fully, Targets = Targets.Methods, Body = Mode.Ignore, AccessModifiers = AccessModifiers.Public)]
    public class TPT_DerivedClassForAbstract : TPT_AbstractBaseClass
    {
        public string DerivedAttribute { get; set; }
    }
}