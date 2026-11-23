using Intent.Modules.Common.DriftEngine.Code;

namespace Intent.Modules.Common.DriftEngine.Stacks.EfCore;

/// <summary>
/// ASP.NET Core Identity's entity bases, whose code is a package's, so the graph doesn't read their members: an
/// <c>ApplicationUser : IdentityUser</c> has <c>Id</c>, <c>UserName</c>, <c>Email</c> and the rest as its own inherited
/// members, as EF Core maps them. <c>IdentityUser</c> is <c>IdentityUser&lt;string&gt;</c>, and <c>IdentityRole</c>
/// <c>IdentityRole&lt;string&gt;</c>.
/// </summary>
internal static class IdentityTypes
{
    private static readonly Span Library = new("", 0);

    public static void AddTo(CodeGraph graph)
    {
        graph.AddLibraryType("IdentityUser", ["TKey"], [Type("string", nullable: false)],
        [
            Property("Id", Key()),
            Property("UserName", Type("string", nullable: true)),
            Property("NormalizedUserName", Type("string", nullable: true)),
            Property("Email", Type("string", nullable: true)),
            Property("NormalizedEmail", Type("string", nullable: true)),
            Property("EmailConfirmed", Type("bool", nullable: false)),
            Property("PasswordHash", Type("string", nullable: true)),
            Property("SecurityStamp", Type("string", nullable: true)),
            Property("ConcurrencyStamp", Type("string", nullable: true)),
            Property("PhoneNumber", Type("string", nullable: true)),
            Property("PhoneNumberConfirmed", Type("bool", nullable: false)),
            Property("TwoFactorEnabled", Type("bool", nullable: false)),
            Property("LockoutEnd", Type("DateTimeOffset", nullable: true)),
            Property("LockoutEnabled", Type("bool", nullable: false)),
            Property("AccessFailedCount", Type("int", nullable: false))
        ]);

        graph.AddLibraryType("IdentityRole", ["TKey"], [Type("string", nullable: false)],
        [
            Property("Id", Key()),
            Property("Name", Type("string", nullable: true)),
            Property("NormalizedName", Type("string", nullable: true)),
            Property("ConcurrencyStamp", Type("string", nullable: true))
        ]);
    }

    /// <summary>The key, typed by the type argument <c>TKey</c>.</summary>
    private static TypeRefNode Key() => new() { Text = "TKey", Name = "TKey", QualifiedName = "TKey" };

    private static TypeRefNode Type(string name, bool nullable) => new()
    {
        Text = nullable ? name + "?" : name, Name = name, QualifiedName = name, NullableMarked = nullable, Nullable = nullable ? "true" : "false",
        IsPredefinedValueType = name is "bool" or "int"
    };

    private static MemberNode Property(string name, TypeRefNode type) => new() { Name = name, Kind = "property", Type = type, At = Library };
}
