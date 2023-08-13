using Intent.Metadata.Models;
using Intent.Modules.Common.CSharp.Builder;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;

namespace Intent.Modules.Application.MediatR.CRUD.Mapping
{
    public class ClassConstructionMapping : CSharpMappingBase
    {
        private readonly ICSharpFileBuilderTemplate _template;

        public ClassConstructionMapping(ICanBeReferencedType model, IElementToElementMappingConnection mapping, IList<ICSharpMapping> children, ICSharpFileBuilderTemplate template) : base(model, mapping, children)
        {
            _template = template;
        }
    }

    public class ObjectInitializationMapping : CSharpMappingBase
    {
        private readonly ICSharpFileBuilderTemplate _template;

        public ObjectInitializationMapping(ICanBeReferencedType model, IElementToElementMappingConnection mapping, IList<ICSharpMapping> children, ICSharpFileBuilderTemplate template) : base(model, mapping, children)
        {
            _template = template;
        }

        public override CSharpStatement GetFromStatement()
        {
            if (Mapping == null)
            {
                SetToReplacement(Model, null);
                return GetConstructorStatement();
            }
            else
            {
                if (Children.Count == 0)
                {
                    return $"{GetFromPath(_fromReplacements)}";
                }
                if (Model.TypeReference.IsCollection)
                {
                    var chain = new CSharpMethodChainStatement($"{GetFromPath(_fromReplacements)}{(Mapping.FromPath.Last().Element.TypeReference.IsNullable ? "?" : "")}").WithoutSemicolon();
                    var select = new CSharpInvocationStatement($"Select").WithoutSemicolon();

                    var variableName = string.Join("", Model.Name.Where(char.IsUpper).Select(char.ToLower));
                    SetFromReplacement(Mapping.FromPath.Skip(_fromReplacements.Count).First().Element, variableName);
                    SetToReplacement(Mapping.ToPath.Skip(_toReplacements.Count).First().Element, null);

                    select.AddArgument(new CSharpLambdaBlock(variableName).WithExpressionBody(GetConstructorStatement()));

                    var init = chain
                        .AddChainStatement(select)
                        .AddChainStatement("ToList()");
                    return init;
                }
                else
                {
                    return GetFromPath(_fromReplacements);
                }
            }
        }

        private CSharpStatement GetConstructorStatement()
        {
            var ctor = Children.SingleOrDefault(x => x is ImplicitConstructorMapping && x.Model.TypeReference == null);
            if (ctor != null)
            {
                var children = Children.Where(x => x is not ImplicitConstructorMapping || x.Model.TypeReference != null).ToList();
                if (!children.Any())
                {
                    return ctor.GetFromStatement();
                }

                var init = new CSharpObjectInitializerBlock(ctor.GetFromStatement().GetText(""));
                init.AddStatements(children.OrderBy(x => ((IElement)x.Model).Order).Select(x => new CSharpObjectInitStatement(x.GetToStatement().GetText(""), x.GetFromStatement())));
                return init;
            }
            else
            {
                var init = (Model.TypeReference != null)
                    ? new CSharpObjectInitializerBlock($"new {_template.GetTypeName((IElement)Model.TypeReference.Element)}")
                    : new CSharpObjectInitializerBlock($"new {_template.GetTypeName((IElement)Model)}");
                init.AddStatements(Children.OrderBy(x => ((IElement)x.Model).Order).Select(x => new CSharpObjectInitStatement(x.GetToStatement().GetText(""), x.GetFromStatement())));
                return init;
            }
        }

        public override CSharpStatement GetToStatement()
        {
            return Model.Name.ToPascalCase();
        }

        //public override IEnumerable<CSharpStatement> GetMappingStatement(IDictionary<ICanBeReferencedType, string> fromReplacements, IDictionary<ICanBeReferencedType, string> toReplacements)
        //{
        //    fromReplacements ??= _fromReplacements;
        //    toReplacements ??= _toReplacements;
        //    if (Mapping == null)
        //    {
        //        var init = (Model.TypeReference != null)
        //            ? new CSharpObjectInitializerBlock($"new {Model.TypeReference.Element.Name.ToPascalCase()}")
        //            : new CSharpObjectInitializerBlock($"new {Model.Name.ToPascalCase()}").WithSemicolon();

        //        AddToReplacement(Model, null);
        //        init.AddStatements(Children.SelectMany(x => x.GetMappingStatement(_fromReplacements, _toReplacements)));

        //        if (Model.TypeReference != null)
        //        {
        //            yield return new CSharpAssignmentStatement(Model.Name.ToPascalCase(), init);
        //            yield break;
        //        }
        //        yield return init;
        //    }
        //    else
        //    {
        //        if (Children.Count == 0)
        //        {
        //            yield return new CSharpObjectInitStatement(Model.Name.ToPascalCase(), $"{GetFromPath(fromReplacements)}");
        //            yield break;
        //        }
        //        if (Model.TypeReference.IsCollection)
        //        {
        //            var chain = new CSharpMethodChainStatement($"{GetFromPath(fromReplacements)}{(Mapping.FromPath.Last().Element.TypeReference.IsNullable ? "?" : "")}").WithoutSemicolon();
        //            var select = new CSharpInvocationStatement($"Select").WithoutSemicolon();

        //            var variableName = string.Join("", Model.Name.Where(char.IsUpper).Select(char.ToLower));
        //            AddFromReplacement(Mapping.FromPath.Skip(fromReplacements.Count).First().Element, variableName);
        //            AddToReplacement(Mapping.ToPath.Skip(toReplacements.Count).First().Element, null);

        //            select.AddArgument(new CSharpLambdaBlock(variableName).WithExpressionBody(new CSharpObjectInitializerBlock($"new {Model.TypeReference.Element.Name.ToPascalCase()}")
        //                .AddStatements(Children.SelectMany(x => x.GetMappingStatement(_fromReplacements, _toReplacements)))));

        //            var init = chain
        //                .AddChainStatement(select)
        //                .AddChainStatement("ToList()");
        //            yield return new CSharpObjectInitStatement(Model.Name.ToPascalCase(), init);
        //        }
        //        else
        //        {
        //            yield return new CSharpObjectInitStatement(Model.Name.ToPascalCase(), $"{GetFromPath(fromReplacements)}");
        //        }
        //    }
        //}
    }
}
