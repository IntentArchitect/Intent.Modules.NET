using System.Linq;
using Intent.Engine;
using Intent.Modules.AspNetCore.ODataQuery.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Plugins;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.AspNetCore.ODataQuery.FactoryExtensions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class EFRepositoryExtension : FactoryExtensionBase
    {
        public override string Id => "Intent.AspNetCore.ODataQuery.EFRepositoryExtension";

        [IntentManaged(Mode.Ignore)]
        public override int Order => 0;

        protected override void OnAfterTemplateRegistrations(IApplication application)
        {
            base.OnAfterTemplateRegistrations(application);
            var baseEFRepositoryTemplate = application.FindTemplateInstance<ICSharpFileBuilderTemplate>("Intent.EntityFrameworkCore.Repositories.RepositoryBase");
            if (baseEFRepositoryTemplate == null)
                return;

            AppendEFRepository(application, baseEFRepositoryTemplate);

            var baseEFRepositoryInterfaceTemplate = application.FindTemplateInstance<ICSharpFileBuilderTemplate>("Intent.EntityFrameworkCore.Repositories.EFRepositoryInterface");
            if (baseEFRepositoryInterfaceTemplate == null)
                return;

            AppendEFRepositoryInterface(application, baseEFRepositoryInterfaceTemplate);


        }

        private void AppendEFRepositoryInterface(IApplication application, ICSharpFileBuilderTemplate template)
        {
            template.CSharpFile.OnBuild(file =>
            {
                template.AddUsing("System.Collections");

                var @interface = file.Interfaces.First();
                @interface.AddMethod("Task<IEnumerable>", "FindAllProjectToWithTransformationAsync", method =>
                {
                    method
                        .AddGenericParameter("TProjection")
                        .AddParameter("Expression<Func<TPersistence, bool>>?", "filterExpression")
                        .AddParameter("Func<IQueryable<TProjection>, IQueryable>", "transform")
                        .AddParameter("CancellationToken", "cancellationToken", p => p.WithDefaultValue("default"));
                });

                @interface.AddMethod("Task<List<TProjection>>", "FindAllProjectToAsync", method =>
                {
                    method
                        .AddGenericParameter("TProjection")
                        .AddParameter("Expression<Func<TPersistence, bool>>?", "filterExpression")
                        .AddParameter("Func<IQueryable<TProjection>, IQueryable>", "filterProjection")
                        .AddParameter("CancellationToken", "cancellationToken", p => p.WithDefaultValue("default"));
                });
                if (template.ExecutionContext.Settings.GetDatabaseSettings().AddSynchronousMethodsToRepositories())
                {
                    @interface.AddMethod("IEnumerable", "FindAllProjectToWithTransformation", method =>
                    {
                        method
                            .AddGenericParameter("TProjection")
                            .AddParameter("Expression<Func<TPersistence, bool>>?", "filterExpression")
                            .AddParameter("Func<IQueryable<TProjection>, IQueryable>", "transform");
                    });

                    @interface.AddMethod("List<TProjection>", "FindAllProjectTo", method =>
                    {
                        method
                            .AddGenericParameter("TProjection")
                            .AddParameter("Expression<Func<TPersistence, bool>>?", "filterExpression")
                            .AddParameter("Func<IQueryable<TProjection>, IQueryable>", "filterProjection");
                    });
                }
            });
        }

        private void AppendEFRepository(IApplication application, ICSharpFileBuilderTemplate template)
        {
            var entityRepositories = application.FindTemplateInstances<ICSharpFileBuilderTemplate>("Intent.EntityFrameworkCore.Repositories.Repository");

            template.CSharpFile.OnBuild(file =>
            {
                template.AddUsing("System.Collections");
                var @class = file.Classes.First();

                @class.AddMethod("Task<IEnumerable>", "FindAllProjectToWithTransformationAsync", method =>
                {
                    method
                        .Async()
                        .AddGenericParameter("TProjection")
                        .AddParameter("Expression<Func<TPersistence, bool>>?", "filterExpression")
                        .AddParameter("Func<IQueryable<TProjection>, IQueryable>", "transform")
                        .AddParameter("CancellationToken", "cancellationToken", p => p.WithDefaultValue("default"));
                    method
                        .AddStatement("var queryable = QueryInternal(filterExpression);")
                        .AddStatement("var projection = queryable.ProjectTo<TProjection>(_mapper.ConfigurationProvider);")
                        .AddStatement("var response = transform(projection);")
                        .AddStatement("return await response.Cast<object>().ToListAsync();");
                });
                @class.AddMethod("Task<List<TProjection>>", "FindAllProjectToAsync", method =>
                {
                    method
                        .Async()
                        .AddGenericParameter("TProjection")
                        .AddParameter("Expression<Func<TPersistence, bool>>?", "filterExpression")
                        .AddParameter("Func<IQueryable<TProjection>, IQueryable>", "filterProjection")
                        .AddParameter("CancellationToken", "cancellationToken", p => p.WithDefaultValue("default"));
                    method
                        .AddStatement("var queryable = QueryInternal(filterExpression);")
                        .AddStatement("var projection = queryable.ProjectTo<TProjection>(_mapper.ConfigurationProvider);")
                        .AddStatement("var response = filterProjection(projection);")
                        .AddStatement("return await response.Cast<TProjection>().ToListAsync();");
                });
                if (template.ExecutionContext.Settings.GetDatabaseSettings().AddSynchronousMethodsToRepositories())
                {
                    @class.AddMethod("IEnumerable", "FindAllProjectToWithTransformation", method =>
                    {
                        method
                            .AddGenericParameter("TProjection")
                            .AddParameter("Expression<Func<TPersistence, bool>>?", "filterExpression")
                            .AddParameter("Func<IQueryable<TProjection>, IQueryable>", "transform");
                        method
                        .AddStatement("var queryable = QueryInternal(filterExpression);")
                        .AddStatement("var projection = queryable.ProjectTo<TProjection>(_mapper.ConfigurationProvider);")
                        .AddStatement("var response = transform(projection);")
                        .AddStatement("return response.Cast<object>().ToList();");
                    });
                    @class.AddMethod("List<TProjection>", "FindAllProjectTo", method =>
                    {
                        method
                            .AddGenericParameter("TProjection")
                            .AddParameter("Expression<Func<TPersistence, bool>>?", "filterExpression")
                            .AddParameter("Func<IQueryable<TProjection>, IQueryable>", "filterProjection");
                        method
                        .AddStatement("var queryable = QueryInternal(filterExpression);")
                        .AddStatement("var projection = queryable.ProjectTo<TProjection>(_mapper.ConfigurationProvider);")
                        .AddStatement("var response = filterProjection(projection);")
                        .AddStatement("return response.Cast<TProjection>().ToList();");
                    });
                }
            });
        }
    }
}