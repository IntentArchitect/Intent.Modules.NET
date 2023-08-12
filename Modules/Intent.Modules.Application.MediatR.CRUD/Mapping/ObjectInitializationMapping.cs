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
    public class ObjectInitializationMapping : CSharpMappingBase
    {
        public ObjectInitializationMapping(ICanBeReferencedType model, IElementToElementMappingConnection mapping, IList<ICSharpMapping> children) : base(model, mapping, children)
        {
        }

        public override CSharpStatement GetFromStatement()
        {
            if (Mapping == null)
            {
                var init = (Model.TypeReference != null)
                    ? new CSharpObjectInitializerBlock($"new {Model.TypeReference.Element.Name.ToPascalCase()}")
                    : new CSharpObjectInitializerBlock($"new {Model.Name.ToPascalCase()}").WithSemicolon();

                SetToReplacement(Model, null);
                init.AddStatements(Children.Select(x => new CSharpObjectInitStatement(x.GetToStatement().GetText(""), x.GetFromStatement())));

                return init;
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

                    select.AddArgument(new CSharpLambdaBlock(variableName).WithExpressionBody(new CSharpObjectInitializerBlock($"new {Model.TypeReference.Element.Name.ToPascalCase()}")
                        .AddStatements(Children.Select(x => new CSharpObjectInitStatement(x.GetToStatement().GetText(""), x.GetFromStatement())))));

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
