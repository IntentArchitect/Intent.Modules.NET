﻿// ------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version: 17.0.0.0
//  
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
// ------------------------------------------------------------------------------
namespace Intent.Modules.EntityFramework.Repositories.Templates.RepositoryBase
{
    using Intent.Modelers.Domain.Api;
    using Intent.Modules.Common.Templates;
    using Intent.Modules.Common.CSharp.Templates;
    using System;
    using System.IO;
    using System.Diagnostics;
    using System.Linq;
    using System.Collections;
    using System.Collections.Generic;
    using Intent.Modules.Entities.Repositories.Api.Templates;
    
    /// <summary>
    /// Class to produce the template output
    /// </summary>
    
    #line 1 "C:\Dev\Intent.Modules.NET\Modules\Intent.Modules.EntityFramework.Repositories\Templates\RepositoryBase\RepositoryBaseTemplate.tt"
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.TextTemplating", "17.0.0.0")]
    public partial class RepositoryBaseTemplate : CSharpTemplateBase
    {
#line hidden
        /// <summary>
        /// Create the template output
        /// </summary>
        public override string TransformText()
        {
            this.Write("\n");
            this.Write("using System;\r\nusing System.Collections.Generic;\r\nusing System.Data.Entity;\r\nusin" +
                    "g System.Linq;\r\nusing System.Linq.Expressions;\r\nusing System.Threading;\r\nusing S" +
                    "ystem.Threading.Tasks;\r\n\r\n[assembly: DefaultIntentManaged(Mode.Fully)]\r\n\r\nnamesp" +
                    "ace ");
            
            #line 24 "C:\Dev\Intent.Modules.NET\Modules\Intent.Modules.EntityFramework.Repositories\Templates\RepositoryBase\RepositoryBaseTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Namespace));
            
            #line default
            #line hidden
            this.Write("\r\n{\r\n    public class ");
            
            #line 26 "C:\Dev\Intent.Modules.NET\Modules\Intent.Modules.EntityFramework.Repositories\Templates\RepositoryBase\RepositoryBaseTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(ClassName));
            
            #line default
            #line hidden
            this.Write("<TDomain, TPersistence, TDbContext> : ");
            
            #line 26 "C:\Dev\Intent.Modules.NET\Modules\Intent.Modules.EntityFramework.Repositories\Templates\RepositoryBase\RepositoryBaseTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(RepositoryInterfaceName));
            
            #line default
            #line hidden
            this.Write("<TDomain, TPersistence>\r\n        where TDbContext : DbContext, ");
            
            #line 27 "C:\Dev\Intent.Modules.NET\Modules\Intent.Modules.EntityFramework.Repositories\Templates\RepositoryBase\RepositoryBaseTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(this.GetUnitOfWorkInterfaceName()));
            
            #line default
            #line hidden
            this.Write("\r\n        where TPersistence : class, TDomain\r\n        where TDomain : class\r\n   " +
                    " {\r\n        private readonly TDbContext _dbContext;\r\n\r\n        public ");
            
            #line 33 "C:\Dev\Intent.Modules.NET\Modules\Intent.Modules.EntityFramework.Repositories\Templates\RepositoryBase\RepositoryBaseTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(ClassName));
            
            #line default
            #line hidden
            this.Write("(TDbContext dbContext)\r\n        {\r\n            _dbContext = dbContext ?? throw ne" +
                    "w ArgumentNullException(nameof(dbContext));\r\n        }\r\n\r\n        public ");
            
            #line 38 "C:\Dev\Intent.Modules.NET\Modules\Intent.Modules.EntityFramework.Repositories\Templates\RepositoryBase\RepositoryBaseTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(this.GetUnitOfWorkInterfaceName()));
            
            #line default
            #line hidden
            this.Write(" UnitOfWork => _dbContext;\r\n\r\n        public virtual void Remove(TDomain entity)\r" +
                    "\n        {\r\n            GetSet().Remove((TPersistence)entity);\r\n        }\r\n\r\n   " +
                    "     public virtual void Add(TDomain entity)\r\n        {\r\n            GetSet().Ad" +
                    "d((TPersistence)entity);\r\n        }\r\n\r\n        public Task<int> SaveChangesAsync" +
                    "()\r\n        {\r\n            return _dbContext.SaveChangesAsync();\r\n        }\r\n\r\n " +
                    "       public Task<int> SaveChangesAsync(CancellationToken cancellationToken)\r\n " +
                    "       {\r\n            return _dbContext.SaveChangesAsync(cancellationToken);\r\n  " +
                    "      }\r\n\r\n        public virtual async Task<TDomain> FindAsync(Expression<Func<" +
                    "TPersistence, bool>> filterExpression, CancellationToken cancellationToken = def" +
                    "ault)\r\n        {\r\n            return await QueryInternal(filterExpression).Singl" +
                    "eOrDefaultAsync<TDomain>(cancellationToken);\r\n        }\r\n\r\n        public virtua" +
                    "l async Task<List<TDomain>> FindAllAsync(CancellationToken cancellationToken = d" +
                    "efault)\r\n        {\r\n            return await QueryInternal(x => true).ToListAsyn" +
                    "c<TDomain>(cancellationToken);\r\n        }\r\n        \r\n        public virtual asyn" +
                    "c Task<List<TDomain>> FindAllAsync(Expression<Func<TPersistence, bool>> filterEx" +
                    "pression, CancellationToken cancellationToken = default)\r\n        {\r\n           " +
                    " return await QueryInternal(filterExpression).ToListAsync<TDomain>(cancellationT" +
                    "oken);\r\n        }\r\n\r\n        \r\n        public virtual async Task<List<TDomain>> " +
                    "FindAllAsync(Expression<Func<TPersistence, bool>> filterExpression, Func<IQuerya" +
                    "ble<TPersistence>, IQueryable<TPersistence>> linq, CancellationToken cancellatio" +
                    "nToken = default)\r\n        {\r\n            return await QueryInternal(filterExpre" +
                    "ssion, linq).ToListAsync<TDomain>(cancellationToken);\r\n        }\r\n\r\n        publ" +
                    "ic virtual async Task<IPagedResult<TDomain>> FindAllAsync(int pageNo, int pageSi" +
                    "ze, CancellationToken cancellationToken = default)\r\n        {\r\n            var q" +
                    "uery = QueryInternal(x => true);\r\n            return await ");
            
            #line 84 "C:\Dev\Intent.Modules.NET\Modules\Intent.Modules.EntityFramework.Repositories\Templates\RepositoryBase\RepositoryBaseTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(PagedListClassName));
            
            #line default
            #line hidden
            this.Write(@"<TDomain>.CreateAsync(
                query,
                pageNo,
                pageSize);
        }
        
        public virtual async Task<IPagedResult<TDomain>> FindAllAsync(Expression<Func<TPersistence, bool>> filterExpression, int pageNo, int pageSize, CancellationToken cancellationToken = default)
        {
            var query = QueryInternal(filterExpression);
            return await ");
            
            #line 93 "C:\Dev\Intent.Modules.NET\Modules\Intent.Modules.EntityFramework.Repositories\Templates\RepositoryBase\RepositoryBaseTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(PagedListClassName));
            
            #line default
            #line hidden
            this.Write(@"<TDomain>.CreateAsync(
                query,
                pageNo,
                pageSize,
                cancellationToken);
        }

                public virtual async Task<IPagedResult<TDomain>> FindAllAsync(Expression<Func<TPersistence, bool>> filterExpression, int pageNo, int pageSize, Func<IQueryable<TPersistence>, IQueryable<TPersistence>> linq, CancellationToken cancellationToken = default)
        {
            var query = QueryInternal(filterExpression, linq);
            return await ");
            
            #line 103 "C:\Dev\Intent.Modules.NET\Modules\Intent.Modules.EntityFramework.Repositories\Templates\RepositoryBase\RepositoryBaseTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(PagedListClassName));
            
            #line default
            #line hidden
            this.Write("<TDomain>.CreateAsync(\r\n                query,\r\n                pageNo,\r\n        " +
                    "        pageSize,\r\n                cancellationToken);\r\n        }\r\n\r\n        pub" +
                    "lic virtual async Task<int> CountAsync(Expression<Func<TPersistence, bool>> filt" +
                    "erExpression, CancellationToken cancellationToken = default)\r\n        {\r\n       " +
                    "     return await QueryInternal(filterExpression).CountAsync(cancellationToken);" +
                    "\r\n        }\r\n\r\n        public bool Any(Expression<Func<TPersistence, bool>> filt" +
                    "erExpression)\r\n        {\r\n            return QueryInternal(filterExpression).Any" +
                    "();\r\n        }\r\n\r\n        public virtual async Task<bool> AnyAsync(Expression<Fu" +
                    "nc<TPersistence, bool>> filterExpression, CancellationToken cancellationToken = " +
                    "default)\r\n        {\r\n            return await QueryInternal(filterExpression).An" +
                    "yAsync(cancellationToken);\r\n        }\r\n\r\n        protected virtual IQueryable<TP" +
                    "ersistence> QueryInternal(Expression<Func<TPersistence, bool>> filterExpression)" +
                    "\r\n        {\r\n            var queryable = CreateQuery();\r\n            if (filterE" +
                    "xpression != null)\r\n            {\r\n                queryable = queryable.Where(f" +
                    "ilterExpression);\r\n            }\r\n            return queryable;\r\n        }\r\n\r\n  " +
                    "      protected virtual IQueryable<TResult> QueryInternal<TResult>(Expression<Fu" +
                    "nc<TPersistence, bool>> filterExpression, Func<IQueryable<TPersistence>, IQuerya" +
                    "ble<TResult>> linq)\r\n        {\r\n            var queryable = CreateQuery();\r\n    " +
                    "        queryable = queryable.Where(filterExpression);\r\n\r\n            var result" +
                    " = linq(queryable);\r\n            return result;\r\n        }\r\n\r\n        protected " +
                    "virtual IQueryable<TPersistence> CreateQuery()\r\n        {\r\n            return Ge" +
                    "tSet();\r\n        }\r\n\r\n        protected virtual DbSet<TPersistence> GetSet()\r\n  " +
                    "      {\r\n            return _dbContext.Set<TPersistence>();\r\n        }\r\n    }\r\n}" +
                    "\r\n");
            return this.GenerationEnvironment.ToString();
        }
    }
    
    #line default
    #line hidden
}
