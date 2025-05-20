using System;
using System.Collections.Generic;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.EntityFramework.Application.LinqExtensions.Templates.QueryableMarkerImplementation
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class QueryableMarkerImplementationTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.EntityFramework.Application.LinqExtensions.QueryableMarkerImplementation";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public QueryableMarkerImplementationTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("System.Linq.Expressions")
                .AddUsing("Microsoft.EntityFrameworkCore")
                .AddClass($"QueryableMarkerImplementation", @class =>
                {
                    @class.Static();

                    @class.AddMethod("IQueryable<T>", "ApplyMarkers", method =>
                    {
                        method
                            .AddGenericParameter("T", out var tType)
                            .AddGenericTypeConstraint(tType, c => c
                                .AddType("class"));
                        method.Static();
                        method.AddParameter($"IQueryable<{tType}>", "queryable", p => p.WithThisModifier());

                        method.AddStatement($@"var visitor = new {this.GetQueryableMarkerExtensionsName()}.QueryBehaviorVisitor();
            var visitedExpr = visitor.Visit(queryable.Expression);
            var visited = (IQueryable<T>)visitedExpr.ToQueryable(queryable.Provider);

            if (visitor.NoTracking)
            {{
                visited = EntityFrameworkQueryableExtensions.AsNoTracking(visited);
            }}

            if (visitor.AsTracking)
            {{
                visited = EntityFrameworkQueryableExtensions.AsTracking(visited);
            }}");

                        method.AddStatement("return visited;", o => o.SeparatedFromPrevious());
                    });


                    @class.AddMethod("IQueryable", "ToQueryable", method => 
                    {
                        method.Static();
                        method.AddParameter("Expression", "expression", p => p.WithThisModifier());
                        method.AddParameter("IQueryProvider", "provider");
                        method.AddStatement("return provider.CreateQuery(expression);");
                    });
                });
        }

        [IntentManaged(Mode.Fully)]
        public CSharpFile CSharpFile { get; }

        [IntentManaged(Mode.Fully)]
        protected override CSharpFileConfig DefineFileConfig()
        {
            return CSharpFile.GetConfig();
        }

        [IntentManaged(Mode.Fully)]
        public override string TransformText()
        {
            return CSharpFile.ToString();
        }
    }
}