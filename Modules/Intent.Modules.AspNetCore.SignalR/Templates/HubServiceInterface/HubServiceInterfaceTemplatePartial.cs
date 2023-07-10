using System;
using System.Collections.Generic;
using Intent.AspNetCore.SignalR.Api;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.AspNetCore.SignalR.Templates.HubServiceInterface
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class HubServiceInterfaceTemplate : CSharpTemplateBase<SignalRHubModel>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.AspNetCore.SignalR.HubServiceInterface";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public HubServiceInterfaceTemplate(IOutputTarget outputTarget, SignalRHubModel model) : base(TemplateId, outputTarget, model)
        {
            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddInterface($"I{Model.Name.RemoveSuffix("Hub")}Hub", inter =>
                {
                    AddOperations(inter);
                });
        }

        private void AddOperations(CSharpInterface inter)
        {
            foreach (var sendMessage in Model.SendMessages)
            {
                var operationName = $"SendAsync";
                inter.AddMethod(UseType("System.Threading.Tasks.Task"), $"{operationName}", method =>
                {
                    method.AddParameter(GetTypeName(sendMessage.InternalElement.TypeReference), "model");
                    switch (sendMessage.GetHubPublishMessageSettings().TargetClients().AsEnum())
                    {
                        case SendMessageModelStereotypeExtensions.HubPublishMessageSettings.TargetClientsOptionsEnum.All:
                            break;
                        case SendMessageModelStereotypeExtensions.HubPublishMessageSettings.TargetClientsOptionsEnum.User:
                            method.AddParameter("string", "userId");
                            break;
                        case SendMessageModelStereotypeExtensions.HubPublishMessageSettings.TargetClientsOptionsEnum.Users:
                            method.AddParameter(UseType("System.Collections.Generic.IReadOnlyList<string>"), "userIds");
                            break;
                        case SendMessageModelStereotypeExtensions.HubPublishMessageSettings.TargetClientsOptionsEnum.Group:
                            method.AddParameter("string", "groupId");
                            break;
                        case SendMessageModelStereotypeExtensions.HubPublishMessageSettings.TargetClientsOptionsEnum.Groups:
                            method.AddParameter(UseType("System.Collections.Generic.IReadOnlyList<string>"), "groupIds");
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                });
            }
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