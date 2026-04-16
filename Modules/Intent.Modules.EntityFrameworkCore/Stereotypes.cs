using System.Diagnostics.CodeAnalysis;

namespace Intent.Modules.EntityFrameworkCore
{
    [SuppressMessage("ReSharper", "MemberHidesStaticFromOuterClass")]
    internal static class Stereotypes
    {
        public static class EntityFrameworkCore
        {
            public static class EFMappingOptions
            {
                public const string Name = "EFMappingOptions";

                public static class Property
                {
                    public const string ColumnType = "ColumnType";
                }
            }

            public static class ConcurrencyToken
            {
                public const string Name = "ConcurrencyToken";
            }

            public static class RowVersion
            {
                public const string Name = "RowVersion";
            }
        }

        internal static class DomainConstraintStereotypes
        {
            public const string TextLimitsStereotypeId = "13649b19-4dfe-43ec-967f-0b85a5801dd6";
        }
    }
}
