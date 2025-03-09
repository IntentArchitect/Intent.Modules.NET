using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modules.AspNetCore.Grpc.Templates.CommonTypesPartial;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.CSharp.VisualStudio;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.ProjectItemTemplate.Partial", Version = "1.0")]

namespace Intent.Modules.AspNetCore.Grpc.Templates.CommonTypesProtoFile
{
    [IntentManaged(Mode.Merge)]
    partial class CommonTypesProtoFileTemplate : IntentTemplateBase<object>, IGrpcProtoTemplate<object>
    {
        private readonly SortedDictionary<string, string> _types = [];
        private bool _beforeTemplateExecutionCalled;

        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "Intent.AspNetCore.Grpc.CommonTypesProtoFile";

        [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
        public CommonTypesProtoFileTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
        }

        public SortedSet<string> Imports { get; } = [];

        public override void BeforeTemplateExecution()
        {
            _beforeTemplateExecutionCalled = true;
        }

        public override bool CanRunTemplate() => !_beforeTemplateExecutionCalled || _types.Count > 0;

        internal string AddDecimalType()
        {
            const string name = "DecimalValue";

            if (_types.ContainsKey(name))
            {
                return name;
            }

            var definition = $$"""

                               // See https://learn.microsoft.com/aspnet/core/grpc/protobuf?view=aspnetcore-9.0#creating-a-custom-decimal-type-for-protobuf
                               // Example: 12345.6789 -> { units = 12345, nanos = 678900000 }
                               message {{name}} {
                                   // Whole units part of the amount
                                   int64 units = 1;
                               
                                   // Nano units of the amount (10^-9)
                                   // Must be same sign as units
                                   sfixed32 nanos = 2;
                               }
                               """;

            PartialTemplate.AddDecimalType(
                messageName: name,
                @namespace: CSharpNamespace);
            _types.Add(name, definition.ReplaceLineEndings());

            return name;
        }

        internal string AddListOf(
            string ofName,
            string protoTypeName,
            string import)
        {
            var name = $"ListOf{ofName}";

            if (_types.ContainsKey(name))
            {
                return name;
            }

            if (import != null)
            {
                Imports.Add(import);
            }

            var definition = $$"""


                               message {{name}} {
                                   repeated {{protoTypeName}} items = 1;
                               }
                               """;
            _types.Add(name, definition.ReplaceLineEndings());

            return name;
        }

        internal string AddListOfDecimalType()
        {
            var ofName = AddDecimalType();

            return AddListOf(ofName, ofName, import: null);
        }

        internal string AddListOfMapOf(ITypeReference typeReference)
        {
            var ofName = AddMapOf(typeReference);

            return AddListOf(ofName, ofName, import: null);
        }

        internal string AddMapOf(ITypeReference typeReference)
        {
            var keyType = typeReference.GenericTypeParameters.First();
            var valueType = typeReference.GenericTypeParameters.Skip(1).First();

            var name = $"MapOf{keyType.GetClosedGenericTypeName()}And{valueType.GetClosedGenericTypeName()}";
            if (_types.ContainsKey(name))
            {
                return name;
            }

            var definition = $$"""


                               message {{name}} {
                                   map<{{this.ResolveProtoType(keyType)}}, {{this.ResolveProtoType(valueType)}}> items = 1;
                               }
                               """;

            _types.Add(name, definition.ReplaceLineEndings());

            return name;
        }

        private CommonTypesPartialTemplate PartialTemplate => field ??= ExecutionContext.FindTemplateInstance<CommonTypesPartialTemplate>(CommonTypesPartialTemplate.TemplateId);

        private string Package => field ??= string.Join('.', PackageParts);

        public string[] PackageParts => field ??= CSharpNamespace.Split('.').Select(x => x.ToSnakeCase()).ToArray();

        internal string CSharpNamespace => field ??= this.GetNamespace();

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public override ITemplateFileConfig GetTemplateFileConfig()
        {
            return new TemplateFileConfig(
                    fileName: $"CommonTypes",
                    fileExtension: "proto",
                    relativeLocation: this.GetFolderPath())
                .WithItemType("Protobuf", wasAddedImplicitly: false)
                .WithAttribute("ProtoRoot", string.Join('\\', this.GetProtoRootParts()));
        }
    }
}