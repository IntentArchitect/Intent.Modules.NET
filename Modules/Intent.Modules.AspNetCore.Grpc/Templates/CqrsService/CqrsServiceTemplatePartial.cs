using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Metadata.Security.Api;
using Intent.Modules.AspNetCore.Grpc.Templates.MessageProtoFile;
using Intent.Modules.AspNetCore.Grpc.Templates.ServiceProtoFile;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Metadata.Security.Models;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.AspNetCore.Grpc.Templates.CqrsService
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class CqrsServiceTemplate : CSharpTemplateBase<IElement>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.AspNetCore.Grpc.CqrsService";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public CqrsServiceTemplate(IOutputTarget outputTarget, IElement model = null) : base(TemplateId, outputTarget, model)
        {
            var name = Model.Name.ToPascalCase();

            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("MediatR")
                .AddClass(name.EnsureSuffixedWith("Service"), @class =>
                {
                    var (serviceAuthAttributes, authAttributesByEndpoint) = this.GetAuthorizationAttributes(Model, Model.ChildElements.Where(IsApplicable));

                    foreach (var attribute in serviceAuthAttributes)
                    {
                        @class.AddAttribute(attribute);
                    }

                    @class.WithBaseType($"{UseType($"{ServiceProtoFileTemplateInstance.CSharpNamespace}.{name}")}.{name}Base");

                    @class.AddConstructor(ctor =>
                    {
                        ctor.AddParameter("ISender", "mediator", param =>
                        {
                            AddUsing("System");
                            param.IntroduceReadonlyField((_, s) => s.ThrowArgumentNullException());
                        });
                    });

                    foreach (var childElement in Model.ChildElements)
                    {
                        if (!IsApplicable(childElement))
                        {
                            continue;
                        }

                        @class.AddMethod(this.MapToOperationReturnType(childElement.TypeReference), childElement.Name.RemoveSuffix("Command", "Query"), method =>
                        {
                            foreach (var attribute in authAttributesByEndpoint[childElement])
                            {
                                method.AddAttribute(attribute);
                            }

                            var dtoTemplate = GrpcTypeResolverHelper.GetTemplateInstance<MessageProtoFileTemplate>(this, MessageProtoFileTemplate.TemplateId, childElement.Id);

                            method.Override();
                            method.Async();
                            method.AddParameter(UseType($"{dtoTemplate.CSharpNamespace}.{childElement.Name.ToPascalCase()}"), "request");
                            method.AddParameter(UseType("Grpc.Core.ServerCallContext"), "context");

                            var result = childElement.TypeReference.Element != null
                                ? "var result = "
                                : null;

                            method.AddStatement($"{result}await _mediator.Send(request.ToContract(), context.CancellationToken);");

                            method.AddReturn(this.MapToReturnStatement(childElement.TypeReference, "result"));
                        });
                    }
                });
        }

        public override void AfterTemplateRegistration()
        {
            ExecutionContext.EventDispatcher.Publish(new RegisterGrpcService(this));
        }

        private static bool IsApplicable(IElement childElement)
        {
            return childElement.SpecializationTypeId is MetadataIds.QueryElementTypeId or MetadataIds.CommandElementTypeId &&
                   childElement.HasStereotype(MetadataIds.ExposeWithGrpcStereotypeId);
        }

        private ServiceProtoFileTemplate ServiceProtoFileTemplateInstance => field ??= ExecutionContext.FindTemplateInstance<ServiceProtoFileTemplate>(ServiceProtoFileTemplate.TemplateId, Model.Id);

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