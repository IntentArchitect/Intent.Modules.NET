using System;
using System.Linq;
using System.Threading;
using Intent.Engine;
using Intent.Modelers.Services.Api;
using Intent.Modules.AspNetCore.ODataQuery.Settings;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.CSharp.VisualStudio;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Constants;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;
using Intent.Utils;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.AspNetCore.ODataQuery.FactoryExtensions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class CosmosRepositoryExtension : FactoryExtensionBase
    {
        public override string Id => "Intent.AspNetCore.ODataQuery.CosmosRepositoryExtension";

        [IntentManaged(Mode.Ignore)]
        public override int Order => 0;

        protected override void OnAfterTemplateRegistrations(IApplication application)
        {
            if (!application.InstalledModules.Any(p => p.ModuleId == "Intent.CosmosDB"))
            {
                return;
            }

            base.OnAfterTemplateRegistrations(application);
            var baseEFRepositoryTemplate = application.FindTemplateInstance<ICSharpFileBuilderTemplate>("Intent.CosmosDB.CosmosDBRepositoryBase");
            if (baseEFRepositoryTemplate == null)
                return;

            AppendCosmosDBRepositoryBase(application, baseEFRepositoryTemplate);
            SetupAutoMapper(application, baseEFRepositoryTemplate);

            var baseEFRepositoryInterfaceTemplate = application.FindTemplateInstance<ICSharpFileBuilderTemplate>("Intent.CosmosDB.CosmosDBRepositoryInterface");
            if (baseEFRepositoryInterfaceTemplate == null)
                return;

            AppendCosmosDBRepositoryInterface(application, baseEFRepositoryInterfaceTemplate);
        }

        private void SetupAutoMapper(IApplication application, ICSharpFileBuilderTemplate template)
        {
            var entityRepositories = application.FindTemplateInstances<ICSharpFileBuilderTemplate>("Intent.CosmosDB.CosmosDBRepository");
            foreach (var entityRepositoryTemplate in entityRepositories)
            {
                entityRepositoryTemplate.CSharpFile.OnBuild(file =>
                {
                    var @class = file.Classes.First();
                    var constructor = @class.Constructors.First();
                    if (constructor == null) return;

                    entityRepositoryTemplate.AddUsing("AutoMapper");

                    if (!constructor.Parameters.Any(p => p.Type.EndsWith("IMapper")))
                    {
                        constructor
                            .AddParameter("IMapper", "mapper");
                        constructor.ConstructorCall.AddArgument("mapper");
                    }
                });
            }

            template.CSharpFile.OnBuild(file =>
            {
                var @class = file.Classes.First();
                var constructor = @class.Constructors.First();
                if (constructor == null) return;

                string nullableChar = template.OutputTarget.GetProject().NullableEnabled ? "?" : "";
                template.AddUsing("AutoMapper");
                template.AddUsing("AutoMapper.QueryableExtensions");

                if (!constructor.Parameters.Any(p => p.Type.EndsWith("IMapper")))
                {
                    constructor.AddParameter("IMapper", "mapper", p => p.IntroduceReadonlyField());
                }

            });
        }

        private void AppendCosmosDBRepositoryInterface(IApplication application, ICSharpFileBuilderTemplate template)
        {
            template.CSharpFile.OnBuild(file =>
            {
                template.AddUsing("System.Collections");

                var @interface = file.Interfaces.First();
                @interface.AddMethod("Task<IEnumerable>", "FindAllProjectToWithTransformationAsync", method =>
                {
                    method
                        .AddGenericParameter("TProjection")
                        .AddParameter("Expression<Func<TDocumentInterface, bool>>?", "filterExpression")
                        .AddParameter("Func<IQueryable<TProjection>, IQueryable>", "transform")
                        .AddParameter("CancellationToken", "cancellationToken", p => p.WithDefaultValue("default"));
                });

                @interface.AddMethod("Task<List<TProjection>>", "FindAllProjectToAsync", method =>
                {
                    method
                        .AddGenericParameter("TProjection")
                        .AddParameter("Expression<Func<TDocumentInterface, bool>>?", "filterExpression")
                        .AddParameter("Func<IQueryable<TProjection>, IQueryable>", "filterProjection")
                        .AddParameter("CancellationToken", "cancellationToken", p => p.WithDefaultValue("default"));
                });
            });
        }

        private void AppendCosmosDBRepositoryBase(IApplication application, ICSharpFileBuilderTemplate template)
        {
            template.CSharpFile.OnBuild(file =>
            {
                template.AddUsing("System.Collections");
                var @class = file.Classes.First();

                @class.AddMethod("Task<IEnumerable>", "FindAllProjectToWithTransformationAsync", method =>
                {
                    method
                        .Async()
                        .AddGenericParameter("TProjection")
                        .AddParameter("Expression<Func<TDocumentInterface, bool>>?", "filterExpression")
                        .AddParameter("Func<IQueryable<TProjection>, IQueryable>", "transform")
                        .AddParameter("CancellationToken", "cancellationToken", p => p.WithDefaultValue("default"));
                    method
                        .AddStatement("var linq = await CreateQuery();")
                        .AddIfStatement("filterExpression != null", ifs => ifs.AddStatement("linq = linq.Where(filterExpression);"))
                        .AddStatement("var projected = linq.ProjectTo<TProjection>(_mapper.ConfigurationProvider);")
                        .AddStatement("var materialized = await ProcessResults(projected);")
                        .AddStatement("var results = (IQueryable<object>)transform(materialized.AsQueryable());")
                        .AddStatement("return results.ToList();", stmt => stmt.SeparatedFromPrevious());
                });
                @class.AddMethod("Task<List<TProjection>>", "FindAllProjectToAsync", method =>
                {
                    method
                        .Async()
                        .AddGenericParameter("TProjection")
                        .AddParameter("Expression<Func<TDocumentInterface, bool>>?", "filterExpression")
                        .AddParameter("Func<IQueryable<TProjection>, IQueryable>", "filterProjection")
                        .AddParameter("CancellationToken", "cancellationToken", p => p.WithDefaultValue("default"));
                    method
                        .AddStatement("var linq = await CreateQuery();")
                        .AddIfStatement("filterExpression != null", ifs => ifs.AddStatement("linq = linq.Where(filterExpression);"))
                        .AddStatement("var projected = linq.ProjectTo<TProjection>(_mapper.ConfigurationProvider);")
                        .AddStatement("var filtered = (IQueryable<TProjection>)filterProjection(projected);")
                        .AddStatement("return await ProcessResults(filtered);", stmt => stmt.SeparatedFromPrevious());
                });
            });
        }
    }
}