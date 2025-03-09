using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modelers.Services.Api;
using Intent.Modules.AspNetCore.Grpc.Templates.MessageProtoFile;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.CSharp.VisualStudio;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.ProjectItemTemplate.Partial", Version = "1.0")]

namespace Intent.Modules.AspNetCore.Grpc.Templates.ServiceProtoFile
{
    [IntentManaged(Mode.Merge)]
    partial class ServiceProtoFileTemplate : IntentTemplateBase<IElement>, IGrpcProtoTemplate<IElement>
    {
        private readonly List<string> _operations = [];
        private readonly List<string> _messages = [];

        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "Intent.AspNetCore.Grpc.ServiceProtoFile";

        [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
        public ServiceProtoFileTemplate(IOutputTarget outputTarget, IElement model = null) : base(TemplateId, outputTarget, model)
        {
        }

        public override void AfterTemplateRegistration()
        {
            var serviceName = Model.Name.ToPascalCase();

            foreach (var childElement in Model.ChildElements)
            {
                if (!childElement.HasStereotype(MetadataIds.ExposeWithGrpcStereotypeId))
                {
                    continue;
                }

                string operationName;
                string argumentType;

                if (childElement.SpecializationTypeId is MetadataIds.QueryElementTypeId or MetadataIds.CommandElementTypeId)
                {
                    var dtoTemplate = GrpcTypeResolverHelper.GetTemplateInstance<MessageProtoFileTemplate>(this, MessageProtoFileTemplate.TemplateId, childElement.Id);
                    var dtoTemplatePackageParts = dtoTemplate.PackageParts;

                    var commonPartCount = dtoTemplatePackageParts
                        .Zip(PackageParts)
                        .TakeWhile(x => x.First == x.Second)
                        .Count();

                    var qualifier = string.Join('.', dtoTemplatePackageParts.Skip(commonPartCount));
                    if (qualifier.Length > 0)
                    {
                        qualifier = $"{qualifier}.";
                    }

                    operationName = childElement.Name.ToPascalCase().RemoveSuffix("Command", "Query");
                    argumentType = $"{qualifier}{childElement.Name.ToPascalCase()}";
                }
                else if (childElement.SpecializationTypeId is OperationModel.SpecializationTypeId)
                {
                    operationName = childElement.Name.ToPascalCase();
                    var operation = childElement.AsOperationModel();
                    if (operation.Parameters.Count == 0)
                    {
                        Imports.Add("google/protobuf/empty.proto");
                        argumentType = "google.protobuf.Empty";
                    }
                    else
                    {
                        argumentType = $"{serviceName}{operationName}Request";

                        var sb = new StringBuilder();
                        sb.AppendLine($"message {argumentType} {{");

                        var number = 1;
                        foreach (var parameter in operation.Parameters)
                        {
                            sb.AppendLine($"    {this.ResolveProtoType(parameter)} {parameter.Name.ToSnakeCase()} = {number++};");
                        }

                        sb.AppendLine("}");

                        _messages.Add(sb.ToString());
                    }
                }
                else
                {
                    continue;
                }

                // Forcing isNullable ensures that list and scalar values are returned in a containing
                // message as neither are allowed in a service in a .proto file.
                var returnType = this.ResolveProtoType(
                    typeReference: childElement.TypeReference,
                    isNullable: true);

                _operations.Add($"    rpc {operationName} ({argumentType}) returns ({returnType});");
            }
        }

        public SortedSet<string> Imports { get; } = [];

        private string Package => field ??= string.Join('.', PackageParts);

        public string[] PackageParts => field ??= CSharpNamespace.Split('.').Select(x => x.ToSnakeCase()).ToArray();

        internal string CSharpNamespace => field ??= this.GetProtoNamespace();

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public override ITemplateFileConfig GetTemplateFileConfig()
        {
            return new TemplateFileConfig(
                    fileName: $"{Model.Name.EnsureSuffixedWith("Service")}",
                    fileExtension: "proto",
                    relativeLocation: this.GetProtoFolderPath())
                .WithItemType("Protobuf", wasAddedImplicitly: false)
                .WithAttribute("ProtoRoot", string.Join('\\', this.GetProtoRootParts()))
                .WithAttribute("GrpcServices", "Server");
        }
    }
}