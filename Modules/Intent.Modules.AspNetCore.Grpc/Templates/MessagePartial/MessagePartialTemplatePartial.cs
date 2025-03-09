using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modelers.Services.Api;
using Intent.Modules.AspNetCore.Grpc.Templates.MessageProtoFile;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.CSharp.TypeResolvers;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.Types.Api;
using Intent.Modules.Constants;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.AspNetCore.Grpc.Templates.MessagePartial
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class MessagePartialTemplate : CSharpTemplateBase<IElement>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.AspNetCore.Grpc.MessagePartial";
        private readonly SortedDictionary<string, Action> _types = [];
        private readonly IReadOnlyCollection<DTOFieldModel> _fields;
        private bool _beforeTemplateExecutionCalled;

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public MessagePartialTemplate(IOutputTarget outputTarget, IElement model = null) : base(TemplateId, outputTarget, model)
        {
            SetDefaultCollectionFormatter(CSharpCollectionFormatter.CreateList());
            AddTypeSource(TemplateRoles.Domain.Enum);
            AddTypeSource(TemplateRoles.Application.Command);
            AddTypeSource(TemplateRoles.Application.Query);
            AddTypeSource(TemplateRoles.Application.Contracts.Dto);
            AddTypeSource(TemplateRoles.Application.Contracts.Enum);

            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath());

            _fields = Model.ChildElements
                .Select(x => x.AsDTOFieldModel())
                .Where(x => x is not null)
                .ToArray();
        }

        internal void Add(ITypeReference typeReference, string messageName, string @namespace)
        {
            if (messageName == null) throw new ArgumentNullException(nameof(messageName));
            if (@namespace == null) throw new ArgumentNullException(nameof(@namespace));

            const string messageVariableName = "message";
            const string contractVariableName = "contract";

            if (_types.ContainsKey(messageName))
            {
                return;
            }

            AddKnownType($"{@namespace}.{messageName}");

            // Do this beforehand so that type disambiguation is adequately
            // informed for the actual resolved type that gets used later
            _ = typeReference != null ? $"{GetTypeName(typeReference)}" : GetTypeName(Model);
            _ = UseType($"{@namespace}.{messageName}");

            _types.Add(messageName, () =>
            {
                CSharpFile
                    .AddUsing("System.Diagnostics.CodeAnalysis")
                    .AddClass(messageName, @class =>
                    {
                        @class.Partial();

                        var applicationType = typeReference != null
                            ? $"{GetTypeName(typeReference)}"
                            : GetTypeName(Model);

                        @class.AddMethod(applicationType, "ToContract", method =>
                        {
                            if (Model.SpecializationTypeId is MetadataIds.CommandElementTypeId or MetadataIds.QueryElementTypeId)
                            {
                                method.AddInvocationStatement($"return new {applicationType}", invocation =>
                                {
                                    foreach (var field in _fields)
                                    {
                                        var (expression, _) = this.MapForMessage(GrpcTypeResolverHelper.MappingType.FromMessage, field.TypeReference, field.Name.ToPascalCase());

                                        invocation.AddArgument(field.Name.ToCamelCase(), expression);
                                    }
                                });
                            }
                            else
                            {
                                method.AddObjectInitializerBlock($"return new {applicationType}", initBlock =>
                                {
                                    foreach (var field in _fields)
                                    {
                                        var (expression, _) = this.MapForMessage(GrpcTypeResolverHelper.MappingType.FromMessage, field.TypeReference, field.Name.ToPascalCase());

                                        initBlock.AddInitStatement(field.Name.ToPascalCase(), expression);
                                    }

                                    initBlock.WithSemicolon();
                                });
                            }
                        });

                        @class.AddMethod($"{messageName}?", "Create", method =>
                        {
                            method.AddAttribute("[return: NotNullIfNotNull(nameof(contract))]");
                            method.Static();
                            method.AddParameter(EnsureNullable(applicationType), contractVariableName);

                            method.AddIfStatement("contract == null", @if => @if.AddReturn("null"));

                            method.AddObjectInitializerBlock($"var {messageVariableName} = new {messageName}", initBlock =>
                            {
                                foreach (var field in _fields)
                                {
                                    var (expression, isForAddRange) = this.MapForMessage(GrpcTypeResolverHelper.MappingType.ToMessage, field.TypeReference, $"{contractVariableName}.{field.Name.ToPascalCase()}");
                                    if (isForAddRange)
                                    {
                                        continue;
                                    }

                                    initBlock.AddInitStatement(field.Name.ToPascalCase(), expression);
                                }

                                initBlock.WithSemicolon();
                            });

                            foreach (var field in _fields)
                            {
                                var (expression, isForMethodCall) = this.MapForMessage(GrpcTypeResolverHelper.MappingType.ToMessage, field.TypeReference, $"{contractVariableName}.{field.Name.ToPascalCase()}");
                                if (!isForMethodCall)
                                {
                                    continue;
                                }

                                method.AddStatement($"{messageVariableName}.{field.Name.ToPascalCase()}{expression};");
                            }

                            method.AddStatement($"return {messageVariableName};");
                        });
                    });
            });
        }

        internal void AddListOf(ITypeReference typeReference, string messageName, string singularTypeMessageName, string @namespace)
        {
            if (_types.ContainsKey(messageName))
            {
                return;
            }

            Add(typeReference: typeReference,
                messageName: singularTypeMessageName,
                @namespace: @namespace);

            AddKnownType($"{@namespace}.{messageName}");

            // Do this beforehand so that type disambiguation is adequately
            // informed for the actual resolved type that gets used later
            _ = GetTypeName(typeReference);
            _ = UseType($"{@namespace}.{messageName}");

            _types.Add(messageName, () =>
            {
                CSharpFile
                    .AddUsing("System.Diagnostics.CodeAnalysis")
                    .AddUsing("System.Linq")
                    .AddClass(messageName, @class =>
                    {
                        @class.Partial();

                        var applicationType = GetTypeName(typeReference);

                        @class.AddMethod(applicationType, "ToContract", method =>
                        {
                            method.AddStatement("return Items.Select(x => x.ToContract()).ToList();");
                        });

                        @class.AddMethod($"{messageName}?", "Create", method =>
                        {
                            method.AddAttribute("[return: NotNullIfNotNull(nameof(contract))]");
                            method.Static();
                            method.AddParameter(EnsureNullable(applicationType), "contract");

                            method.AddIfStatement("contract == null", @if => @if.AddReturn("null"));

                            method.AddStatement($"var message = new {messageName}();");
                            method.AddStatement($"message.Items.AddRange(contract.Select({singularTypeMessageName}.Create));");
                            method.AddStatement("return message;");
                        });
                    });
            });
        }

        private static string EnsureNullable(string applicationType) => applicationType.EndsWith('?') ? applicationType : $"{applicationType}?";

        public override string Namespace => CSharpFile.Namespace;

        public override void AfterTemplateRegistration()
        {
            CSharpFile.WithNamespace(MessageProtoFileTemplateInstance.CSharpNamespace);
        }

        public override void BeforeTemplateExecution()
        {
            foreach (var action in _types.Values)
            {
                action();
            }

            _beforeTemplateExecutionCalled = true;
        }

        public override bool CanRunTemplate() => !_beforeTemplateExecutionCalled || _types.Count > 0;

        private MessageProtoFileTemplate MessageProtoFileTemplateInstance => field ??= ExecutionContext.FindTemplateInstance<MessageProtoFileTemplate>(MessageProtoFileTemplate.TemplateId, Model.Id);

        [IntentManaged(Mode.Fully)]
        public CSharpFile CSharpFile { get; }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        protected override CSharpFileConfig DefineFileConfig()
        {
            return new CSharpFileConfig(
                className: Model.Name.ToPascalCase(),
                @namespace: this.GetProtoNamespace(),
                relativeLocation: this.GetProtoFolderPath(),
                fileName: Model.Name.ToPascalCase());
        }

        [IntentManaged(Mode.Fully)]
        public override string TransformText()
        {
            return CSharpFile.ToString();
        }
    }
}