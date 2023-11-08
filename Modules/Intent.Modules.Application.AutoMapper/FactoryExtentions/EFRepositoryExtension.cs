using Intent.Engine;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common;
using Intent.Modules.Common.Plugins;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;
using System.Reflection;
using System;
using System.Linq;

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
                template.AddNugetDependency(NugetPackages.AutoMapper);
                var @interface = file.Interfaces.First();
                @interface.AddMethod("Task<List<TProjection>>", "FindAllProjectToAsync", method => 
                {
                    method
                        .AddGenericParameter("TProjection")
                        .AddParameter("Expression<Func<TPersistence, bool>>?", "filterExpression")
                        .AddParameter("CancellationToken", "cancellationToken", p => p.WithDefaultValue("default"));
                });
                if (template.ExecutionContext.Settings.GetDatabaseSettings().AddSynchronousMethodsToRepositories())
                {
                    @interface.AddMethod("List<TProjection>", "FindAllProjectTo", method =>
                    {
                        method
                            .AddGenericParameter("TProjection")
                            .AddParameter("Expression<Func<TPersistence, bool>>?", "filterExpression")
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
                var @class = file.Classes.First();
                var constructor = @class.Constructors.First();
                if (constructor == null) return;

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
                        .AddParameter("Expression<Func<TPersistence, bool>>?", "filterExpression")
                        .AddParameter("CancellationToken", "cancellationToken", p => p.WithDefaultValue("default"));
                    method
                        .AddStatement("var queryable = QueryInternal(filterExpression);")
                        .AddStatement("var dtoProjection = queryable.ProjectTo<TProjection>(_mapper.ConfigurationProvider);")
                        .AddStatement("return await dtoProjection.ToListAsync(cancellationToken);");
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
                        .AddStatement("var dtoProjection = queryable.ProjectTo<TProjection>(_mapper.ConfigurationProvider);")
                        .AddStatement("return dtoProjection.ToList();");
                    });
                }
            });
        }
    }
}