// ------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version: 17.0.0.0
//  
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
// ------------------------------------------------------------------------------
namespace Intent.Modules.Dapr.AspNetCore.StateManagement.Templates.DaprStateStoreGenericRepository
{
    using Intent.Modules.Common.CSharp.Templates;
    using System;
    
    /// <summary>
    /// Class to produce the template output
    /// </summary>
    
    #line 1 "C:\Dev\Intent.Modules.NET\Modules\Intent.Modules.Dapr.AspNetCore.StateManagement\Templates\DaprStateStoreGenericRepository\DaprStateStoreGenericRepositoryTemplate.tt"
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.TextTemplating", "17.0.0.0")]
    public partial class DaprStateStoreGenericRepositoryTemplate : CSharpTemplateBase<object>
    {
#line hidden
        /// <summary>
        /// Create the template output
        /// </summary>
        public override string TransformText()
        {
            this.Write("using System;\r\nusing System.Collections.Concurrent;\r\nusing System.Threading;\r\nusi" +
                    "ng System.Threading.Tasks;\r\nusing Dapr.Client;\r\n\r\n[assembly: DefaultIntentManage" +
                    "d(Mode.Fully)]\r\n\r\nnamespace ");
            
            #line 12 "C:\Dev\Intent.Modules.NET\Modules\Intent.Modules.Dapr.AspNetCore.StateManagement\Templates\DaprStateStoreGenericRepository\DaprStateStoreGenericRepositoryTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Namespace));
            
            #line default
            #line hidden
            this.Write("\r\n{\r\n    public class ");
            
            #line 14 "C:\Dev\Intent.Modules.NET\Modules\Intent.Modules.Dapr.AspNetCore.StateManagement\Templates\DaprStateStoreGenericRepository\DaprStateStoreGenericRepositoryTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(ClassName));
            
            #line default
            #line hidden
            this.Write(" : ");
            
            #line 14 "C:\Dev\Intent.Modules.NET\Modules\Intent.Modules.Dapr.AspNetCore.StateManagement\Templates\DaprStateStoreGenericRepository\DaprStateStoreGenericRepositoryTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(this.GetDaprStateStoreGenericRepositoryInterfaceName()));
            
            #line default
            #line hidden
            this.Write("\r\n    {\r\n        private const string StateStoreName = \"statestore\";\r\n        pri" +
                    "vate readonly DaprClient _daprClient;\r\n        private readonly ");
            
            #line 18 "C:\Dev\Intent.Modules.NET\Modules\Intent.Modules.Dapr.AspNetCore.StateManagement\Templates\DaprStateStoreGenericRepository\DaprStateStoreGenericRepositoryTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(this.GetDaprStateStoreUnitOfWorkName()));
            
            #line default
            #line hidden
            this.Write(" _unitOfWork;\r\n        private readonly ConcurrentQueue<Func<CancellationToken, T" +
                    "ask>> _actions = new();\r\n\r\n        public ");
            
            #line 21 "C:\Dev\Intent.Modules.NET\Modules\Intent.Modules.Dapr.AspNetCore.StateManagement\Templates\DaprStateStoreGenericRepository\DaprStateStoreGenericRepositoryTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(ClassName));
            
            #line default
            #line hidden
            this.Write("(\r\n            DaprClient daprClient,\r\n            ");
            
            #line 23 "C:\Dev\Intent.Modules.NET\Modules\Intent.Modules.Dapr.AspNetCore.StateManagement\Templates\DaprStateStoreGenericRepository\DaprStateStoreGenericRepositoryTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(this.GetDaprStateStoreUnitOfWorkName()));
            
            #line default
            #line hidden
            this.Write(@" unitOfWork)
        {
            _daprClient = daprClient;
            _unitOfWork = unitOfWork;
        }

        public void Upsert<TValue>(string key, TValue state)
        {
            _unitOfWork.Enqueue(async cancellationToken =>
            {
                var currentState = await _daprClient.GetStateEntryAsync<TValue>(StateStoreName, key, cancellationToken: cancellationToken);
                currentState.Value = state;
                await currentState.SaveAsync(cancellationToken: cancellationToken);
            });
        }

        public async Task<TValue> GetAsync<TValue>(string key, CancellationToken cancellationToken = default)
        {
            return await _daprClient.GetStateAsync<TValue>(StateStoreName, key, cancellationToken: cancellationToken);
        }

        public void Delete(string key)
        {
            _unitOfWork.Enqueue(async cancellationToken =>
            {
                await _daprClient.DeleteStateAsync(StateStoreName, key, cancellationToken: cancellationToken);
            });
        }

        public ");
            
            #line 52 "C:\Dev\Intent.Modules.NET\Modules\Intent.Modules.Dapr.AspNetCore.StateManagement\Templates\DaprStateStoreGenericRepository\DaprStateStoreGenericRepositoryTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(this.GetDaprStateStoreUnitOfWorkInterfaceName()));
            
            #line default
            #line hidden
            this.Write(" UnitOfWork => _unitOfWork;\r\n    }\r\n}");
            return this.GenerationEnvironment.ToString();
        }
    }
    
    #line default
    #line hidden
}
