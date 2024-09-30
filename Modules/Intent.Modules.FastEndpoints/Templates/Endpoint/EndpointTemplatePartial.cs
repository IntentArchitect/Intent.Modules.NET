using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Modelers.Services.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Metadata.WebApi.Models;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.FastEndpoints.Templates.Endpoint
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class EndpointTemplate : CSharpTemplateBase<IEndpointModel>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.FastEndpoints.EndpointTemplate";

        private CSharpClass? _requestModelClass;

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public EndpointTemplate(IOutputTarget outputTarget, IEndpointModel model = null) : base(TemplateId, outputTarget, model)
        {
            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("System")
                .AddUsing("System.Threading")
                .AddUsing("System.Threading.Tasks")
                .AddUsing("FastEndpoints");

            AddRequestModelIfApplicable();
            AddEndpointClass();
        }

        private void AddRequestModelIfApplicable()
        {
            _requestModelClass = new CSharpClass($"{Model.Name.RemoveSuffix("Endpoint")}RequestModel", CSharpFile);
            CSharpFile.OnBuild(file =>
            {
                file.TypeDeclarations.Add(_requestModelClass);
                foreach (var parameter in Model.Parameters)
                {
                    _requestModelClass.AddProperty(GetTypeName(parameter.TypeReference), parameter.Name.ToPropertyName(), prop =>
                    {
                        var attr = GetParameterBindingAttribute(parameter, prop);
                        if (attr is not null)
                        {
                            prop.AddAttribute(attr);
                        }
                    });
                }
            }, 1);
        }

        private void AddEndpointClass()
        {
            CSharpFile.AddClass($"{Model.Name.RemoveSuffix("Endpoint")}Endpoint", @class =>
            {
                @class.AddMethod("void", "Configure", method =>
                {
                    method.Override();
                    AddHttpVerbAndRoute(method);

                    method.AddStatement("AllowAnonymous();");
                });

                @class.AddMethod("Task", "HandleAsync", method =>
                {
                    method.Override().Async();
                    method.AddParameter(_requestModelClass.Name, "req");
                    method.AddParameter("CancellationToken", "ct");
                });
            });
        }

        private void AddHttpVerbAndRoute(CSharpClassMethod method)
        {
            var verb = Model.Verb switch
            {
                HttpVerb.Get => "Get",
                HttpVerb.Post => "Post",
                HttpVerb.Delete => "Delete",
                HttpVerb.Put => "Put",
                HttpVerb.Patch => "Patch",
                _ => throw new NotSupportedException($"Verb {Model.Verb} is not supported.")
            };
            method.AddStatement(new CSharpInvocationStatement(verb)
                .AddArgument($@"""{Model.Route}"""));
        }

        private CSharpAttribute? GetParameterBindingAttribute(IEndpointParameterModel parameter, CSharpProperty prop)
        {
            if (parameter.TypeReference.Element.IsDTOModel() &&
                parameter.Source is null or HttpInputSource.FromBody)
            {
                return new CSharpAttribute("FromBody");
            }

            if (parameter.Source is null or HttpInputSource.FromRoute &&
                Model.Route.Contains($"{{{parameter.Name}}}", StringComparison.OrdinalIgnoreCase))
            {
                return new CSharpAttribute("FromRoute");
            }

            return null;
        }

        [IntentManaged(Mode.Fully)] public CSharpFile CSharpFile { get; }

        [IntentManaged(Mode.Fully)]
        protected override CSharpFileConfig DefineFileConfig()
        {
            return CSharpFile.GetConfig();
        }

        [IntentManaged(Mode.Fully)]
        public override string TransformText()
        {
            return CSharpFile.ToString();
        }
    }
}