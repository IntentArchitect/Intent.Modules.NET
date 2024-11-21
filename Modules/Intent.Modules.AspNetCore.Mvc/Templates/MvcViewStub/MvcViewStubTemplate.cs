using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FileTemplateStringInterpolation", Version = "1.0")]

namespace Intent.Modules.AspNetCore.Mvc.Templates.MvcViewStub
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class MvcViewStubTemplate
    {
        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public override string TransformText()
        {
            return $$"""
                   @model {{GetModelType()}}
                   
                   @{
                       Layout = null;
                   }
                   
                   <!DOCTYPE html>
                   
                   <html>
                   <head>
                       <title>title</title>
                   </head>
                   <body>
                   <div>
                         
                   </div>
                   </body>
                   </html>
                   """;
        }
    }
}