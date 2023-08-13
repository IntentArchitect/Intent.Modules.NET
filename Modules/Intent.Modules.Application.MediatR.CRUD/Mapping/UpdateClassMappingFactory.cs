using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common;
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
        if (model.SpecializationType == "Operation")
        {
            return new MethodInvocationMapping(model, mapping, children, _template);
        }

        if (model.TypeReference?.Element.SpecializationType == "Value Object")
        {
            return new ImplicitConstructorMapping(model, mapping, children, _template);
        }
        return new ObjectUpdateMapping(model, mapping, children.Where(x => !x.Model.HasStereotype("Primary Key")).ToList(), _template);
    }
}

public class ObjectUpdateMapping : CSharpMappingBase
{
    private readonly ICSharpFileBuilderTemplate _template;

    public ObjectUpdateMapping(ICanBeReferencedType model, IElementToElementMappingConnection mapping, IList<ICSharpMapping> children, ICSharpFileBuilderTemplate template) : base(model, mapping, children)
    {
        _template = template;
    }

    public override CSharpStatement GetFromStatement()
    {
        if (Mapping == null) // is traversal
        {
            return GetPath(GetFromPath(), _fromReplacements);
        }
        else
        {
            if (Children.Count == 0)
            {
                return $"{GetFromPath(_fromReplacements)}";
            }
            else if (Model.TypeReference.IsCollection)
            {
                var from = $"UpdateHelper.CreateOrUpdateCollection({GetToPath(_toReplacements)}, {GetFromPath(_fromReplacements)}, (e, d) => e.Id == d.Id, CreateOrUpdate{Model.TypeReference.Element.Name.ToPascalCase()})";

                CreateUpdateMethod($"CreateOrUpdate{Model.TypeReference.Element.Name.ToPascalCase()}");
                //_template.CSharpFile.AfterBuild(file =>
                //{
                //    file.Classes.First().AddMethod(_template.GetTypeName((IElement)Model.TypeReference.Element), $"CreateOrUpdate{Mapping.ToPath.Last().Element.Name.ToPascalCase()}", method =>
                //    {

                //        method.Private().Static();
                //        method.AddParameter(_template.GetTypeName(Model.TypeReference.Element.AsTypeReference(true, false)), "entity");
                //        method.AddParameter(_template.GetTypeName((IElement)Mapping.FromPath.Last().Element.TypeReference.Element), "dto");
                //        method.AddStatement($"entity ??= new {_template.GetTypeName((IElement)Model.TypeReference.Element)}();");

                //        AddFromReplacement(Mapping.FromPath.Skip(_fromReplacements.Count).First().Element, "dto");
                //        AddToReplacement(Mapping.ToPath.Skip(_toReplacements.Count).First().Element, "entity");
                //        method.AddStatements(Children.SelectMany(x => x.GetMappingStatement(null, null)).Select(x => x.WithSemicolon()));

                //        method.AddStatement("return entity;");
                //    });
                //});

                return from;
            }
        }

        return null;
    }

    public override IEnumerable<CSharpStatement> GetMappingStatement()
    {
        if (Mapping == null) // is traversal
        {
          
            if (Model.TypeReference == null)
            {
                foreach (var statement in Children.SelectMany(x => x.GetMappingStatement()))
                {
                    yield return statement.WithSemicolon();
                }
            }
            else
            {
                yield return new CSharpAssignmentStatement(GetToStatement(), $"CreateOrUpdate{Model.TypeReference.Element.Name.ToPascalCase()}({GetToStatement()}, {GetFromStatement()})");

                CreateUpdateMethod($"CreateOrUpdate{Model.TypeReference.Element.Name.ToPascalCase()}");
            }
        }
        else
        {
            yield return new CSharpAssignmentStatement(GetToStatement(), GetFromStatement());
            //if (Children.Count == 0)
            //{
            //    yield return new CSharpAssignmentStatement($"{GetToPath(toReplacements)}", $"{GetFromPath(fromReplacements)}").WithSemicolon();
            //} 
            //else if (Model.TypeReference.IsCollection)
            //{
            //    yield return new CSharpAssignmentStatement($"{GetToPath(toReplacements)}", $"UpdateHelper.CreateOrUpdateCollection({GetToPath(toReplacements)}, {GetFromPath(fromReplacements)}, (e, d) => e.Id == d.Id, {$"CreateOrUpdate{Mapping.ToPath.Last().Element.Name.ToPascalCase()}"})").WithSemicolon();

            //    _template.CSharpFile.AfterBuild(file =>
            //    {
            //        file.Classes.First().AddMethod(_template.GetTypeName((IElement)Model.TypeReference.Element), $"CreateOrUpdate{Mapping.ToPath.Last().Element.Name.ToPascalCase()}", method =>
            //        {

            //            method.Private().Static();
            //            method.AddParameter(_template.GetTypeName(Model.TypeReference.Element.AsTypeReference(true, false)), "entity");
            //            method.AddParameter(_template.GetTypeName((IElement)Mapping.FromPath.Last().Element.TypeReference.Element), "dto");
            //            method.AddStatement($"entity ??= new {_template.GetTypeName((IElement)Model.TypeReference.Element)}();");

            //            fromReplacements = fromReplacements.Concat(new[] { new KeyValuePair<ICanBeReferencedType, string>(Mapping.FromPath.Skip(fromReplacements.Count).First().Element, "dto") }).ToDictionary(x => x.Key, x => x.Value);
            //            toReplacements = toReplacements.Concat(new[] { new KeyValuePair<ICanBeReferencedType, string>(Mapping.ToPath.Skip(toReplacements.Count).First().Element, "entity") }).ToDictionary(x => x.Key, x => x.Value);
            //            method.AddStatements(Children.Select(x => new CSharpAssignmentStatement(x.GetToStatement(), x.GetFromStatement())));

            //            method.AddStatement("return entity;");
            //        });
            //    });
            //}
        }
    }

    private void CreateUpdateMethod(string updateMethodName)
    {
        var domainTypeName = _template.GetTypeName((IElement)Model.TypeReference.Element);
        var fromField = GetFromPath().Last().Element;
        var fieldIsNullable = fromField.TypeReference.IsNullable;

        var @class = _template.CSharpFile.Classes.First();
        var existingMethod = @class.FindMethod(x => x.Name == updateMethodName &&
                                                    x.ReturnType == domainTypeName &&
                                                    x.Parameters.FirstOrDefault()?.Type == domainTypeName &&
                                                    x.Parameters.Skip(1).FirstOrDefault()?.Type == _template.GetTypeName((IElement)fromField.TypeReference.Element));
        if (existingMethod != null)
        {
            return;
        }
        _template.CSharpFile.AfterBuild(file =>
        {
            file.Classes.First().AddMethod(domainTypeName, updateMethodName, method =>
            {
                method.Private().Static();
                method.AddParameter(_template.GetTypeName(Model.TypeReference.Element.AsTypeReference(true, false)), "entity");
                method.AddParameter(_template.GetTypeName((IElement)GetFromPath().Last().Element.TypeReference.Element), "dto");

                if (fieldIsNullable)
                {
                    method.AddIfStatement("dto == null", s => s
                        .AddStatement("return null;"));
                }

                method.AddStatement($"entity ??= new {_template.GetTypeName((IElement)Model.TypeReference.Element)}();");

                SetFromReplacement(GetFromPath().Last().Element, "dto");
                SetToReplacement(GetToPath().Last().Element, "entity");
                method.AddStatements(Children.SelectMany(x => x.GetMappingStatement()).Select(x => x.WithSemicolon()));

                method.AddStatement("return entity;");
            });
        });
    }
}