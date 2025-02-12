//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:8.0.10
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Intent.Modules.AzureFunctions.EntityFrameworkCore.Templates.DbInitializationService {
    using System.Collections.Generic;
    using System.Linq;
    using Intent.Modules.Common;
    using Intent.Modules.Common.Templates;
    using Intent.Modules.Common.CSharp.Templates;
    using Intent.Templates;
    using Intent.Metadata.Models;
    using Intent.Modules.EntityFrameworkCore.Templates;
    using System;
    
    
    public partial class DbInitializationServiceTemplate : CSharpTemplateBase<object> {
        
        public override string TransformText() {
            this.GenerationEnvironment = null;
            
            #line 11 ""
            this.Write(@"using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Description;
using Microsoft.Azure.WebJobs.Host.Config;
using Microsoft.Azure.WebJobs.Hosting;
using Microsoft.Extensions.DependencyInjection;

[assembly: WebJobsStartup(typeof(");
            
            #line default
            #line hidden
            
            #line 18 ""
            this.Write(this.ToStringHelper.ToStringWithCulture( Namespace ));
            
            #line default
            #line hidden
            
            #line 18 ""
            this.Write(".");
            
            #line default
            #line hidden
            
            #line 18 ""
            this.Write(this.ToStringHelper.ToStringWithCulture( ClassName ));
            
            #line default
            #line hidden
            
            #line 18 ""
            this.Write("), \"DbSeeder\")]\r\n\r\n[assembly: DefaultIntentManaged(Mode.Fully)]\r\n\r\nnamespace ");
            
            #line default
            #line hidden
            
            #line 22 ""
            this.Write(this.ToStringHelper.ToStringWithCulture( Namespace ));
            
            #line default
            #line hidden
            
            #line 22 ""
            this.Write("\r\n{\r\n    public class ");
            
            #line default
            #line hidden
            
            #line 24 ""
            this.Write(this.ToStringHelper.ToStringWithCulture( ClassName ));
            
            #line default
            #line hidden
            
            #line 24 ""
            this.Write(@" : IWebJobsStartup
    {
        public void Configure(IWebJobsBuilder builder)
        {
            builder.AddExtension<DbSeedConfigProvider>();
        }
    }

    [Extension(""DbSeed"")]
    internal class DbSeedConfigProvider : IExtensionConfigProvider
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public DbSeedConfigProvider(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        public void Initialize(ExtensionConfigContext context)
        {
            using var scope = _scopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetService<");
            
            #line default
            #line hidden
            
            #line 45 ""
            this.Write(this.ToStringHelper.ToStringWithCulture( this.GetDbContextName() ));
            
            #line default
            #line hidden
            
            #line 45 ""
            this.Write(@">();
            if (dbContext == null)
            {
                throw new InvalidOperationException(""DbContext not configured in Services Collection in order to ensure that the database is created."");
            }

            dbContext.EnsureDbCreated();
        }
    }
}");
            
            #line default
            #line hidden
            return this.GenerationEnvironment.ToString();
        }
        
        public override void Initialize() {
            base.Initialize();
        }
    }
}
