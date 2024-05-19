using System;
using System.Collections.Generic;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modelers.Services.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.CSharp.VisualStudio;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using Intent.Modules.Eventing.MassTransit.RequestResponse.Templates.RequestResponse;
using Intent.Modules.Eventing.MassTransit.RequestResponse.Templates.RequestResponse.MapperRequestInterface;
using Intent.Modules.Eventing.MassTransit.RequestResponse.Templates.RequestResponse.MapperResponseMessage;
using Intent.Modules.Eventing.MassTransit.RequestResponse.Templates.RequestResponse.RequestCompletedMessage;
using Intent.Modules.Eventing.MassTransit.RequestResponse.Templates.RequestResponse.ResponseMappingFactory;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Eventing.MassTransit.RequestResponse.Templates.RequestResponse.ResponseMappingFactory
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class ResponseMappingFactoryTemplate : CSharpTemplateBase<IList<HybridDtoModel>>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.Eventing.MassTransit.RequestResponse.RequestResponse.ResponseMappingFactory";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public ResponseMappingFactoryTemplate(IOutputTarget outputTarget, IList<HybridDtoModel> model) : base(TemplateId, outputTarget, model)
        {
            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("System")
                .AddUsing("System.Collections.Generic")
                .AddUsing("System.Linq")
                .AddClass("ResponseMappingFactory", @class =>
                {
                    @class.Static();
                    @class.AddMethod("object", "CreateResponseMessage", method =>
                    {
                        method.Static();
                        method.AddParameter("object", "originalRequest");
                        method.AddParameter($"object{NullableEnabled}", "originalResponse");
                        method.AddSwitchStatement("originalRequest", switchStmt =>
                        {
                            switchStmt.AddCase("null", block => block.AddStatement("throw new ArgumentNullException(nameof(originalRequest));"));

                            foreach (var dtoModel in Model)
                            {
                                switchStmt.AddCase(GetFullyQualifiedTypeName(GetRelevantTemplateId(dtoModel), dtoModel.InternalElement), block =>
                                {
                                    switch (dtoModel.TypeReference?.Element)
                                    {
                                        case null:
                                            block.WithReturn($"{this.GetRequestCompletedMessageName()}.Instance");
                                            break;
                                        case var element when element.SpecializationType == "DTO" && !dtoModel.TypeReference.IsCollection:
                                            block.WithReturn($"new {this.GetRequestCompletedMessageName()}<{GetFullyQualifiedTypeName(MapperResponseMessageTemplate.TemplateId, element)}>(new {GetFullyQualifiedTypeName(MapperResponseMessageTemplate.TemplateId, element)}(({GetFullyQualifiedTypeName(TemplateRoles.Application.Contracts.Dto, element)})originalResponse))");
                                            break;
                                        case var element when element.SpecializationType == "DTO" && dtoModel.TypeReference.IsCollection:
                                            block.WithReturn($"new {this.GetRequestCompletedMessageName()}<List<{GetFullyQualifiedTypeName(MapperResponseMessageTemplate.TemplateId, element)}>>(new List<{GetFullyQualifiedTypeName(MapperResponseMessageTemplate.TemplateId, element)}>(((List<{GetFullyQualifiedTypeName(TemplateRoles.Application.Contracts.Dto, element)}>)originalResponse).Select(s => new {GetFullyQualifiedTypeName(MapperResponseMessageTemplate.TemplateId, element)}(s)).ToList()))");
                                            break;
                                        case IElement element:
                                            block.WithReturn($"new {this.GetRequestCompletedMessageName()}<{GetTypeName(element)}>(({GetTypeName(element)})originalResponse)");
                                            break;
                                    }
                                });
                            }

                            switchStmt.AddDefault(block =>
                                block.AddStatement(@"throw new ArgumentOutOfRangeException(originalRequest.GetType().Name, ""Unexpected request type"");"));
                        });
                    });
                });
        }

        private static string GetRelevantTemplateId(IElementWrapper dtoModel)
        {
            return dtoModel.InternalElement.SpecializationType switch
            {
                "Command" => TemplateRoles.Application.Command,
                "Query" => TemplateRoles.Application.Query,
                _ => TemplateRoles.Application.Contracts.Dto
            };
        }

        public override bool CanRunTemplate()
        {
            return TryGetTemplate<ITemplate>(MapperRequestInterfaceTemplate.TemplateId, out var requestTemplate) && requestTemplate.CanRunTemplate();
        }

        private string NullableEnabled => OutputTarget.GetProject().NullableEnabled ? "?" : string.Empty;

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