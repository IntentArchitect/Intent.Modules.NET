using System;
using System.Collections.Generic;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modules.AspNetCore.Grpc.Templates.PagedResultProtoFile;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.CSharp.TypeResolvers;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.AspNetCore.Grpc.Templates.CommonTypesPartial
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class CommonTypesPartialTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.AspNetCore.Grpc.CommonTypesPartial";
        private readonly SortedDictionary<string, Action> _types = [];
        private bool _beforeTemplateExecutionCalled;

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public CommonTypesPartialTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
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

        internal void AddDecimalType(string messageName, string @namespace)
        {
            if (_types.ContainsKey(messageName))
            {
                return;
            }

            AddKnownType($"{@namespace}.{messageName}");

            _types.Add(messageName, () =>
            {
                CSharpFile
                    .AddClass(messageName, @class =>
                    {
                        @class.Partial();

                        @class.AddField("decimal", "NanoFactor", f => f.PrivateConstant("1_000_000_000"));

                        @class.AddConstructor(ctor =>
                        {
                            ctor.AddParameter("long", "units");
                            ctor.AddParameter("int", "nanos");
                            ctor.AddStatement("Units = units;");
                            ctor.AddStatement("Nanos = nanos;");
                        });

                        @class.AddMethod("implicit", "decimal", method =>
                        {
                            method.Static().Operator();
                            method.AddParameter(messageName, "grpcDecimal");
                            method.AddStatement("return grpcDecimal.Units + grpcDecimal.Nanos / NanoFactor;");
                        });

                        @class.AddMethod("implicit", messageName, method =>
                        {
                            method.Static().Operator();
                            method.AddParameter("decimal", "value");
                            method.AddStatement("var units = decimal.ToInt64(value);");
                            method.AddStatement("var nanos = decimal.ToInt32((value - units) * NanoFactor);");
                            method.AddStatement($"return new {messageName}(units, nanos);");
                        });
                    });
            });
        }

        internal void AddListOf(
            ITypeReference typeReference,
            string typeName,
            string @namespace,
            string toContractTransform = null,
            string onCreateTransform = null)
        {
            AddListOf(
                applicationType: GetTypeName(typeReference),
                typeName: typeName,
                @namespace: @namespace,
                toContractTransform: toContractTransform,
                onCreateTransform: onCreateTransform);
        }

        internal void AddListOf(
            string applicationType,
            string typeName,
            string @namespace,
            string toContractTransform = null,
            string onCreateTransform = null)
        {
            if (_types.ContainsKey(typeName))
            {
                return;
            }

            AddKnownType($"{@namespace}.{typeName}");

            _types.Add(typeName, () =>
            {
                CSharpFile
                    .AddClass(typeName, @class =>
                    {
                        @class.Partial();

                        @class.AddMethod(applicationType.TrimEnd('?'), "ToContract", method =>
                        {
                            method.AddStatement($"return Items{toContractTransform}.ToList();");
                        });

                        @class.AddMethod($"{typeName}?", "Create", method =>
                        {
                            method.AddAttribute($"[return: {UseType("System.Diagnostics.CodeAnalysis.NotNullIfNotNull")}(nameof(contract))]");
                            method.Static();
                            method.AddParameter(applicationType.Replace("List<", "IEnumerable<"), "contract");

                            method.AddIfStatement("contract == null", @if => @if.AddStatement("return null;"));

                            method.AddStatement($"var message = new {typeName}();", s => s.SeparatedFromPrevious());
                            method.AddStatement($"message.Items.AddRange(contract{onCreateTransform});");
                            method.AddStatement("return message;");
                        });
                    });
            });
        }

        internal void AddMapOf(
            string applicationType,
            string messageName,
            string @namespace)
        {
            if (_types.ContainsKey(messageName))
            {
                return;
            }

            AddKnownType($"{@namespace}.{messageName}");

            _types.Add(messageName, () =>
            {
                CSharpFile
                    .AddClass(messageName, @class =>
                    {
                        @class.Partial();

                        @class.AddMethod(applicationType, "ToContract", method =>
                        {
                            method.AddStatement("return Items.ToDictionary(x => x.Key, x => x.Value);");
                        });

                        @class.AddMethod($"{messageName}?", "Create", method =>
                        {
                            method.AddAttribute($"[return: {UseType("System.Diagnostics.CodeAnalysis.NotNullIfNotNull")}(nameof(contract))]");
                            method.Static();
                            method.AddParameter($"{applicationType}?", "contract");

                            method.AddIfStatement("contract == null", @if => @if.AddStatement("return null;"));

                            method.AddStatement($"var message = new {messageName}();", s => s.SeparatedFromPrevious());
                            method.AddStatement("message.Items.Add(contract);");
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
                className: "CommonTypes",
                @namespace: this.GetNamespace(),
                relativeLocation: this.GetFolderPath(),
                fileName: "CommonTypes");
        }

        [IntentManaged(Mode.Fully)]
        public override string TransformText()
        {
            return CSharpFile.ToString();
        }
    }
}