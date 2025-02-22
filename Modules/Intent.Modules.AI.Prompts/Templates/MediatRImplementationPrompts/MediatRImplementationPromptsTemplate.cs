using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Intent.Metadata.Models;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.FileBuilders;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using Intent.RoslynWeaver.Attributes;
using Intent.Modules.Modelers.Services.DomainInteractions;
using Intent.Modelers.Services.DomainInteractions.Api;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FileTemplateStringInterpolation", Version = "1.0")]

namespace Intent.Modules.AI.Prompts.Templates.MediatRImplementationPrompts
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class MediatRImplementationPromptsTemplate
    {
        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public override string TransformText()
        {
            var queryTemplate = GetTemplate<ICSharpFileBuilderTemplate>(TemplateRoles.Application.Query, Model);
            var queryHandlerTemplate = GetTemplate<ICSharpFileBuilderTemplate>(TemplateRoles.Application.Handler.Query, Model);
            var queriedEntity = Model.QueryEntityActions().FirstOrDefault()?.TypeReference.Element.AsClassModel();

            return @$"
I'm going to provide you with a C# class called {queryHandlerTemplate.ClassName} that is a handler for a MediatR query. 
I want you to implement the Handle method to fetch out the {queryHandlerTemplate.ClassName.RemovePrefix("Get").RemoveSuffix("Query", "Handler").ToSentenceCase()} appropriately using best practices.

I want you to return the updated handler class, {queryHandlerTemplate.ClassName}, and only that class. Do not give my anything but just the code. No explanation.

You can inject in any entity repository and/or the IApplicationDbContext which is an interface over the EF Core DbContect.
You must keep all attributes unchanged on the class or its methods.

The architecture is using EF Core and a Repository pattern. There is a repository interface for every entity You can inject in the repository and make a call to fetch out the statistics about the Buyer.

The current implementation of the {queryHandlerTemplate.ClassName} class is as follows:
{queryHandlerTemplate.CSharpFile.Classes.First().ToString()}

The Query payload is as follows:
{queryTemplate.CSharpFile.Classes.First().ToString()}

{GetRelatedEntities(queriedEntity)}

Your base repository interface is as follows:
public interface IEFRepository<TDomain, TPersistence> : IRepository<TDomain>
{{
    IUnitOfWork UnitOfWork {{ get; }}
    Task<TDomain?> FindAsync(Expression<Func<TPersistence, bool>> filterExpression, CancellationToken cancellationToken = default);
    Task<TDomain?> FindAsync(Expression<Func<TPersistence, bool>> filterExpression, Func<IQueryable<TPersistence>, IQueryable<TPersistence>> queryOptions, CancellationToken cancellationToken = default);
    Task<TDomain?> FindAsync(Func<IQueryable<TPersistence>, IQueryable<TPersistence>> queryOptions, CancellationToken cancellationToken = default);
    Task<List<TDomain>> FindAllAsync(CancellationToken cancellationToken = default);
    Task<List<TDomain>> FindAllAsync(Expression<Func<TPersistence, bool>> filterExpression, CancellationToken cancellationToken = default);
    Task<List<TDomain>> FindAllAsync(Expression<Func<TPersistence, bool>> filterExpression, Func<IQueryable<TPersistence>, IQueryable<TPersistence>> queryOptions, CancellationToken cancellationToken = default);
    Task<IPagedList<TDomain>> FindAllAsync(int pageNo, int pageSize, CancellationToken cancellationToken = default);
    Task<IPagedList<TDomain>> FindAllAsync(Expression<Func<TPersistence, bool>> filterExpression, int pageNo, int pageSize, CancellationToken cancellationToken = default);
    Task<IPagedList<TDomain>> FindAllAsync(Expression<Func<TPersistence, bool>> filterExpression, int pageNo, int pageSize, Func<IQueryable<TPersistence>, IQueryable<TPersistence>> queryOptions, CancellationToken cancellationToken = default);
    Task<List<TDomain>> FindAllAsync(Func<IQueryable<TPersistence>, IQueryable<TPersistence>> queryOptions, CancellationToken cancellationToken = default);
    Task<IPagedList<TDomain>> FindAllAsync(int pageNo, int pageSize, Func<IQueryable<TPersistence>, IQueryable<TPersistence>> queryOptions, CancellationToken cancellationToken = default);
    Task<int> CountAsync(Expression<Func<TPersistence, bool>> filterExpression, CancellationToken cancellationToken = default);
    Task<int> CountAsync(Func<IQueryable<TPersistence>, IQueryable<TPersistence>>? queryOptions = default, CancellationToken cancellationToken = default);
    Task<bool> AnyAsync(Expression<Func<TPersistence, bool>> filterExpression, CancellationToken cancellationToken = default);
    Task<bool> AnyAsync(Func<IQueryable<TPersistence>, IQueryable<TPersistence>>? queryOptions = default, CancellationToken cancellationToken = default);
    Task<List<TProjection>> FindAllProjectToAsync<TProjection>(CancellationToken cancellationToken = default);
    Task<List<TProjection>> FindAllProjectToAsync<TProjection>(Func<IQueryable<TPersistence>, IQueryable<TPersistence>> queryOptions, CancellationToken cancellationToken = default);
    Task<List<TProjection>> FindAllProjectToAsync<TProjection>(Expression<Func<TPersistence, bool>> filterExpression, CancellationToken cancellationToken = default);
    Task<List<TProjection>> FindAllProjectToAsync<TProjection>(Expression<Func<TPersistence, bool>> filterExpression, Func<IQueryable<TPersistence>, IQueryable<TPersistence>> queryOptions, CancellationToken cancellationToken = default);
    Task<IPagedList<TProjection>> FindAllProjectToAsync<TProjection>(int pageNo, int pageSize, CancellationToken cancellationToken = default);
    Task<IPagedList<TProjection>> FindAllProjectToAsync<TProjection>(int pageNo, int pageSize, Func<IQueryable<TPersistence>, IQueryable<TPersistence>> queryOptions, CancellationToken cancellationToken = default);
    Task<IPagedList<TProjection>> FindAllProjectToAsync<TProjection>(Expression<Func<TPersistence, bool>> filterExpression, int pageNo, int pageSize, CancellationToken cancellationToken = default);
    Task<IPagedList<TProjection>> FindAllProjectToAsync<TProjection>(Expression<Func<TPersistence, bool>> filterExpression, int pageNo, int pageSize, Func<IQueryable<TPersistence>, IQueryable<TPersistence>> queryOptions, CancellationToken cancellationToken = default);
    Task<TProjection?> FindProjectToAsync<TProjection>(Expression<Func<TPersistence, bool>> filterExpression, CancellationToken cancellationToken = default);
    Task<TProjection?> FindProjectToAsync<TProjection>(Expression<Func<TPersistence, bool>> filterExpression, Func<IQueryable<TPersistence>, IQueryable<TPersistence>> queryOptions, CancellationToken cancellationToken = default);
    Task<TProjection?> FindProjectToAsync<TProjection>(Func<IQueryable<TPersistence>, IQueryable<TPersistence>> queryOptions, CancellationToken cancellationToken = default);
}}

The Buyer repository is as follows:
namespace DSF.Orders.Domain.Repositories
{{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface IBuyerRepository : IEFRepository<Buyer, Buyer>
    {{
        [IntentManaged(Mode.Fully)]
        Task<TProjection?> FindByIdProjectToAsync<TProjection>(Guid id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<Buyer?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<Buyer>> FindByIdsAsync(Guid[] ids, CancellationToken cancellationToken = default);
    }}
}}

The Order repository is as follows:
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface IOrderRepository : IEFRepository<Order, Order>
    {{
        [IntentManaged(Mode.Fully)]
        Task<TProjection?> FindByIdProjectToAsync<TProjection>(Guid id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<Order?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<Order>> FindByIdsAsync(Guid[] ids, CancellationToken cancellationToken = default);
    }}

The GetBuyerStatisticsQueryHandler returns the following DTO:
    public class BuyerStatisticsDto
    {{
        public BuyerStatisticsDto()
        {{
        }}

        public int NoOfOrder {{ get; set; }}
        public decimal AverageCartValue {{ get; set; }}

        public static BuyerStatisticsDto Create(int noOfOrder, decimal averageCartValue)
        {{
            return new BuyerStatisticsDto
            {{
                NoOfOrder = noOfOrder,
                AverageCartValue = averageCartValue
            }};
        }}
    }}";
        }

        private string GetRelatedEntities(ClassModel queriedEntity)
        {
            if (queriedEntity == null)
            {
                return string.Empty;
            }
            var sb = new StringBuilder();
            var relatedClasses = new[] { queriedEntity }.Concat(queriedEntity.AssociatedClasses.Where(x => x.Class != null).Select(x => x.Class));
            foreach (var relatedClass in relatedClasses)
            {
                var entityTemplate = GetTemplate<ICSharpFileBuilderTemplate>(TemplateRoles.Domain.Entity.Primary, relatedClass);
                sb.AppendLine($@"
The {relatedClass.Name} is as follows:
{entityTemplate.CSharpFile.Classes.First()}");
            }

            return sb.ToString();
        }
    }
}