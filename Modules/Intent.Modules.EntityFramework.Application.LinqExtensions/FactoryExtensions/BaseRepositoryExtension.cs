using System;
using System.Linq;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Plugins;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.EntityFramework.Application.LinqExtensions.FactoryExtensions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class BaseRepositoryExtension : FactoryExtensionBase
    {
        public override string Id => "Intent.EntityFramework.Application.LinqExtensions.BaseRepositoryExtension";

        [IntentManaged(Mode.Ignore)]
        public override int Order => 0;

        protected override void OnAfterTemplateRegistrations(IApplication application)
        {
            base.OnAfterTemplateRegistrations(application);
            var baseEFRepositoryTemplate = application.FindTemplateInstance<ICSharpFileBuilderTemplate>("Intent.EntityFrameworkCore.Repositories.RepositoryBase");
            if (baseEFRepositoryTemplate == null)
                return;

            AppendEFBaseRepository(application, baseEFRepositoryTemplate);
        }

        private void AppendEFBaseRepository(IApplication application, ICSharpFileBuilderTemplate baseEFRepositoryTemplate)
        {
            baseEFRepositoryTemplate.CSharpFile.AfterBuild(file =>
            {
                var @class = file.Classes.First();
                var queryInternal = @class.FindMethod(m => m.Name == "QueryInternal" && m.Parameters.Count == 1 && m.Parameters[0].Name == "queryOptions");

                var statement = queryInternal.FindStatement(s => s.GetText("") == "queryable = queryOptions(queryable);");
                statement.InsertBelow(new CSharpStatement("queryable = queryable.ApplyMarkers();"));

                queryInternal = @class.FindMethod(m => m.Name == "QueryInternal" && m.Parameters.Count == 2 && m.Parameters[1].Name == "queryOptions");
                statement = queryInternal.FindStatement(s => s.GetText("") == "var result = queryOptions(queryable);");
                statement.InsertBelow(new CSharpStatement("result = result.ApplyMarkers();"));
                var tParam = queryInternal.GenericParameters.First();
                //Need to ensure this constraint is in place for AsTracking
                if (!queryInternal.GenericTypeConstraints.Any(gc => gc.GenericTypeParameter == tParam.TypeName && gc.Types.Contains("class")))
                {
                    queryInternal.AddGenericTypeConstraint(tParam, c => c
                        .AddType("class"));
                }

            }, 1000);
        }
    }
}