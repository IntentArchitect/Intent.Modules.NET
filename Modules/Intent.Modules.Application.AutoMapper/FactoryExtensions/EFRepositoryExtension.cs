using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
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

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.Application.AutoMapper.FactoryExtensions
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

            AppendEFRepositoryInterface(baseEFRepositoryInterfaceTemplate);

            var entityRepositoryTemplates = application.FindTemplateInstances<ICSharpFileBuilderTemplate>("Intent.EntityFrameworkCore.Repositories.Repository");

            foreach (var entityRepositoryTemplate in entityRepositoryTemplates)
            {
                var classModel = ((IIntentTemplate<ClassModel>)entityRepositoryTemplate).Model;
                if (!GetPrimaryKey(classModel).Any())
                {
                    continue;
                }

                AppendEntityRepository(entityRepositoryTemplate);
                var entityRepositoryInterfaceTemplate = application.FindTemplateInstance<ICSharpFileBuilderTemplate>("Intent.Entities.Repositories.Api.EntityRepositoryInterface", ((ITemplateWithModel)entityRepositoryTemplate).Model);
                AppendEntityRepositoryInterface(entityRepositoryInterfaceTemplate);
            }
        }

        private static void AppendEntityRepositoryInterface(ICSharpFileBuilderTemplate template)
        {
            template.CSharpFile.OnBuild(file =>
            {
                var nullableChar = template.OutputTarget.GetProject().NullableEnabled ? "?" : "";
                var @interface = file.Interfaces.First();
                AddEntityRepositoryInterfaceMethods(template, @interface, nullableChar, true);
                if (template.ExecutionContext.Settings.GetDatabaseSettings().AddSynchronousMethodsToRepositories())
                {
                    AddEntityRepositoryInterfaceMethods(template, @interface, nullableChar, false);
                }
            });
        }

        private static void AddEntityRepositoryInterfaceMethods(ICSharpFileBuilderTemplate template, CSharpInterface @interface, string nullableChar, bool asAsync)
        {
            var classModel = ((IIntentTemplate<ClassModel>)template).Model;
            var pks = GetPrimaryKey(classModel).ToArray();
            var postFix = asAsync ? "Async" : "";
            @interface.AddMethod($"TProjection{nullableChar}", $"FindByIdProjectTo{postFix}", method =>
            {
                method.AddAttribute("[IntentManaged(Mode.Fully)]");
                string parameterType;
                string parameterName;
                if (pks.Length == 1)
                {
                    var pk = pks.First();
                    parameterType = template.GetTypeName(pk.TypeReference);
                    parameterName = pk.Name.ToParameterName();
                }
                else
                {
                    parameterType = $"({string.Join(", ", pks.Select(pk => $"{template.GetTypeName(pk.TypeReference)} {pk.Name.ToPascalCase()}"))})";
                    parameterName = "id";
                }

                method
                    .AddGenericParameter("TProjection")
                    .AddParameter(parameterType, parameterName);

                if (asAsync) AsyncAdjust(method);
            });
        }

        private static void AppendEntityRepository(ICSharpFileBuilderTemplate template)
        {
            template.CSharpFile.OnBuild(file =>
            {
                var nullableChar = template.OutputTarget.GetProject().NullableEnabled ? "?" : "";
                var @class = file.Classes.First();
                AddEntityRepositoryMethods(template, @class, nullableChar, true);
                if (template.ExecutionContext.Settings.GetDatabaseSettings().AddSynchronousMethodsToRepositories())
                {
                    AddEntityRepositoryMethods(template, @class, nullableChar, false);
                }
            });
        }

        private static void AddEntityRepositoryMethods(ICSharpFileBuilderTemplate template, CSharpClass @class, string nullableChar, bool asAsync)
        {
            var classModel = ((IIntentTemplate<ClassModel>)template).Model;
            var pks = GetPrimaryKey(classModel).ToArray();
            var postFix = asAsync ? "Async" : "";
            @class.AddMethod($"TProjection{nullableChar}", $"FindByIdProjectTo{postFix}", method =>
            {
                string parameterType;
                string filter;
                string parameterName;
                if (pks.Length == 1)
                {
                    var pk = pks.First();
                    parameterType = template.GetTypeName(pk.TypeReference);
                    filter = $"x.{pk.Name.ToPropertyName()} == {pk.Name.ToParameterName()}";
                    parameterName = pk.Name.ToParameterName();
                }
                else
                {
                    parameterType = $"({string.Join(", ", pks.Select(pk => $"{template.GetTypeName(pk.TypeReference)} {pk.Name.ToPascalCase()}"))})";
                    filter = string.Join(" && ", pks.Select(pk => $"x.{pk.Name.ToPropertyName()} == id.{pk.Name.ToPascalCase()}"));
                    parameterName = "id";
                }

                method
                    .AddGenericParameter("TProjection")
                    .AddParameter(parameterType, parameterName);

                method.AddStatement(asAsync
                    ? $"return await FindProjectToAsync<TProjection>(x => {filter}, cancellationToken);"
                    : $"return FindProjectTo<TProjection>(x => {filter});");

                if (asAsync)
                {
                    AsyncAdjust(method);
                }
            });
        }

        private static IEnumerable<AttributeModel> GetPrimaryKey(ClassModel classModel)
        {
            while (classModel.ParentClass != null)
            {
                foreach (var pk in classModel.Attributes.Where(a => a.HasStereotype("Primary Key")))
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

        private static void AppendEFRepositoryInterface(ICSharpFileBuilderTemplate template)
        {
            template.CSharpFile.OnBuild(file =>
            {
                var nullableChar = template.OutputTarget.GetProject().NullableEnabled ? "?" : "";
                var @interface = file.Interfaces.First();
#pragma warning disable CS0618 // Type or member is obsolete
                var pagedListInterface = template.TryGetTypeName(TemplateRoles.Repository.Interface.PagedList, out var name)
                    ? name
                    : template.GetTypeName(TemplateRoles.Repository.Interface.PagedResult); // for backward compatibility
#pragma warning restore CS0618 // Type or member is obsolete
                AddInterfaceMethods(@interface, nullableChar, pagedListInterface, true);

                if (template.ExecutionContext.Settings.GetDatabaseSettings().AddSynchronousMethodsToRepositories())
                {
                    AddInterfaceMethods(@interface, nullableChar, pagedListInterface, false);
                }
            });
        }

        private static void AddInterfaceMethods(CSharpInterface @interface, string nullableChar, string pagedListInterface, bool asAsync)
        {
            var postFix = asAsync ? "Async" : "";

            // FindAll (without paging)
            @interface.AddMethod("List<TProjection>", $"FindAllProjectTo{postFix}", method =>
            {
                method.AddGenericParameter("TProjection");

                if (asAsync)
                {
                    AsyncAdjust(method);
                }
            });

            @interface.AddMethod("List<TProjection>", $"FindAllProjectTo{postFix}", method =>
            {
                method.AddGenericParameter("TProjection");
                AddQueryOptionsParameter(method);

                if (asAsync)
                {
                    AsyncAdjust(method);
                }
            });

            @interface.AddMethod("List<TProjection>", $"FindAllProjectTo{postFix}", method =>
            {
                method.AddGenericParameter("TProjection");
                AddFilterExpressionParameter(method);

                if (asAsync)
                {
                    AsyncAdjust(method);
                }
            });

            @interface.AddMethod("List<TProjection>", $"FindAllProjectTo{postFix}", method =>
            {
                method.AddGenericParameter("TProjection");
                AddFilterExpressionParameter(method);
                AddQueryOptionsParameter(method);

                if (asAsync)
                {
                    AsyncAdjust(method);
                }
            });

            // FindAll (with paging)
            @interface.AddMethod($"{pagedListInterface}<TProjection>", $"FindAllProjectTo{postFix}", method =>
            {
                method.AddGenericParameter("TProjection");
                AddPagingParameters(method);

                if (asAsync)
                {
                    AsyncAdjust(method);
                }
            });

            @interface.AddMethod($"{pagedListInterface}<TProjection>", $"FindAllProjectTo{postFix}", method =>
            {
                method.AddGenericParameter("TProjection");
                AddPagingParameters(method);
                AddQueryOptionsParameter(method);

                if (asAsync)
                {
                    AsyncAdjust(method);
                }
            });

            @interface.AddMethod($"{pagedListInterface}<TProjection>", $"FindAllProjectTo{postFix}", method =>
            {
                method.AddGenericParameter("TProjection");
                AddFilterExpressionParameter(method);
                AddPagingParameters(method);

                if (asAsync)
                {
                    AsyncAdjust(method);
                }
            });

            @interface.AddMethod($"{pagedListInterface}<TProjection>", $"FindAllProjectTo{postFix}", method =>
            {
                method.AddGenericParameter("TProjection");
                AddFilterExpressionParameter(method);
                AddPagingParameters(method);
                AddQueryOptionsParameter(method);

                if (asAsync)
                {
                    AsyncAdjust(method);
                }
            });

            // Find
            @interface.AddMethod($"TProjection{nullableChar}", $"FindProjectTo{postFix}", method =>
            {
                method.AddGenericParameter("TProjection");
                AddFilterExpressionParameter(method);

                if (asAsync)
                {
                    AsyncAdjust(method);
                }
            });

            @interface.AddMethod($"TProjection{nullableChar}", $"FindProjectTo{postFix}", method =>
            {
                method.AddGenericParameter("TProjection");
                AddFilterExpressionParameter(method);
                AddQueryOptionsParameter(method);

                if (asAsync)
                {
                    AsyncAdjust(method);
                }
            });

            @interface.AddMethod($"TProjection{nullableChar}", $"FindProjectTo{postFix}", method =>
            {
                method.AddGenericParameter("TProjection");
                AddQueryOptionsParameter(method);

                if (asAsync)
                {
                    AsyncAdjust(method);
                }
            });

            return;

            static void AddPagingParameters(CSharpInterfaceMethod method)
            {
                method
                    .AddParameter("int", "pageNo")
                    .AddParameter("int", "pageSize");
            }

            static void AddQueryOptionsParameter(CSharpInterfaceMethod method)
            {
                method.AddParameter(
                    type: "Func<IQueryable<TPersistence>, IQueryable<TPersistence>>",
                    name: "queryOptions");
            }

            static void AddFilterExpressionParameter(CSharpInterfaceMethod method)
            {
                method.AddParameter("Expression<Func<TPersistence, bool>>", "filterExpression");
            }
        }

        private static void AsyncAdjust(CSharpInterfaceMethod method)
        {
            method.Async();
            method.AddParameter("CancellationToken", "cancellationToken", p => p.WithDefaultValue("default"));
        }

        private static void AppendEFRepository(IApplication application, ICSharpFileBuilderTemplate template)
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

                    if (constructor.Parameters.Any(p => p.Type.EndsWith("IMapper")))
                    {
                        return;
                    }

                    constructor.AddParameter("IMapper", "mapper");
                    constructor.ConstructorCall.AddArgument("mapper");
                });
            }

            template.CSharpFile.OnBuild(file =>
            {
#pragma warning disable CS0618 // Type or member is obsolete
                var pagedListInterface = template.TryGetTypeName(TemplateRoles.Repository.Interface.PagedList, out var name)
                    ? name
                    : template.GetTypeName(TemplateRoles.Repository.Interface.PagedResult); // for backward compatibility
#pragma warning restore CS0618 // Type or member is obsolete
                template.AddNugetDependency(NugetPackages.AutoMapper(template.OutputTarget));
                var @class = file.Classes.First();
                var constructor = @class.Constructors.First();
                if (constructor == null) return;

                var nullableChar = template.OutputTarget.GetProject().NullableEnabled ? "?" : "";
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

        private static void AddImplementationMethods(CSharpClass @class, string nullableChar, string pagedListInterface, bool asAsync)
        {
            var postFix = asAsync ? "Async" : "";

            // FindAll (without paging)
            @class.AddMethod("List<TProjection>", $"FindAllProjectTo{postFix}", method =>
            {
                method.AddGenericParameter("TProjection");

                method
                    .AddStatement("var queryable = QueryInternal(filterExpression: null);")
                    .AddStatement("var projection = queryable.ProjectTo<TProjection>(_mapper.ConfigurationProvider);");

                method.AddStatement(asAsync
                    ? "return await projection.ToListAsync(cancellationToken);"
                    : "return projection.ToList();");

                if (asAsync)
                {
                    AsyncAdjust(method);
                }
            });

            @class.AddMethod("List<TProjection>", $"FindAllProjectTo{postFix}", method =>
            {
                method.AddGenericParameter("TProjection");
                AddQueryOptionsParameter(method);

                method
                    .AddStatement("var queryable = QueryInternal(queryOptions);")
                    .AddStatement("var projection = queryable.ProjectTo<TProjection>(_mapper.ConfigurationProvider);");

                method.AddStatement(asAsync
                    ? "return await projection.ToListAsync(cancellationToken);"
                    : "return projection.ToList();");

                if (asAsync)
                {
                    AsyncAdjust(method);
                }
            });

            @class.AddMethod("List<TProjection>", $"FindAllProjectTo{postFix}", method =>
            {
                method.AddGenericParameter("TProjection");
                AddFilterExpressionParameter(method);

                method
                    .AddStatement("var queryable = QueryInternal(filterExpression);")
                    .AddStatement("var projection = queryable.ProjectTo<TProjection>(_mapper.ConfigurationProvider);");

                method.AddStatement(asAsync
                    ? "return await projection.ToListAsync(cancellationToken);"
                    : "return projection.ToList();");

                if (asAsync)
                {
                    AsyncAdjust(method);
                }
            });

            @class.AddMethod("List<TProjection>", $"FindAllProjectTo{postFix}", method =>
            {
                method.AddGenericParameter("TProjection");
                AddFilterExpressionParameter(method);
                AddQueryOptionsParameter(method);

                method
                    .AddStatement("var queryable = QueryInternal(filterExpression, queryOptions);")
                    .AddStatement("var projection = queryable.ProjectTo<TProjection>(_mapper.ConfigurationProvider);");

                method.AddStatement(asAsync
                    ? "return await projection.ToListAsync(cancellationToken);"
                    : "return projection.ToList();");

                if (asAsync)
                {
                    AsyncAdjust(method);
                }
            });

            // FindAll (with paging)
            @class.AddMethod($"{pagedListInterface}<TProjection>", $"FindAllProjectTo{postFix}", method =>
            {
                method.AddGenericParameter("TProjection");
                AddPagingParameters(method);

                method
                    .AddStatement("var queryable = QueryInternal(filterExpression: null);")
                    .AddStatement("var projection = queryable.ProjectTo<TProjection>(_mapper.ConfigurationProvider);");

                method.AddStatement(asAsync
                    ? """
                      return await ToPagedListAsync(
                                      projection,
                                      pageNo,
                                      pageSize,
                                      cancellationToken);
                      """
                    : """
                      return ToPagedList(
                                      projection,
                                      pageNo,
                                      pageSize);
                      """);

                if (asAsync)
                {
                    AsyncAdjust(method);
                }
            });

            @class.AddMethod($"{pagedListInterface}<TProjection>", $"FindAllProjectTo{postFix}", method =>
            {
                method.AddGenericParameter("TProjection");
                AddPagingParameters(method);
                AddQueryOptionsParameter(method);

                method
                    .AddStatement("var queryable = QueryInternal(queryOptions);")
                    .AddStatement("var projection = queryable.ProjectTo<TProjection>(_mapper.ConfigurationProvider);");

                method.AddStatement(asAsync
                    ? """
                      return await ToPagedListAsync(
                                      projection,
                                      pageNo,
                                      pageSize,
                                      cancellationToken);
                      """
                    : """
                      return ToPagedList(
                                      projection,
                                      pageNo,
                                      pageSize);
                      """);

                if (asAsync)
                {
                    AsyncAdjust(method);
                }
            });

            @class.AddMethod($"{pagedListInterface}<TProjection>", $"FindAllProjectTo{postFix}", method =>
            {
                method.AddGenericParameter("TProjection");
                AddFilterExpressionParameter(method);
                AddPagingParameters(method);

                method
                    .AddStatement("var queryable = QueryInternal(filterExpression);")
                    .AddStatement("var projection = queryable.ProjectTo<TProjection>(_mapper.ConfigurationProvider);");

                method.AddStatement(asAsync
                    ? """
                      return await ToPagedListAsync(
                                      projection,
                                      pageNo,
                                      pageSize,
                                      cancellationToken);
                      """
                    : """
                      return ToPagedList(
                                      projection,
                                      pageNo,
                                      pageSize);
                      """);

                if (asAsync)
                {
                    AsyncAdjust(method);
                }
            });

            @class.AddMethod($"{pagedListInterface}<TProjection>", $"FindAllProjectTo{postFix}", method =>
            {
                method.AddGenericParameter("TProjection");
                AddFilterExpressionParameter(method);
                AddPagingParameters(method);
                AddQueryOptionsParameter(method);

                method
                    .AddStatement("var queryable = QueryInternal(filterExpression, queryOptions);")
                    .AddStatement("var projection = queryable.ProjectTo<TProjection>(_mapper.ConfigurationProvider);");

                method.AddStatement(asAsync
                    ? """
                      return await ToPagedListAsync(
                                      projection,
                                      pageNo,
                                      pageSize,
                                      cancellationToken);
                      """
                    : """
                      return ToPagedList(
                                      projection,
                                      pageNo,
                                      pageSize);
                      """);

                if (asAsync)
                {
                    AsyncAdjust(method);
                }
            });

            // Find
            @class.AddMethod($"TProjection{nullableChar}", $"FindProjectTo{postFix}", method =>
            {
                method.AddGenericParameter("TProjection");
                AddFilterExpressionParameter(method);

                method
                    .AddStatement("var queryable = QueryInternal(filterExpression);")
                    .AddStatement("var projection = queryable.ProjectTo<TProjection>(_mapper.ConfigurationProvider);");

                method.AddStatement(asAsync
                    ? "return await projection.FirstOrDefaultAsync(cancellationToken);"
                    : "return projection.FirstOrDefault();");

                if (asAsync)
                {
                    AsyncAdjust(method);
                }
            });

            @class.AddMethod($"TProjection{nullableChar}", $"FindProjectTo{postFix}", method =>
            {
                method.AddGenericParameter("TProjection");
                AddFilterExpressionParameter(method);
                AddQueryOptionsParameter(method);

                method
                    .AddStatement("var queryable = QueryInternal(filterExpression, queryOptions);")
                    .AddStatement("var projection = queryable.ProjectTo<TProjection>(_mapper.ConfigurationProvider);");

                method.AddStatement(asAsync
                    ? "return await projection.FirstOrDefaultAsync(cancellationToken);"
                    : "return projection.FirstOrDefault();");

                if (asAsync)
                {
                    AsyncAdjust(method);
                }
            });

            @class.AddMethod($"TProjection{nullableChar}", $"FindProjectTo{postFix}", method =>
            {
                method.AddGenericParameter("TProjection");
                AddQueryOptionsParameter(method);

                method
                    .AddStatement("var queryable = QueryInternal(queryOptions);")
                    .AddStatement("var projection = queryable.ProjectTo<TProjection>(_mapper.ConfigurationProvider);");

                method.AddStatement(asAsync
                    ? "return await projection.FirstOrDefaultAsync(cancellationToken);"
                    : "return projection.FirstOrDefault();");

                if (asAsync)
                {
                    AsyncAdjust(method);
                }
            });

            return;

            static void AddPagingParameters(CSharpClassMethod method)
            {
                method
                    .AddParameter("int", "pageNo")
                    .AddParameter("int", "pageSize");
            }

            static void AddQueryOptionsParameter(CSharpClassMethod method)
            {
                method.AddParameter(
                    type: "Func<IQueryable<TPersistence>, IQueryable<TPersistence>>",
                    name: "queryOptions");
            }

            static void AddFilterExpressionParameter(CSharpClassMethod method)
            {
                method.AddParameter("Expression<Func<TPersistence, bool>>", "filterExpression");
            }
        }

        private static void AsyncAdjust(CSharpClassMethod method)
        {
            method.Async();
            method.AddParameter("CancellationToken", "cancellationToken", p => p.WithDefaultValue("default"));
        }
    }
}