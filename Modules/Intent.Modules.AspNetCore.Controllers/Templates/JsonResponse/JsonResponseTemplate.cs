using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly:IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpStringInterpolationTemplate",Version= "1.0")]

namespace Intent.Modules.AspNetCore.Controllers.Templates.JsonResponse
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class JsonResponseTemplate
    {
        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public override string TransformText()
        {
            return $$"""
                     [assembly: DefaultIntentManaged(Mode.Fully)]
                     
                     namespace {{Namespace}}
                     {
                         /// <summary>
                         /// Implicit wrapping of types that serialize to non-complex values.
                         /// </summary>
                         /// <typeparam name="T">Types such as string, Guid, int, long, etc.</typeparam>
                         public class {{ClassName}}<T>
                         {
                             public {{ClassName}}(T value)
                             {
                                 Value = value;
                             }
                     
                             public T Value { get; set; }
                         }
                     }
                     """;
        }
    }
}