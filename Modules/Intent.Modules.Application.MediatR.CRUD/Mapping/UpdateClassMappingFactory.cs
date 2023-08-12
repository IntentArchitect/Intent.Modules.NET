using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.TypeResolution;

namespace Intent.Modules.Application.MediatR.CRUD.Mapping;

public class UpdateClassMappingFactory : MappingFactoryBase
{
    private readonly ICSharpFileBuilderTemplate _template;

    public UpdateClassMappingFactory(IDictionary<string, Func<ICanBeReferencedType, IElementToElementMappingConnection, IList<ICSharpMapping>, ICSharpMapping>> factoryRegistry, ICSharpFileBuilderTemplate template) : base(factoryRegistry)
    {
        _template = template;
    }

    public override ICSharpMapping CreateMappingType(ICanBeReferencedType model, IElementToElementMappingConnection mapping, List<ICSharpMapping> children)
    {
        if (model.TypeReference?.Element.SpecializationType == "Value Object")
        {
            return new ImplicitConstructorMapping(model, mapping, children);
        }
        return new ObjectUpdateMapping(model, mapping, children, _template);
    }
}

public class ObjectUpdateMapping : CSharpMappingBase
{
    private readonly ICSharpFileBuilderTemplate _template;

    public ObjectUpdateMapping(ICanBeReferencedType model, IElementToElementMappingConnection mapping, IList<ICSharpMapping> children, ICSharpFileBuilderTemplate template) : base(model, mapping, children)
    {
        _template = template;
    }

    public override IEnumerable<CSharpStatement> GetMappingStatement(IDictionary<ICanBeReferencedType, string> fromReplacements, IDictionary<ICanBeReferencedType, string> toReplacements)
    {
        if (Mapping == null) // is traversal
        {
          
            if (Model.TypeReference == null)
            {
                foreach (var statement in Children.SelectMany(x => x.GetMappingStatement(fromReplacements, toReplacements)))
                {
                    yield return statement.WithSemicolon();
                }
            }
            else
            {
                //var init = (Model.TypeReference != null)
                //    ? new CSharpObjectInitializerBlock($"new {Model.TypeReference.Element.Name.ToPascalCase()}")
                //    : new CSharpObjectInitializerBlock($"new {Model.Name.ToPascalCase()}").WithSemicolon();

                //init.AddStatements(Children.SelectMany(x => x.GetMappingStatement(fromReplacements)));

                //yield return init;
            }
        }
        else
        {
            if (Children.Count == 0)
            {
                yield return new CSharpAssignmentStatement($"{GetToPath(toReplacements)}", $"{GetFromPath(fromReplacements)}").WithSemicolon();
            } 
            else if (Model.TypeReference.IsCollection)
            {
                yield return new CSharpAssignmentStatement($"{GetToPath(toReplacements)}", $"UpdateHelper.CreateOrUpdateCollection({GetToPath(toReplacements)}, {GetFromPath(fromReplacements)}, (e, d) => e.Id == d.Id, {$"CreateOrUpdate{Mapping.ToPath.Last().Element.Name.ToPascalCase()}"})").WithSemicolon();

                _template.CSharpFile.AfterBuild(file =>
                {
                    file.Classes.First().AddMethod(_template.GetTypeName((IElement)Model.TypeReference.Element), $"CreateOrUpdate{Mapping.ToPath.Last().Element.Name.ToPascalCase()}", method =>
                    {
                        
                        method.Private().Static();
                        method.AddParameter(_template.GetTypeName(Model.TypeReference.Element.AsTypeReference(true, false)), "entity");
                        method.AddParameter(_template.GetTypeName((IElement)Mapping.FromPath.Last().Element.TypeReference.Element), "dto");
                        method.AddStatement($"entity ??= new {_template.GetTypeName((IElement)Model.TypeReference.Element)}();");

                        fromReplacements = fromReplacements.Concat(new[] { new KeyValuePair<ICanBeReferencedType, string>(Mapping.FromPath.Skip(fromReplacements.Count).First().Element, "dto") }).ToDictionary(x => x.Key, x => x.Value);
                        toReplacements = toReplacements.Concat(new[] { new KeyValuePair<ICanBeReferencedType, string>(Mapping.ToPath.Skip(toReplacements.Count).First().Element, "entity") }).ToDictionary(x => x.Key, x => x.Value);
                        method.AddStatements(Children.SelectMany(x => x.GetMappingStatement(fromReplacements, toReplacements).Select(st => st.WithSemicolon())));

                        method.AddStatement("return entity;");
                    });
                });
            }
        }
    }
}