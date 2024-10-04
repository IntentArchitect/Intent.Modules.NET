using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using Intent.Engine;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Ardalis.Repositories.Templates;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using Intent.Modules.Entities.Repositories.Api.Templates;
using Intent.Modules.Entities.Repositories.Api.Templates.EntityRepositoryInterface;
using Intent.Modules.EntityFrameworkCore.Repositories.Templates;
using Intent.Modules.EntityFrameworkCore.Repositories.Templates.EFRepositoryInterface;
using Intent.Modules.EntityFrameworkCore.Repositories.Templates.Repository;
using Intent.Modules.EntityFrameworkCore.Repositories.Templates.RepositoryBase;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;
using Intent.Modules.Modelers.Domain.Settings;
using static Intent.Modules.Constants.TemplateRoles.Repository;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.Ardalis.Repositories.FactoryExtensions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class RepositoryFactoryExtension : FactoryExtensionBase
    {
        public override string Id => "Intent.Ardalis.Repositories.RepositoryFactoryExtension";

        [IntentManaged(Mode.Ignore)]
        public override int Order => 0;

        /// <summary>
        /// This is an example override which would extend the
        /// <see cref="ExecutionLifeCycleSteps.Start"/> phase of the Software Factory execution.
        /// See <see cref="FactoryExtensionBase"/> for all available overrides.
        /// </summary>
        /// <remarks>
        /// It is safe to update or delete this method.
        /// </remarks>
        protected override void OnBeforeTemplateExecution(IApplication application)
        {
            UpdateRepositoryBase(application);
            UpdateEntityRepositoryInterface(application);
            UpdateEntityRepository(application);
            UpdateEFInterface(application);
        }

        private void UpdateEFInterface(IApplication application)
        {
            var repoInterfaceTemplates = application.FindTemplateInstances<CSharpTemplateBase<object>>(TemplateDependency.OnTemplate(EFRepositoryInterfaceTemplate.TemplateId));
            foreach (var template in repoInterfaceTemplates)
            {
                template.AddNugetDependency(NugetPackages.ArdalisSpecification(template.OutputTarget));

                ((ICSharpFileBuilderTemplate)template).CSharpFile.AfterBuild(file =>
                {
                    file.AddUsing("Ardalis.Specification");

                    var @interface = file.Interfaces.First();
                    if (@interface.GenericParameters.FirstOrDefault(x => x.TypeName == "TPersistence") != null)
                    {
                        @interface.GenericParameters.Remove(@interface.GenericParameters.First(x => x.TypeName == "TPersistence"));
                    }
                    @interface.Interfaces.Clear();
                    @interface.Methods.Clear();
                    @interface.Properties.Clear();

                    @interface.AddProperty(template.GetUnitOfWorkInterfaceName(), "UnitOfWork", prop => { prop.ReadOnly(); });
                    @interface.AddMethod("void", "Add", method => { method.AddParameter("TDomain", "entity"); });
                    @interface.AddMethod("void", "Remove", method => { method.AddParameter("TDomain", "entity"); });

                    @interface.AddMethod($"Task<List<TDomain>>", "FindAllAsync", method => { method.AddParameter("CancellationToken", "cancellationToken", p => p.WithDefaultValue("default")); });
                    @interface.AddMethod($"Task<{PagedListInterfaceName(template)}<TDomain>>", "FindAllAsync", method => 
                    {
                        method.AddParameter("int", "pageNo");
                        method.AddParameter("int", "PageSize");
                        method.AddParameter("CancellationToken", "cancellationToken", p => p.WithDefaultValue("default")); 
                    });
                    @interface.AddMethod($"Task<{PagedListInterfaceName(template)}<TDomain>>", "FindAllAsync", method =>
                    {
                        method.AddParameter($"Expression<Func<TDomain, bool>>", "filterExpression");
                        method.AddParameter("int", "pageNo");
                        method.AddParameter("int", "PageSize");
                        method.AddParameter("CancellationToken", "cancellationToken", p => p.WithDefaultValue("default"));
                    });
                    @interface.AddMethod($"Task<{PagedListInterfaceName(template)}<TDomain>>", "FindAllAsync", method =>
                    {
                        method.AddParameter($"Expression<Func<TDomain, bool>>", "filterExpression");
                        method.AddParameter("int", "pageNo");
                        method.AddParameter("int", "PageSize");
                        method.AddParameter($"Func<IQueryable<TDomain>, IQueryable<TDomain>> ", "linq");
                        method.AddParameter("CancellationToken", "cancellationToken", p => p.WithDefaultValue("default"));
                    });
                    @interface.AddMethod($"Task<{PagedListInterfaceName(template)}<TDomain>>", "FindAllAsync", method =>
                    {
                        method.AddParameter($"ISpecification<TDomain>", "specification");
                        method.AddParameter("int", "pageNo");
                        method.AddParameter("int", "PageSize");
                        method.AddParameter($"Func<IQueryable<TDomain>, IQueryable<TDomain>> ", "queryOptions");
                        method.AddParameter("CancellationToken", "cancellationToken", p => p.WithDefaultValue("default"));
                    });
                    @interface.AddMethod($"Task<{PagedListInterfaceName(template)}<TDomain>>", "FindAllAsync", method =>
                    {
                        method.AddParameter($"ISpecification<TDomain>", "specification");
                        method.AddParameter("int", "pageNo");
                        method.AddParameter("int", "PageSize");
                        method.AddParameter("CancellationToken", "cancellationToken", p => p.WithDefaultValue("default"));
                    });
                });
            }
        }

        private static void UpdateRepositoryBase(IApplication application)
        {
            var template = application.FindTemplateInstance<CSharpTemplateBase<object>>(TemplateDependency.OnTemplate(RepositoryBaseTemplate.TemplateId));
            if (template != null)
            {
                template.AddNugetDependency(NugetPackages.ArdalisSpecificationEntityFrameworkCore(template.OutputTarget));

                ((ICSharpFileBuilderTemplate)template).CSharpFile.AfterBuild(file =>
                {
                    file.AddUsing("Ardalis.Specification");
                    file.AddUsing("Ardalis.Specification.EntityFrameworkCore");
                    file.AddUsing("Microsoft.EntityFrameworkCore");

                    var @class = file.Classes.First();
                    if (@class.GenericParameters.FirstOrDefault(x => x.TypeName == "TPersistence") != null)
                    {
                        @class.GenericParameters.Remove(@class.GenericParameters.First(x => x.TypeName == "TPersistence"));
                        if (@class.GenericTypeConstraints.FirstOrDefault(x => x.GenericTypeParameter == "TPersistence") != null)
                        {
                            @class.GenericTypeConstraints.Remove(@class.GenericTypeConstraints.First(x => x.GenericTypeParameter == "TPersistence"));
                        }
                    }

                    @class.ExtendsClass($"RepositoryBase<TDomain>");
                    @class.Interfaces.Clear();
                    @class.Methods.Clear();
                    @class.ImplementsInterface($"IRepositoryBase<TDomain>");

                    var ctor = @class.Constructors.First();
                    ctor.CallsBase(b => b.AddArgument("dbContext"));

                    @class.AddMethod("void", "Add", method =>
                    {
                        method.AddAttribute(CSharpIntentManagedAttribute.Fully());
                        method.AddParameter("TDomain", "entity");
                        method.AddStatement("base.AddAsync(entity).Wait();");
                    });

                    @class.AddMethod("void", "Remove", method =>
                    {
                        method.AddAttribute(CSharpIntentManagedAttribute.Fully());
                        method.AddParameter("TDomain", "entity");
                        method.AddStatement("base.DeleteAsync(entity).Wait();");
                    });

                    @class.AddMethod(
                        $"Task<List<TDomain>>",
                        "FindAllAsync",
                        method =>
                        {
                            method.AddAttribute(CSharpIntentManagedAttribute.Fully());
                            method.Async();
                            method.AddParameter("CancellationToken", "cancellationToken", param => param.WithDefaultValue("default"));
                            method.AddStatement($"return (await ListAsync(cancellationToken)).Cast<TDomain>().ToList();");
                        });

                    @class.AddMethod(
                        $"Task<{template.GetPagedResultInterfaceName()}<TDomain>>",
                        "FindAllAsync",
                        method =>
                        {
                            method.AddAttribute(CSharpIntentManagedAttribute.Fully());
                            method.AddParameter("int", "pageNo")
                                .AddParameter("int", "pageSize")
                                .AddParameter("CancellationToken", "cancellationToken", param => param.WithDefaultValue("default"));
                            method.AddStatement($"return FindAllAsync(filterExpression: x => true, pageNo: pageNo, pageSize: pageSize, cancellationToken: cancellationToken);");
                        });

                    @class.AddMethod(
                        $"Task<{template.GetPagedResultInterfaceName()}<TDomain>>",
                        "FindAllAsync",
                        method =>
                        {
                            method.AddAttribute(CSharpIntentManagedAttribute.Fully());
                            method
                                .AddParameter($"Expression<Func<TDomain, bool>>", "filterExpression")
                                .AddParameter("int", "pageNo")
                                .AddParameter("int", "pageSize")
                                .AddParameter("CancellationToken", "cancellationToken", param => param.WithDefaultValue("default"));
                            method.AddStatement(
                                $"return FindAllAsync(filterExpression: filterExpression, pageNo: pageNo, pageSize: pageSize, linq: x => x, cancellationToken: cancellationToken);");
                        });

                    @class.AddMethod(
                        $"Task<{template.GetPagedResultInterfaceName()}<TDomain>>",
                        "FindAllAsync",
                        method =>
                        {
                            method.AddAttribute(CSharpIntentManagedAttribute.Fully());
                            method.Async();
                            method
                                .AddParameter($"Expression<Func<TDomain, bool>>", "filterExpression")
                                .AddParameter("int", "pageNo")
                                .AddParameter("int", "pageSize")
                                .AddParameter($"Func<IQueryable<TDomain>, IQueryable<TDomain>>", "linq")
                                .AddParameter("CancellationToken", "cancellationToken", param => param.WithDefaultValue("default"));
                            method.AddStatement($"IQueryable<TDomain> queryable = _dbContext.Set<TDomain>();")
                                .AddStatement($"queryable = queryable.Where(filterExpression);")
                                .AddStatement($"var result = linq(queryable);")
                                .AddStatement(new CSharpInvocationStatement($"return await ToPagedListAsync<TDomain>")
                                    .AddArgument("result")
                                    .AddArgument("pageNo")
                                    .AddArgument("pageSize")
                                    .AddArgument("cancellationToken")
                                    .WithArgumentsOnNewLines());
                        });

                    @class.AddMethod("Task<int>", "SaveChangesAsync", method =>
                    {
                        method.AddAttribute(CSharpIntentManagedAttribute.Fully());
                        method.WithComments("// To avoid escalating db locks early, we are not going to perform saving changes on every operation.")
                            .WithComments("// The overall infrastructure will ensure that the DbContext SaveChanges is ultimately invoked.");
                        method.Override();
                        method.AddParameter("CancellationToken", "cancellationToken", param => param.WithDefaultValue("default"));
                        method.AddStatement($"return Task.FromResult(0);");
                    });

                    @class.AddMethod("Task<int>", $"IRepositoryBase<TDomain>.SaveChangesAsync", method =>
                    {
                        method.AddAttribute(CSharpIntentManagedAttribute.Fully());
                        method.WithoutAccessModifier();
                        method.WithComments("// In the event that SaveChanges is invoked explicitly, it should still operate as intended. ");
                        method.AddParameter("CancellationToken", "cancellationToken");
                        method.AddStatement("return base.SaveChangesAsync(cancellationToken);");
                    });

                    @class.AddMethod($"{PagedListInterfaceName(template)}<TDomain>", "FindAllAsync", method =>
                    {
                        method.Async();
                        method.AddParameter($"ISpecification<TDomain>", "specification");
                        method.AddParameter($"int", "pageNo");
                        method.AddParameter($"int", "pageSize");
                        method.AddParameter($"Func<IQueryable<TDomain>, IQueryable<TDomain>>", "queryOptions");
                        method.AddParameter($"CancellationToken", "cancellationToken", p => p.WithDefaultValue("default"));
                        method.AddStatement("var queryable = ApplySpecification(specification);");
                        method.AddStatement("queryable = queryOptions == null ? queryable : queryOptions(queryable);");
                        method.AddStatement(@"return await ToPagedListAsync<TDomain>(
                            queryable,
                            pageNo,
                            pageSize,
                            cancellationToken);");
                    });
                    @class.AddMethod($"{PagedListInterfaceName(template)}<TDomain>", "FindAllAsync", method =>
                    {
                        method.Async();
                        method.AddParameter($"ISpecification<TDomain>", "specification");
                        method.AddParameter($"int", "pageNo");
                        method.AddParameter($"int", "pageSize");
                        method.AddParameter($"CancellationToken", "cancellationToken", p => p.WithDefaultValue("default"));
                        method.AddStatement("return await FindAllAsync(specification, pageNo, pageSize, null, cancellationToken);");
                    });

                    @class.AddMethod($"{PagedListInterfaceName(template)}<T>", "ToPagedListAsync<T>", method =>
                    {
                        method.Private().Static().Async();
                        method.AddParameter($"IQueryable<T>", "queryable");
                        method.AddParameter($"int", "pageNo");
                        method.AddParameter($"int", "pageSize");
                        method.AddParameter($"CancellationToken", "cancellationToken", p => p.WithDefaultValue("default"));
                        method.AddStatement("var count = await queryable.CountAsync(cancellationToken);");
                        method.AddStatement("var skip = ((pageNo - 1) * pageSize);");
                        method.AddStatement(new CSharpMethodChainStatement("var results = await queryable")
                            .AddChainStatement("Skip(skip)")
                            .AddChainStatement("Take(pageSize)")
                            .AddChainStatement("ToListAsync(cancellationToken)"));
                        method.AddStatement($"return new {PagedListClassName(template)}<T>(count, pageNo, pageSize, results);");
                    });

                });
            }
        }

        private void UpdateEntityRepositoryInterface(IApplication application)
        {
            var repoInterfaceTemplates = application.FindTemplateInstances<CSharpTemplateBase<ClassModel>>(TemplateDependency.OnTemplate(EntityRepositoryInterfaceTemplate.TemplateId));
            foreach (var template in repoInterfaceTemplates)
            {
                template.AddNugetDependency(NugetPackages.ArdalisSpecification(template.OutputTarget));

                ((ICSharpFileBuilderTemplate)template).CSharpFile.AfterBuild(file =>
                {
                    var @interface = file.Interfaces.First();
                    var model = @interface.GetMetadata<ClassModel>("model");
                    @interface.Interfaces.Clear();
                    @interface.Methods.Clear();
                    @interface.ExtendsInterface($"IEFRepository<{GetEntityStateName(template)}>");
                    @interface.ExtendsInterface($"IRepositoryBase<{GetEntityStateName(template)}>");
                    @interface.ExtendsInterface(template.GetReadRepositoryInterfaceName(model));
                    file.AddUsing("Ardalis.Specification");
                });
            }
        }

        private void UpdateEntityRepository(IApplication application)
        {
            var entityRepoTemplates = application.FindTemplateInstances<CSharpTemplateBase<ClassModel>>(TemplateDependency.OnTemplate(RepositoryTemplate.TemplateId));
            foreach (var template in entityRepoTemplates)
            {
                template.AddNugetDependency(NugetPackages.ArdalisSpecificationEntityFrameworkCore(template.OutputTarget));

                ((ICSharpFileBuilderTemplate)template).CSharpFile.AfterBuild(file =>
                {
                    file.AddUsing("System");
                    file.AddUsing("System.Collections.Generic");
                    file.AddUsing("System.Linq");
                    file.AddUsing("System.Linq.Expressions");
                    file.AddUsing("System.Threading");
                    file.AddUsing("System.Threading.Tasks");
                    file.AddUsing("Ardalis.Specification");

                    var @class = file.Classes.First();
                    var model = @class.GetMetadata<ClassModel>("model");
                    @class.Interfaces.Clear();
                    @class.Methods.Clear();
                    @class.ImplementsInterface(template.GetEntityRepositoryInterfaceName(model));
                    if (@class.BaseType.Name.Contains($"{GetEntityStateName(template)}, {GetEntityStateName(template)}"))
                    {
                        @class.WithBaseType(@class.BaseType.Name.Replace($", {GetEntityStateName(template)}", ""));
                    }

                    if (HasSinglePrimaryKey(template))
                    {
                        @class.AddMethod($"Task<{GetEntityInterfaceName(template)}?>", "FindByIdAsync", method =>
                        {
                            method.AddAttribute(CSharpIntentManagedAttribute.Fully());
                            method.Async();
                            method.AddParameter(GetSurrogateKey(template), "id")
                                .AddParameter("CancellationToken", "cancellationToken", param => param.WithDefaultValue("default"));
                            method.AddStatement($"return await GetByIdAsync(id: id, cancellationToken: cancellationToken);");
                        });
                        @class.AddMethod($"Task<List<{GetEntityInterfaceName(template)}>>", "FindByIdsAsync", method =>
                        {
                            method.AddAttribute(CSharpIntentManagedAttribute.Fully());
                            method.Async();
                            method.AddParameter($"{GetSurrogateKey(template)}[]", "ids")
                                .AddParameter("CancellationToken", "cancellationToken", param => param.WithDefaultValue("default"));
                            if (GetEntityStateName(template) == GetEntityInterfaceName(template))
                            {
                                method.AddStatement($"return await ListAsync(specification: new FindByIdsSpecification(ids), cancellationToken: cancellationToken);");
                            }
                            else
                            {
                                method.AddStatement(
                                    $"return (await ListAsync(specification: new FindByIdsSpecification(ids), cancellationToken: cancellationToken)).Cast<{GetEntityInterfaceName(template)}>().ToList();");
                            }
                        });

                        @class.AddNestedClass("FindByIdsSpecification", nested =>
                        {
                            nested.AddAttribute(CSharpIntentManagedAttribute.Fully());
                            nested.Private();
                            nested.Sealed();
                            nested.ExtendsClass($"Specification<{GetEntityStateName(template)}>");
                            nested.AddConstructor(ctor =>
                            {
                                ctor.AddParameter($"{GetSurrogateKey(template)}[]", "ids");
                                ctor.AddStatement("Query.Where(p => ids.Contains(p.Id));");
                            });
                        });
                    }
                });
            }
        }

        public static string PagedListInterfaceName(CSharpTemplateBase<object> template) => template.TryGetTypeName(TemplateRoles.Repository.Interface.PagedList, out var name)
            ? name : template.GetTypeName(TemplateRoles.Repository.Interface.PagedResult); // for backward compatibility
        public static string PagedListClassName(CSharpTemplateBase<object> template) => template.GetTypeName(TemplateRoles.Application.Common.PagedList);

        private string GetEntityStateName(CSharpTemplateBase<ClassModel> template) => template.GetTypeName("Domain.Entity", template.Model);

        private string GetEntityInterfaceName(CSharpTemplateBase<ClassModel> template) => template.GetTypeName("Domain.Entity.Interface", template.Model);

        public string GetDbContextName(CSharpTemplateBase<ClassModel> template) => template.TryGetTypeName("Infrastructure.Data.DbContext", out var dbContextName) ? dbContextName : $"{template.Model.Application.Name}DbContext";

        private bool HasSinglePrimaryKey(CSharpTemplateBase<ClassModel> template)
        {
            if (!template.TryGetTemplate<ICSharpFileBuilderTemplate>(TemplateRoles.Domain.Entity.Primary, template.Model, out var entityTemplate))
            {
                return false;
            }

            return entityTemplate.CSharpFile.Classes.First().HasSinglePrimaryKey();
        }

        private string GetSurrogateKey(CSharpTemplateBase<ClassModel> template)
        {
            if (!template.TryGetTemplate<ICSharpFileBuilderTemplate>(TemplateRoles.Domain.Entity.Primary, template.Model, out var entityTemplate))
            {
                return string.Empty;
            }

            return template.UseType(entityTemplate.CSharpFile.Classes.First().GetPropertyWithPrimaryKey().Type);
        }
    }
}