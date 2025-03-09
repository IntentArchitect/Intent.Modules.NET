using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modules.AspNetCore.Grpc.Templates.PagedResultProtoFile;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.FactoryExtensions;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.CSharp.TypeResolvers;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.AspNetCore.Grpc.Templates.PagedResultPartial
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class PagedResultPartialTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.AspNetCore.Grpc.PagedResultPartial";
        private readonly SortedDictionary<string, Action> _types = [];
        private bool _beforeTemplateExecutionCalled;

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public PagedResultPartialTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            SetDefaultCollectionFormatter(CSharpCollectionFormatter.CreateList());
            AddTypeSource(TemplateRoles.Domain.Enum);
            AddTypeSource(TemplateRoles.Application.Command);
            AddTypeSource(TemplateRoles.Application.Query);
            AddTypeSource(TemplateRoles.Application.Contracts.Dto);
            AddTypeSource(TemplateRoles.Application.Contracts.Enum);

            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("System.Linq");
        }

        internal void Add(ITypeReference typeReference, string messageName, string @namespace)
        {
            if (typeReference == null) throw new ArgumentNullException(nameof(typeReference));
            if (messageName == null) throw new ArgumentNullException(nameof(messageName));
            if (@namespace == null) throw new ArgumentNullException(nameof(@namespace));

            if (_types.ContainsKey(messageName))
            {
                return;
            }

            AddKnownType($"{@namespace}.{messageName}");
            var genericTypeArgument = typeReference.GenericTypeParameters.Single();

            // Do this beforehand so that type disambiguation is adequately
            // informed for the actual resolved type that gets used later
            _ = $"{GetTypeName((IElement)typeReference.Element)}<{GetTypeName(genericTypeArgument)}>";
            _ = UseType($"{@namespace}.{messageName}");
            _ = this.ResolveDtoTypeName(genericTypeArgument);

            _types.Add(messageName, () =>
            {
                CSharpFile
                    .AddClass(messageName, @class =>
                    {
                        @class.Partial();

                        var applicationType = $"{GetTypeName((IElement)typeReference.Element)}<{GetTypeName(genericTypeArgument)}>";

                        @class.AddMethod(applicationType, "ToContract", method =>
                        {
                            method.AddObjectInitializerBlock($"return new {applicationType}", initBlock =>
                            {
                                initBlock.AddInitStatement("TotalCount", "TotalCount");
                                initBlock.AddInitStatement("PageCount", "PageCount");
                                initBlock.AddInitStatement("PageSize", "PageSize");
                                initBlock.AddInitStatement("PageNumber", "PageNumber");
                                initBlock.AddInitStatement("Data", "Data.Select(x => x.ToContract()).ToArray()");

                                initBlock.WithSemicolon();
                            });
                        });

                        @class.AddMethod(messageName, "Create", method =>
                        {
                            method.Static();
                            method.AddParameter(applicationType, "contract");

                            method.AddStatement($"var message = new {messageName}();");
                            method.AddStatement("message.TotalCount = contract.TotalCount;");
                            method.AddStatement("message.PageCount = contract.PageCount;");
                            method.AddStatement("message.PageSize = contract.PageSize;");
                            method.AddStatement("message.PageNumber = contract.PageNumber;");
                            method.AddStatement($"message.Data.AddRange(contract.Data.Select({this.ResolveDtoTypeName(genericTypeArgument)}.Create));");

                            method.AddStatement("return message;");
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
                    .AddClass(messageName, @class =>
                    {
                        @class.Partial();

                        var applicationType = GetTypeName(typeReference);

                        @class.AddMethod(applicationType, "ToContract", method =>
                        {
                            method.AddStatement("return Items.Select(x => x.ToContract()).ToList();");
                        });

                        @class.AddMethod(messageName, "Create", method =>
                        {
                            method.Static();
                            method.AddParameter(applicationType, "contract");

                            method.AddStatement($"var message = new {messageName}();");
                            method.AddIfStatement("contract != null", @if => @if.AddStatement($"message.Items.AddRange(contract.Select({singularTypeMessageName}.Create));"));
                            method.AddStatement("return message;");
                        });
                    });
            });
        }

        public override string Namespace => CSharpFile.Namespace;

        public override void AfterTemplateRegistration()
        {
            CSharpFile.WithNamespace(MessageTemplate.CSharpNamespace);
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

        private PagedResultProtoFileTemplate MessageTemplate => field ??= ExecutionContext.FindTemplateInstance<PagedResultProtoFileTemplate>(PagedResultProtoFileTemplate.TemplateId);

        [IntentManaged(Mode.Fully)]
        public CSharpFile CSharpFile { get; }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        protected override CSharpFileConfig DefineFileConfig()
        {
            return new CSharpFileConfig(
                className: "PagedResult",
                @namespace: this.GetNamespace(),
                relativeLocation: this.GetFolderPath(),
                fileName: "PagedResult");
        }

        [IntentManaged(Mode.Fully)]
        public override string TransformText()
        {
            return CSharpFile.ToString();
        }
    }
}