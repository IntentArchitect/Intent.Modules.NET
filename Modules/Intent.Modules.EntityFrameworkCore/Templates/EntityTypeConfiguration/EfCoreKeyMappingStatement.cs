using System.Linq;
using Intent.Metadata.RDBMS.Api;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.Templates;

namespace Intent.Modules.EntityFrameworkCore.Templates.EntityTypeConfiguration;

public class EfCoreKeyMappingStatement : CSharpStatement
{
    public RequiredEntityProperty[] KeyColumns { get; set; }

    private readonly bool _isOwnedEntityNoPrimaryKey = false;

    public EfCoreKeyMappingStatement(ClassModel model, RelationshipType? ownedRelationship) : base(null)
    {
        KeyColumns = model.GetExplicitPrimaryKey()
            .Select(x => new RequiredEntityProperty(Class: model.InternalElement, Name: x.Name.ToPascalCase(), Type: x.Type.Element))
            .ToArray();

        // determine if the model is an owned entity, with no primary key
        _isOwnedEntityNoPrimaryKey = !(model.IsAggregateRoot() || model.GetExplicitPrimaryKey().Any() || (ownedRelationship != RelationshipType.OneToOne && ownedRelationship != RelationshipType.OneToMany));
    }

    public override string GetText(string indentation)
    {
        // if it is an owned entity, no PK - then don't add "HasNoKey"
        // as this is now valid on OwnedNavigationBuilder
        if (_isOwnedEntityNoPrimaryKey)
        {
            return string.Empty;
        }

        var method = KeyColumns.Length switch
        {
            0 => "HasNoKey()",
            1 => $"HasKey(x => x.{KeyColumns[0].Name})",
            _ => $"HasKey(x => new {{ {string.Join(", ", KeyColumns.Select(key => $"x.{key.Name}"))} }})"
        };
        
        return $"{indentation}builder.{method};";
    }
}