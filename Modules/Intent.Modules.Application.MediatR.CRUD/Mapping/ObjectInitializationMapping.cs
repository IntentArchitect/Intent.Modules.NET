using Intent.Metadata.Models;
using Intent.Modules.Common.CSharp.Builder;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Intent.Modules.Common.Templates;

namespace Intent.Modules.Application.MediatR.CRUD.Mapping
{
    public class ObjectInitializationMapping : ICSharpMapping
    {
        public ICanBeReferencedType Model { get; }
        public IList<ICSharpMapping> Children { get; }
        public IElementToElementMappingConnection Mapping { get; set; }

        public ObjectInitializationMapping(ICanBeReferencedType model, IElementToElementMappingConnection mapping, IList<ICSharpMapping> children)
        {
            Model = model;
            Mapping = mapping;
            Children = children;
        }

        public virtual CSharpStatement GetFromMappingStatement(IDictionary<ICanBeReferencedType, string> fromReplacements)
        {
            if (Mapping == null)
            {
                var init = (Model.TypeReference != null)
                    ? new CSharpObjectInitializerBlock($"new {Model.TypeReference.Element.Name.ToPascalCase()}")
                    : new CSharpObjectInitializerBlock($"new {Model.Name.ToPascalCase()}").WithSemicolon();

                init.AddStatements(Children.Select(x => new CSharpObjectInitStatement(x.Model.Name.ToPascalCase(), x.GetFromMappingStatement(fromReplacements))));

                return init;
            }
            else
            {
                if (Children.Count == 0)
                {
                    return $"{GetFromPath(Mapping.FromPath, fromReplacements)}";
                }
                if (Model.TypeReference.IsCollection)
                {
                    var chain = new CSharpMethodChainStatement($"{GetFromPath(Mapping.FromPath, fromReplacements)}{(Mapping.FromPath.Last().Element.TypeReference.IsNullable ? "?" : "")}").WithoutSemicolon();
                    var select = new CSharpInvocationStatement($"Select").WithoutSemicolon();

                    var variableName = string.Join("", Model.Name.Where(char.IsUpper).Select(char.ToLower));
                    fromReplacements = fromReplacements.Concat(new[] { new KeyValuePair<ICanBeReferencedType, string>(Mapping.FromPath.Skip(fromReplacements.Count).First().Element, variableName) }).ToDictionary(x => x.Key, x => x.Value);
                    select.AddArgument(new CSharpLambdaBlock(variableName).WithExpressionBody(new CSharpObjectInitializerBlock($"new {Model.TypeReference.Element.Name.ToPascalCase()}")
                        .AddStatements(Children.Select(x => new CSharpObjectInitStatement(x.Model.Name.ToPascalCase(), x.GetFromMappingStatement(fromReplacements))))));

                    var init = new CSharpObjectInitStatement($"{Model.Name.ToPascalCase()}", chain
                        .AddChainStatement(select)
                        .AddChainStatement("ToList()"));
                    return init;
                }
                else
                {
                    return $"{GetFromPath(Mapping.FromPath, fromReplacements)}";
                }

            }
        }

        protected string GetFromPath(IList<IElementMappingPathTarget> mappingFromPath, IDictionary<ICanBeReferencedType, string> fromReplacements)
        {
            var result = "";
            foreach (var mappingPathTarget in mappingFromPath)
            {
                if (fromReplacements.ContainsKey(mappingPathTarget.Element))
                {
                    result = fromReplacements[mappingPathTarget.Element];
                }
                else
                {
                    result += $".{mappingPathTarget.Element.Name.ToPascalCase()}";
                    if (mappingPathTarget.Element.TypeReference.IsNullable && mappingFromPath.Last() != mappingPathTarget)
                    {
                        result += "?";
                    }
                }
            }
            return result;
        }
    }
}
