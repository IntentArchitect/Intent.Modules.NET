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
    }
}
