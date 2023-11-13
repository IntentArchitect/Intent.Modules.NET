using Intent.Engine;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common;
using Intent.Modules.Common.Plugins;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;
using System.Reflection;
using System;
using System.Linq;
using Intent.Modules.Common.CSharp.VisualStudio;
using System.Reflection.Metadata;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.Application.AutoMapper.FactoryExtentions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class EFRepositoryExtension : FactoryExtensionBase
    {
        public override string Id => "Intent.Application.AutoMapper.EFRepositoryExtension";

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
                string nullableChar = template.OutputTarget.GetProject().NullableEnabled ? "?" : "";
                var @interface = file.Interfaces.First();
                @interface.AddMethod("Task<List<TProjection>>", "FindAllProjectToAsync", method => 
                {
                    method
                        .AddGenericParameter("TProjection")
                        .AddParameter($"Expression<Func<TPersistence, bool>>{nullableChar}", "filterExpression")
                        .AddParameter("CancellationToken", "cancellationToken", p => p.WithDefaultValue("default"));
                });
                @interface.AddMethod($"Task<TProjection{nullableChar}>", "FindProjectToAsync", method =>
                {
                    method
                        .AddGenericParameter("TProjection")
                        .AddParameter($"Expression<Func<TPersistence, bool>>{nullableChar}", "filterExpression")
                        .AddParameter("CancellationToken", "cancellationToken", p => p.WithDefaultValue("default"));
                });
                if (template.ExecutionContext.Settings.GetDatabaseSettings().AddSynchronousMethodsToRepositories())
                {
                    @interface.AddMethod("List<TProjection>", "FindAllProjectTo", method =>
                    {
                        method
                            .AddGenericParameter("TProjection")
                            .AddParameter($"Expression<Func<TPersistence, bool>>{nullableChar}", "filterExpression")
                            ;
                    });
                    @interface.AddMethod($"TProjection{nullableChar}", "FindProjectTo", method =>
                    {
                        method
                            .AddGenericParameter("TProjection")
                            .AddParameter($"Expression<Func<TPersistence, bool>>{nullableChar}", "filterExpression")
                            ;
                    });

                }
            });
        }

        private void AppendEFRepository(IApplication application, ICSharpFileBuilderTemplate template)
        {
            var entityRepositories = application.FindTemplateInstances<ICSharpFileBuilderTemplate>("Intent.EntityFrameworkCore.Repositories.Repository");
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
                template.AddNugetDependency(NugetPackages.AutoMapper);
                var @class = file.Classes.First();
                var constructor = @class.Constructors.First();
                if (constructor == null) return;

                string nullableChar = template.OutputTarget.GetProject().NullableEnabled ? "?" : "";
                template.AddUsing("AutoMapper");
                template.AddUsing("AutoMapper.QueryableExtensions");

                if (!constructor.Parameters.Any(p => p.Type.EndsWith("IMapper")))
                {
                    constructor.AddParameter("IMapper", "mapper", p => p.IntroduceReadonlyField() );
                }

                @class.AddMethod("Task<List<TProjection>>", "FindAllProjectToAsync", method =>
                {
                    method
                        .Async()
                        .AddGenericParameter("TProjection")
                        .AddParameter($"Expression<Func<TPersistence, bool>>{nullableChar}", "filterExpression")
                        .AddParameter("CancellationToken", "cancellationToken", p => p.WithDefaultValue("default"));
                    method
                        .AddStatement("var queryable = QueryInternal(filterExpression);")
                        .AddStatement("var projection = queryable.ProjectTo<TProjection>(_mapper.ConfigurationProvider);")
                        .AddStatement("return await projection.ToListAsync(cancellationToken);");
                });
                @class.AddMethod($"Task<TProjection{nullableChar}>", "FindProjectToAsync", method =>
                {
                    method
                        .Async()
                        .AddGenericParameter("TProjection")
                        .AddParameter($"Expression<Func<TPersistence, bool>>{nullableChar}", "filterExpression")
                        .AddParameter("CancellationToken", "cancellationToken", p => p.WithDefaultValue("default"));
                    method
                        .AddStatement("var queryable = QueryInternal(filterExpression);")
                        .AddStatement("var projection = queryable.ProjectTo<TProjection>(_mapper.ConfigurationProvider);")
                        .AddStatement("return await projection.FirstOrDefaultAsync(cancellationToken);");
                });
                if (template.ExecutionContext.Settings.GetDatabaseSettings().AddSynchronousMethodsToRepositories())
                {
                    @class.AddMethod("List<TProjection>", "FindAllProjectTo", method =>
                    {
                        method
                            .AddGenericParameter("TProjection")
                            .AddParameter("Expression<Func<TPersistence, bool>>?", "filterExpression");
                        method
                        .AddStatement("var queryable = QueryInternal(filterExpression);")
                        .AddStatement("var projection = queryable.ProjectTo<TProjection>(_mapper.ConfigurationProvider);")
                        .AddStatement("return projection.ToList();");
                    });
                    @class.AddMethod($"TProjection{nullableChar}", "FindProjectTo", method =>
                    {
                        method
                            .AddGenericParameter("TProjection")
                            .AddParameter($"Expression<Func<TPersistence, bool>>{nullableChar}", "filterExpression");
                        method
                        .AddStatement("var queryable = QueryInternal(filterExpression);")
                        .AddStatement("var projection = queryable.ProjectTo<TProjection>(_mapper.ConfigurationProvider);")
                        .AddStatement("return projection.FirstOrDefault();");
                    });
                }
            });
        }
    }
}