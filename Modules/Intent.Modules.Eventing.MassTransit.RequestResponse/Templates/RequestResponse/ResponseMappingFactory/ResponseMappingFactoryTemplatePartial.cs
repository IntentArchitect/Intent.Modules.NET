using System;
using System.Collections.Generic;
using Intent.Engine;
using Intent.Modelers.Services.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using Intent.Modules.Eventing.MassTransit.RequestResponse.Templates.RequestResponse.MapperResponseMessage;
using Intent.Modules.Eventing.MassTransit.RequestResponse.Templates.RequestResponse.RequestCompletedMessage;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Eventing.MassTransit.RequestResponse.Templates.RequestResponse.ResponseMappingFactory
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class ResponseMappingFactoryTemplate : CSharpTemplateBase<IList<DTOModel>>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.Eventing.MassTransit.RequestResponse.RequestResponse.ResponseMappingFactory";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public ResponseMappingFactoryTemplate(IOutputTarget outputTarget, IList<DTOModel> model) : base(TemplateId, outputTarget, model)
        {
            const string dictReturnType = "Dictionary<Type, Func<object, object>>";
            const string createRequestCompletedMessage = "CreateRequestCompletedMessage";

            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("System")
                .AddUsing("System.Collections.Generic")
                .AddClass($"ResponseMappingFactory", @class =>
                {
                    @class.Static();
                    @class.AddField(dictReturnType, "MappingLookup", field =>
                    {
                        field.PrivateReadOnly().Static();
                        field.WithAssignment(new CSharpStatement("CreateLookup()"));
                    });
                    @class.AddMethod("object", "CreateResponseMessage", method =>
                    {
                        method.AddParameter("object", "originalResponse");
                        method.Static();
                        method.AddStatement(
                            $$"""
                              var responseType = originalResponse.GetType();
                              if (MappingLookup.TryGetValue(responseType, out var predefinedMappingFunc))
                              {
                                  return predefinedMappingFunc(originalResponse);
                              }

                              return {{createRequestCompletedMessage}}(originalResponse);
                              """);
                    });
                    @class.AddMethod(dictReturnType, "CreateLookup", method =>
                    {
                        method.Private().Static();
                        method.AddStatement($"var mappingLookup = new {dictReturnType}();");
                        foreach (var dtoModel in Model)
                        {
                            var sourceType = GetFullyQualifiedTypeName(TemplateRoles.Application.Contracts.Dto, dtoModel);
                            var destType = GetFullyQualifiedTypeName(MapperResponseMessageTemplate.TemplateId, dtoModel);
                            method.AddStatement($"AddMapping<{sourceType}, {destType}>(mappingLookup);");
                        }
                        method.AddStatement("return mappingLookup;");
                    });
                    @class.AddMethod("void", "AddMapping", method =>
                    {
                        method.Private().Static();
                        method.AddGenericParameter("TSource", out var tSource);
                        method.AddGenericParameter("TDest", out var tDest);
                        method.AddParameter(dictReturnType, "mappingLookup");
                        method.AddGenericTypeConstraint(tDest, c => c.AddType("class"));
                        method.AddStatement(
                            $"mappingLookup.Add(typeof({tSource}), originalResponse => {createRequestCompletedMessage}(Activator.CreateInstance(typeof({tDest}), new[] {{ originalResponse }})!));");
                    });
                    @class.AddMethod("object", createRequestCompletedMessage, method =>
                    {
                        method.Private().Static();
                        method.AddParameter("object", "response");
                        method.AddStatement(
                            $$"""
                              var responseType = response.GetType();
                              var genericType = typeof({{this.GetRequestCompletedMessageName()}}<>).MakeGenericType(responseType);
                              var responseInstance = Activator.CreateInstance(genericType, new[] { response })!;
                              return responseInstance;
                              """);
                    });
                });
        }

        public override bool CanRunTemplate()
        {
            return TryGetTemplate<ITemplate>(RequestCompletedMessageTemplate.TemplateId, out var template) && template.CanRunTemplate();
        }

        [IntentManaged(Mode.Fully)]
        public CSharpFile CSharpFile { get; }

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