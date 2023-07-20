// ------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version: 17.0.0.0
//  
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
// ------------------------------------------------------------------------------
namespace Intent.Modules.Application.MediatR.Behaviours.Templates.PerformanceBehaviour
{
    using System.Collections.Generic;
    using System.Linq;
    using Intent.Modules.Common;
    using Intent.Modules.Common.Templates;
    using Intent.Modules.Common.CSharp.Templates;
    using Intent.Templates;
    using Intent.Metadata.Models;
    using System;
    
    /// <summary>
    /// Class to produce the template output
    /// </summary>
    
    #line 1 "C:\Dev\Intent.Modules.NET\Modules\Intent.Modules.Application.MediatR.Behaviours\Templates\PerformanceBehaviour\PerformanceBehaviourTemplate.tt"
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.TextTemplating", "17.0.0.0")]
    public partial class PerformanceBehaviourTemplate : CSharpTemplateBase<object>
    {
#line hidden
        /// <summary>
        /// Create the template output
        /// </summary>
        public override string TransformText()
        {
            this.Write("using MediatR;\r\nusing Microsoft.Extensions.Logging;\r\nusing System.Diagnostics;\r\nu" +
                    "sing System.Threading;\r\nusing System.Threading.Tasks;\r\n\r\n[assembly: DefaultInten" +
                    "tManaged(Mode.Fully)]\r\n\r\nnamespace ");
            
            #line 18 "C:\Dev\Intent.Modules.NET\Modules\Intent.Modules.Application.MediatR.Behaviours\Templates\PerformanceBehaviour\PerformanceBehaviourTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Namespace));
            
            #line default
            #line hidden
            this.Write("\r\n{\r\n    public class ");
            
            #line 20 "C:\Dev\Intent.Modules.NET\Modules\Intent.Modules.Application.MediatR.Behaviours\Templates\PerformanceBehaviour\PerformanceBehaviourTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(ClassName));
            
            #line default
            #line hidden
            this.Write("<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>\r\n        where TRe" +
                    "quest : IRequest<TResponse>\r\n    {\r\n        private readonly Stopwatch _timer;\r\n" +
                    "        private readonly ILogger<TRequest> _logger;\r\n        private readonly ");
            
            #line 25 "C:\Dev\Intent.Modules.NET\Modules\Intent.Modules.Application.MediatR.Behaviours\Templates\PerformanceBehaviour\PerformanceBehaviourTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(this.GetCurrentUserServiceInterface()));
            
            #line default
            #line hidden
            this.Write(" _currentUserService;\r\n\r\n        public ");
            
            #line 27 "C:\Dev\Intent.Modules.NET\Modules\Intent.Modules.Application.MediatR.Behaviours\Templates\PerformanceBehaviour\PerformanceBehaviourTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(ClassName));
            
            #line default
            #line hidden
            this.Write("(\r\n            ILogger<TRequest> logger, \r\n            ");
            
            #line 29 "C:\Dev\Intent.Modules.NET\Modules\Intent.Modules.Application.MediatR.Behaviours\Templates\PerformanceBehaviour\PerformanceBehaviourTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(this.GetCurrentUserServiceInterface()));
            
            #line default
            #line hidden
            this.Write(@" currentUserService)
        {
            _timer = new Stopwatch();

            _logger = logger;
            _currentUserService = currentUserService;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            _timer.Start();

            var response = await next();

            _timer.Stop();

            var elapsedMilliseconds = _timer.ElapsedMilliseconds;

            if (elapsedMilliseconds > 500)
            {
                var requestName = typeof(TRequest).Name;
                var userId = _currentUserService.UserId ?? string.Empty;
                var userName = _currentUserService.UserName ?? string.Empty;

                _logger.LogWarning(""");
            
            #line 53 "C:\Dev\Intent.Modules.NET\Modules\Intent.Modules.Application.MediatR.Behaviours\Templates\PerformanceBehaviour\PerformanceBehaviourTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(ExecutionContext.GetApplicationConfig().Name));
            
            #line default
            #line hidden
            this.Write(" Long Running Request: {Name} ({ElapsedMilliseconds} milliseconds) {@UserId} {@Us" +
                    "erName} {@Request}\",\r\n                    requestName, elapsedMilliseconds, user" +
                    "Id, userName, request);\r\n            }\r\n\r\n            return response;\r\n        " +
                    "}\r\n    }\r\n}");
            return this.GenerationEnvironment.ToString();
        }
    }
    
    #line default
    #line hidden
}
