using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using Intent.Engine;
using Intent.Exceptions;
using Intent.Metadata.Models;
using Intent.Modules.AspNetCore.Grpc.Templates.PagedResultPartial;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.CSharp.VisualStudio;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.ProjectItemTemplate.Partial", Version = "1.0")]

namespace Intent.Modules.AspNetCore.Grpc.Templates.PagedResultProtoFile
{
    [IntentManaged(Mode.Merge)]
    partial class PagedResultProtoFileTemplate : IntentTemplateBase<object>, IGrpcProtoTemplate<object>
    {
        private readonly SortedDictionary<string, string> _types = [];
        private bool _beforeTemplateExecutionCalled;

        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "Intent.AspNetCore.Grpc.PagedResultProtoFile";

        [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
        public PagedResultProtoFileTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
        }

        internal string Add(ITypeReference typeReference)
        {
            var name = typeReference.GetClosedGenericTypeName();
            if (_types.ContainsKey(name))
            {
                return name;
            }

            var dataType = this.ResolveProtoType(typeReference.GenericTypeParameters.Single());
            var definition = $$"""

                               message {{name}} {
                                   int32 total_count = 1;
                                   int32 page_count = 2;
                                   int32 page_size = 3;
                                   int32 page_number = 4;
                                   repeated {{dataType}} data = 5;
                               }
                               """;

            PartialTemplate.Add(
                typeReference: typeReference,
                messageName: name,
                @namespace: CSharpNamespace);
            _types.Add(name, definition.ReplaceLineEndings());

            return name;
        }

        internal string AddListOf(ITypeReference typeReference)
        {
            var typeName = Add(typeReference);
            var name = $"ListOf{typeName}";

            if (_types.ContainsKey(name))
            {
                return name;
            }

            var definition = $$"""

                               message {{name}} {
                                   repeated {{typeName}} items = 1;
                               }
                               """;

            PartialTemplate.AddListOf(
                typeReference: typeReference,
                messageName: name,
                singularTypeMessageName: typeName,
                @namespace: CSharpNamespace);
            _types.Add(name, definition.ReplaceLineEndings());

            return name;
        }

        private PagedResultPartialTemplate PartialTemplate => field ??= ExecutionContext.FindTemplateInstance<PagedResultPartialTemplate>(PagedResultPartialTemplate.TemplateId);

        public SortedSet<string> Imports { get; } = [];

        public override void BeforeTemplateExecution() => _beforeTemplateExecutionCalled = true;

        public override bool CanRunTemplate() => !_beforeTemplateExecutionCalled || _types.Count > 0;

        private string Package => field ??= string.Join('.', PackageParts);

        public string[] PackageParts => field ??= CSharpNamespace.Split('.').Select(x => x.ToSnakeCase()).ToArray();

        internal string CSharpNamespace => field ??= this.GetNamespace();

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public override ITemplateFileConfig GetTemplateFileConfig()
        {
            return new TemplateFileConfig(
                    fileName: $"PagedResult",
                    fileExtension: "proto")
                .WithItemType("Protobuf", wasAddedImplicitly: false)
                .WithAttribute("ProtoRoot", string.Join('\\', this.GetProtoRootParts()));
        }
    }
}