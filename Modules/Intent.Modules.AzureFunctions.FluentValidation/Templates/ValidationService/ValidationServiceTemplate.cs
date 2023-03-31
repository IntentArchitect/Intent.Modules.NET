// ------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version: 17.0.0.0
//  
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
// ------------------------------------------------------------------------------
namespace Intent.Modules.AzureFunctions.FluentValidation.Templates.ValidationService
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
    
    #line 1 "C:\Dev\Intent.Modules.NET\Modules\Intent.Modules.AzureFunctions.FluentValidation\Templates\ValidationService\ValidationServiceTemplate.tt"
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.TextTemplating", "17.0.0.0")]
    public partial class ValidationServiceTemplate : CSharpTemplateBase<object>
    {
#line hidden
        /// <summary>
        /// Create the template output
        /// </summary>
        public override string TransformText()
        {
            this.Write(@"using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

[assembly: DefaultIntentManaged(Mode.Fully)]

namespace ");
            
            #line 20 "C:\Dev\Intent.Modules.NET\Modules\Intent.Modules.AzureFunctions.FluentValidation\Templates\ValidationService\ValidationServiceTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(Namespace));
            
            #line default
            #line hidden
            this.Write("\r\n{\r\n    public class ");
            
            #line 22 "C:\Dev\Intent.Modules.NET\Modules\Intent.Modules.AzureFunctions.FluentValidation\Templates\ValidationService\ValidationServiceTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(ClassName));
            
            #line default
            #line hidden
            this.Write(" : ");
            
            #line 22 "C:\Dev\Intent.Modules.NET\Modules\Intent.Modules.AzureFunctions.FluentValidation\Templates\ValidationService\ValidationServiceTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(this.GetValidationServiceInterfaceName()));
            
            #line default
            #line hidden
            this.Write("\r\n    {\r\n        private readonly IServiceProvider _serviceProvider;\r\n\r\n        p" +
                    "ublic ");
            
            #line 26 "C:\Dev\Intent.Modules.NET\Modules\Intent.Modules.AzureFunctions.FluentValidation\Templates\ValidationService\ValidationServiceTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(ClassName));
            
            #line default
            #line hidden
            this.Write(@"(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task Validate<TRequest>(TRequest request, CancellationToken cancellationToken = default)
        {
            var validators = _serviceProvider.GetService<IEnumerable<IValidator<TRequest>>>();
            if (validators.Any())
            {
                var context = new ValidationContext<TRequest>(request);

                var validationResults = await Task.WhenAll(validators.Select(v => v.ValidateAsync(context, cancellationToken)));
                var failures = validationResults.SelectMany(r => r.Errors).Where(f => f != null).ToList();

                if (failures.Count != 0)
                    throw new ValidationException(failures);
            }
        }
    }
}");
            return this.GenerationEnvironment.ToString();
        }
    }
    
    #line default
    #line hidden
}
