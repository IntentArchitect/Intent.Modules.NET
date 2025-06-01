using System;
using System.Collections.Generic;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;
using JetBrains.Annotations;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.EntityFramework.Application.LinqExtensions.Templates.QueryableMarkerExtensions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class QueryableMarkerExtensionsTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.EntityFramework.Application.LinqExtensions.QueryableMarkerExtensions";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public QueryableMarkerExtensionsTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("System.Reflection")
                .AddUsing("System.Linq.Expressions")
                .AddClass($"QueryableMarkerExtensions", @class =>
                {
                    @class.Static();

                    AddSimpleLinqExtensionMarker(@class, "AsNoTracking");
                    AddSimpleLinqExtensionMarker(@class, "AsTracking");
                    @class.AddNestedClass("QueryBehaviorVisitor", nestedClass =>
                    {
                        nestedClass.WithBaseType("ExpressionVisitor");

                        nestedClass.AddProperty("bool", "NoTracking", p => p.WithInitialValue("false").Setter.Private());
                        nestedClass.AddProperty("bool", "AsTracking", p => p.WithInitialValue("false").Setter.Private());

                        nestedClass.AddMethod("Expression", "VisitMethodCall", method =>
                        {
                            method.Protected().Override();
                            method.AddParameter("MethodCallExpression", "node");
                            method.AddStatement(@$"            if (node.Method.DeclaringType == typeof({@class.Name}))
            {{
                switch (node.Method.Name)
                {{
                    case nameof({@class.Name}.AsNoTrackingImpl):
                        NoTracking = true;
                        return Visit(node.Arguments[0]);
                    case nameof({@class.Name}.AsTrackingImpl):
                        AsTracking = true;
                        return Visit(node.Arguments[0]);
                }}
            }}");
                            method.AddStatement("return base.VisitMethodCall(node);", c => c.SeparatedFromPrevious());
                        });
                    });
                });
        }

        private void AddSimpleLinqExtensionMarker(CSharpClass @class, string extensionName)
        {
            @class.AddField("MethodInfo", $"{extensionName}MethodInfo", f =>
            {
                f.PrivateReadOnly().Static().WithAssignment($"typeof({@class.Name}).GetMethod(nameof({extensionName}Impl), BindingFlags.NonPublic | BindingFlags.Static)!");
            });
            @class.AddMethod("IQueryable<T>", $"{extensionName}<T>", method =>
            {
                method.Static();
                method.AddParameter("IQueryable<T>", "source", p => p.WithThisModifier());
                method.AddStatement(@$"return source.Provider.CreateQuery<T>(
                Expression.Call(
                    method: {extensionName}MethodInfo.MakeGenericMethod(typeof(T)),
                    arguments: [source.Expression]
                ));");
            });
            @class.AddMethod("IQueryable<T>", $"{extensionName}Impl<T>", method =>
            {
                method.Private().Static();
                method.AddParameter("IQueryable<T>", "source", p => p.WithThisModifier());
                method.WithExpressionBody("source");
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