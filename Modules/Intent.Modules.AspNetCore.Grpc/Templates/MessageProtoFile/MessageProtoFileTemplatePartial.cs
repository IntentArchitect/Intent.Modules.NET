using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Intent.Engine;
using Intent.Exceptions;
using Intent.Metadata.Models;
using Intent.Modelers.Services.Api;
using Intent.Modules.AspNetCore.Grpc.Templates.MessagePartial;
using Intent.Modules.AspNetCore.Grpc.Templates.PagedResultPartial;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.CSharp.VisualStudio;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.Types.Api;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.ProjectItemTemplate.Partial", Version = "1.0")]

namespace Intent.Modules.AspNetCore.Grpc.Templates.MessageProtoFile
{
    [IntentManaged(Mode.Merge)]
    partial class MessageProtoFileTemplate : IntentTemplateBase<IElement>, IGrpcProtoTemplate<IElement>
    {
        private readonly SortedDictionary<string, string> _types = [];
        private readonly IReadOnlyCollection<DTOFieldModel> _fields;
        private bool _beforeTemplateExecutionCalled;

        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "Intent.AspNetCore.Grpc.MessageProtoFile";

        [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
        public MessageProtoFileTemplate(IOutputTarget outputTarget, IElement model = null) : base(TemplateId, outputTarget, model)
        {
            _fields = Model.ChildElements
                .Select(x => x.AsDTOFieldModel())
                .Where(x => x is not null)
                .ToArray();
        }

        public SortedSet<string> Imports { get; } = [];

        public override void AfterTemplateRegistration()
        {
            if (Model.GenericTypes.Any())
            {
                return;
            }

            Add(typeReference: null);
        }

        public override void BeforeTemplateExecution()
        {
            _beforeTemplateExecutionCalled = true;
        }

        internal string Add(ITypeReference typeReference)
        {
            try
            {
                var messageName = typeReference != null
                    ? typeReference.GetClosedGenericTypeName()
                    : Model.Name.ToPascalCase();

                if (_types.ContainsKey(messageName))
                {
                    return messageName;
                }

                var genericArguments = typeReference != null
                    ? typeReference.GenericTypeParameters.ToArray()
                    : [];

                var genericParameters = Model.GenericTypes.ToArray();

                if (genericArguments.Length != genericParameters.Length)
                {
                    throw new ElementException(Model, "Mismatch between number of generic parameters and provided arguments");
                }

                var genericArgumentsByParameterId = genericParameters
                    .Zip(genericArguments)
                    .ToDictionary(x => x.First.Id, x => x.Second);

                var sb = new StringBuilder();
                sb.AppendLine();
                sb.AppendLine($"message {messageName} {{");

                var fieldNumber = 1;
                foreach (var field in _fields)
                {
                    var fieldTypeReference = genericArgumentsByParameterId.TryGetValue(field.TypeReference.ElementId, out var genericArgument)
                        ? genericArgument
                        : field.TypeReference;
                    var isNullable = field.TypeReference.IsNullable;
                    var isCollection = field.TypeReference.IsCollection;

                    sb.AppendLine($"    {this.ResolveProtoType(fieldTypeReference, isNullable, isCollection)} {GetName(field)} = {fieldNumber++};");
                }

                sb.AppendLine("}");

                PartialTemplate.Add(
                    typeReference: typeReference,
                    messageName: messageName,
                    @namespace: CSharpNamespace);
                _types.Add(messageName, sb.ToString());

                return messageName;
            }
            catch (ElementException)
            {
                throw;
            }
            catch (Exception e)
            {
                throw new ElementException(Model, "An exception occured processing the template, see the inner exception for more details.", e);
            }
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

        public override bool CanRunTemplate() => !_beforeTemplateExecutionCalled || _types.Count > 0;

        private static string GetName(DTOFieldModel field) => field.Name.ToSnakeCase();

        private MessagePartialTemplate PartialTemplate => field ??= ExecutionContext.FindTemplateInstance<MessagePartialTemplate>(MessagePartialTemplate.TemplateId, Model.Id);

        private string Package => field ??= string.Join('.', PackageParts);

        public string[] PackageParts => field ??= CSharpNamespace.Split('.').Select(x => x.ToSnakeCase()).ToArray();

        internal string CSharpNamespace => field ??= this.GetProtoNamespace();

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public override ITemplateFileConfig GetTemplateFileConfig()
        {
            return new TemplateFileConfig(
                    fileName: $"{Model.Name.ToPascalCase()}",
                    fileExtension: "proto",
                    relativeLocation: this.GetProtoFolderPath())
                .WithItemType("Protobuf", wasAddedImplicitly: false)
                .WithAttribute("ProtoRoot", string.Join('\\', this.GetProtoRootParts()));
        }
    }
}