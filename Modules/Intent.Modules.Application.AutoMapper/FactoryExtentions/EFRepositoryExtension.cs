using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.CSharp.VisualStudio;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;
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

            var entityRepositoryTemplates = application.FindTemplateInstances<ICSharpFileBuilderTemplate>("Intent.EntityFrameworkCore.Repositories.Repository");

            foreach (var entityRepositoryTemplate in entityRepositoryTemplates)
            AppendEntityRespository(application, entityRepositoryTemplate);

            var entityRepositoryInterfaceTemplates = application.FindTemplateInstances<ICSharpFileBuilderTemplate>("Intent.Entities.Repositories.Api.EntityRepositoryInterface");

            foreach (var entityRepositoryInterfaceTemplate in entityRepositoryInterfaceTemplates)
                AppendEntityRespositoryInterface(application, entityRepositoryInterfaceTemplate);

        }

        private void AppendEntityRespositoryInterface(IApplication application, ICSharpFileBuilderTemplate template)
        {
            template.CSharpFile.OnBuild(file =>
            {
                string nullableChar = template.OutputTarget.GetProject().NullableEnabled ? "?" : "";
                var @interface = file.Interfaces.First();
                AddEntityRespositoryInterfaceMethods(template, @interface, nullableChar, true);
                if (template.ExecutionContext.Settings.GetDatabaseSettings().AddSynchronousMethodsToRepositories())
                {
                    AddEntityRespositoryInterfaceMethods(template, @interface, nullableChar, false);
                }
            });
        }

        private void AddEntityRespositoryInterfaceMethods(ICSharpFileBuilderTemplate template, CSharpInterface @interface, string nullableChar, bool asAsync)
        {
            var classModel = ((IIntentTemplate<ClassModel>)template).Model;
            var pks = GetPrimaryKey(classModel);
            string postFix = asAsync ? "Async" : "";
            @interface.AddMethod($"TProjection{nullableChar}", $"FindByIdProjectTo{postFix}", method =>
            {
                method.AddAttribute("[IntentManaged(Mode.Fully)]");
                string parameterType;
                if (pks.Count() == 1)
                {
                    var pk = pks.First();
                    parameterType = template.GetTypeName(pk.TypeReference);
                }
                else
                {
                    parameterType = $"({string.Join(", ", pks.Select(pk => $"{template.GetTypeName(pk.TypeReference)} {pk.Name.ToPascalCase()}"))})";
                }

                method
                    .AddGenericParameter("TProjection")
                    .AddParameter(parameterType, "id");

                if (asAsync) AsyncAdjust(method);
            });
        }

        private void AppendEntityRespository(IApplication application, ICSharpFileBuilderTemplate template)
        {
            template.CSharpFile.OnBuild(file =>
            {
                string nullableChar = template.OutputTarget.GetProject().NullableEnabled ? "?" : "";
                var @class = file.Classes.First();
                AddEntityRespositoryMethods(template, @class, nullableChar, true);
                if (template.ExecutionContext.Settings.GetDatabaseSettings().AddSynchronousMethodsToRepositories())
                {
                    AddEntityRespositoryMethods(template, @class, nullableChar, false);
                }
            });
        }

        private void AddEntityRespositoryMethods(ICSharpFileBuilderTemplate template, CSharpClass @class, string nullableChar, bool asAsync)
        {
            var classModel = ((IIntentTemplate<ClassModel>)template).Model;
            var pks = GetPrimaryKey(classModel);
            string postFix = asAsync ? "Async" : "";
            @class.AddMethod($"TProjection{nullableChar}", $"FindByIdProjectTo{postFix}", method =>
            {
                string parameterType;
                string filter;
                if (pks.Count() == 1)
                {
                    var pk = pks.First();
                    parameterType = template.GetTypeName(pk.TypeReference);
                    filter = $"x.{pk.Name} == {pk.Name.ToCamelCase()}";
                }
                else
                {
                    parameterType = $"({string.Join(", ", pks.Select(pk => $"{template.GetTypeName(pk.TypeReference)} {pk.Name.ToPascalCase()}"))})";
                    filter = string.Join(" && ", pks.Select(pk => $"x.{pk.Name} == id.{pk.Name.ToPascalCase()}"));
                }

                method
                    .AddGenericParameter("TProjection")
                    .AddParameter(parameterType, "id");

                if (asAsync)
                {
                    method.AddStatement($"return await FindProjectToAsync<TProjection>(x => {filter}, cancellationToken);");
                }
                else
                {
                    method.AddStatement($"return FindProjectTo<TProjection>(x => {filter});");
                }
                if (asAsync) AsyncAdjust(method);
            });
        }

        private IEnumerable<AttributeModel> GetPrimaryKey(ClassModel classModel)
        {
            while (classModel.ParentClass != null)
            {
                foreach (var pk in  classModel.Attributes.Where(a => a.HasStereotype("Primary Key")))
                {
                    yield return pk;
                }
                classModel = classModel.ParentClass;
            }

            foreach (var pk in classModel.Attributes.Where(a => a.HasStereotype("Primary Key")))
            {
                yield return pk;
            }
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
            @class.AddMethod("List<TProjection>", $"FindAllProjectTo{postFix}", method =>
            {
                method
                    .AddGenericParameter("TProjection");

                method
                    .AddStatement("var queryable = QueryInternal((Expression<Func<TPersistence, bool>>)null);")
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

            @class.AddMethod($"TProjection{nullableChar}", $"FindProjectTo{postFix}", method =>
            {
                method
                    .AddGenericParameter("TProjection")
                    .AddParameter($"Expression<Func<TPersistence, bool>>", "filterExpression");
                method
                    .AddStatement("var queryable = QueryInternal(filterExpression);")
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