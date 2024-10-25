using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.CSharp.VisualStudio;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;
using static Intent.Modules.Constants.TemplateRoles.Repository;

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
                var pagedListInterface = template.TryGetTypeName(TemplateRoles.Repository.Interface.PagedList, out var name)
                    ? name
                    : template.GetTypeName(TemplateRoles.Repository.Interface.PagedResult); // for backward compatibility
                AddInterfaceMethods(@interface, nullableChar, pagedListInterface, true);
                if (template.ExecutionContext.Settings.GetDatabaseSettings().AddSynchronousMethodsToRepositories())
                {
                    AddInterfaceMethods(@interface, nullableChar, pagedListInterface, false);
                }
            });
        }

        private void AddInterfaceMethods(CSharpInterface @interface, string nullableChar, string pagedListInterface, bool asAsync)
        {
            string postFix = asAsync ? "Async" : "";
            @interface.AddMethod("List<TProjection>", $"FindAllProjectTo{postFix}", method =>
            {
                method.AddGenericParameter("TProjection");
                if (asAsync) AsyncAdjust(method);
            });
            @interface.AddMethod("List<TProjection>", $"FindAllProjectTo{postFix}", method =>
            {
                method
                    .AddGenericParameter("TProjection")
                    .AddParameter($"Func<IQueryable<TPersistence>, IQueryable<TPersistence>>{nullableChar}", "queryOptions", p => p.WithDefaultValue("default"));
                if (asAsync) AsyncAdjust(method);
            });
            @interface.AddMethod($"{pagedListInterface}<TProjection>", $"FindAllProjectTo{postFix}", method =>
            {
                method
                    .AddGenericParameter("TProjection")
                    .AddParameter("int", "pageNo")
                    .AddParameter("int", "pageSize")
                    .AddParameter($"Func<IQueryable<TPersistence>, IQueryable<TPersistence>>{nullableChar}", "queryOptions", p => p.WithDefaultValue("default"))
                    ;
                if (asAsync) AsyncAdjust(method);
            });
            @interface.AddMethod($"TProjection{nullableChar}", $"FindProjectTo{postFix}", method =>
            {
                method
                    .AddGenericParameter("TProjection")
                    .AddParameter($"Func<IQueryable<TPersistence>, IQueryable<TPersistence>>", "queryOptions")
                    ;
                if (asAsync) AsyncAdjust(method);
            });
            @interface.AddMethod($"TProjection{nullableChar}", $"FindProjectTo{postFix}", method =>
            {
                method
                    .AddGenericParameter("TProjection")
                    .AddParameter($"Expression<Func<TPersistence, bool>>", "filterExpression")
                    ;
                if (asAsync) AsyncAdjust(method);
            });
        }

        private void AsyncAdjust(CSharpInterfaceMethod method)
        {
            method.Async();
            method.AddParameter("CancellationToken", "cancellationToken", p => p.WithDefaultValue("default"));
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
                var pagedListInterface = template.TryGetTypeName(TemplateRoles.Repository.Interface.PagedList, out var name)
                    ? name
                    : template.GetTypeName(TemplateRoles.Repository.Interface.PagedResult); // for backward compatibility
                template.AddNugetDependency(NugetPackages.AutoMapper(template.OutputTarget));
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

                AddImplementationMethods(@class, nullableChar, pagedListInterface, true);
                if (template.ExecutionContext.Settings.GetDatabaseSettings().AddSynchronousMethodsToRepositories())
                {
                    AddImplementationMethods(@class, nullableChar, pagedListInterface, false);
                }
            });
        }

        private void AddImplementationMethods(CSharpClass @class, string nullableChar, string pagedListInterface, bool asAsync)
        {
            string postFix = asAsync ? "Async" : "";

            @class.AddMethod("List<TProjection>", $"FindAllProjectTo{postFix}", method =>
            {
                method
                    .AddGenericParameter("TProjection")
                    .AddParameter($"Func<IQueryable<TPersistence>, IQueryable<TPersistence>>{nullableChar}", "queryOptions", p => p.WithDefaultValue("default"));

                method
                    .AddStatement("var queryable = QueryInternal(queryOptions);")
                    .AddStatement("var projection = queryable.ProjectTo<TProjection>(_mapper.ConfigurationProvider);");

                if (asAsync)
                {
                    method.AddStatement("return await projection.ToListAsync(cancellationToken);");
                }
                else
                {
                    method.AddStatement("return projection.ToList();");
                }
                if (asAsync) AsyncAdjust(method);
            });
            @class.AddMethod($"{pagedListInterface}<TProjection>", $"FindAllProjectTo{postFix}", method =>
            {
                method
                    .AddGenericParameter("TProjection")
                    .AddParameter("int", "pageNo")
                    .AddParameter("int", "pageSize")
                    .AddParameter($"Func<IQueryable<TPersistence>, IQueryable<TPersistence>>{nullableChar}", "queryOptions", p => p.WithDefaultValue("default"));

                method
                    .AddStatement("var queryable = QueryInternal(queryOptions);")
                    .AddStatement("var projection = queryable.ProjectTo<TProjection>(_mapper.ConfigurationProvider);");

                if (asAsync)
                {
                    method.AddStatement(@"return await ToPagedListAsync(
                projection,
                pageNo,
                pageSize,
                cancellationToken);");
                }
                else
                {
                    method.AddStatement(@"return ToPagedList(
                projection,
                pageNo,
                pageSize);");
                }
                if (asAsync) AsyncAdjust(method);
            });

            @class.AddMethod($"TProjection{nullableChar}", $"FindProjectTo{postFix}", method =>
            {
                method
                    .AddGenericParameter("TProjection")
                    .AddParameter($"Func<IQueryable<TPersistence>, IQueryable<TPersistence>>", "queryOptions");
                method
                    .AddStatement("var queryable = QueryInternal(queryOptions);")
                    .AddStatement("var projection = queryable.ProjectTo<TProjection>(_mapper.ConfigurationProvider);");

                if (asAsync)
                {
                    method.AddStatement("return await projection.FirstOrDefaultAsync(cancellationToken);");
                }
                else
                {
                    method.AddStatement("return projection.FirstOrDefault();");
                }
                if (asAsync) AsyncAdjust(method);
            });
        }

        private void AsyncAdjust(CSharpClassMethod method)
        {
            method.Async();
            method.AddParameter("CancellationToken", "cancellationToken", p => p.WithDefaultValue("default"));
        }

    }
}