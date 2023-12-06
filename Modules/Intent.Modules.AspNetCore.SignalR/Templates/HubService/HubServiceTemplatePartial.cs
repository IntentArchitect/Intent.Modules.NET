using System;
using System.Collections.Generic;
using Intent.AspNetCore.SignalR.Api;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.AspNetCore.SignalR.Templates.HubService
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class HubServiceTemplate : CSharpTemplateBase<SignalRHubModel>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.AspNetCore.SignalR.HubService";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public HubServiceTemplate(IOutputTarget outputTarget, SignalRHubModel model) : base(TemplateId, outputTarget, model)
        {
            AddTypeSource(TemplateRoles.Application.Contracts.Dto);

            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddClass($"{Model.Name.RemoveSuffix("Hub")}HubService", @class =>
                {
                    @class.ImplementsInterface(this.GetHubServiceInterfaceName(Model));
                    @class.AddConstructor(ctor =>
                    {
                        ctor.AddParameter($"{UseType("Microsoft.AspNetCore.SignalR.IHubContext")}<{this.GetHubName(Model)}>", "hub", param => param.IntroduceReadonlyField());
                    });

                    AddOperations(@class);
                });
        }

        private void AddOperations(CSharpClass @class)
        {
            foreach (var sendMessage in Model.SendMessages)
            {
                var operationName = $"SendAsync";
                @class.AddMethod(UseType("System.Threading.Tasks.Task"), $"{operationName}", method =>
                {
                    method.Async();
                    method.AddParameter(GetTypeName(sendMessage.InternalElement.TypeReference), "model");
                    string clientTarget;
                    switch (sendMessage.GetHubSendMessageSettings().TargetClients().AsEnum())
                    {
                        case SendMessageModelStereotypeExtensions.HubSendMessageSettings.TargetClientsOptionsEnum.All:
                            clientTarget = "All";
                            break;
                        case SendMessageModelStereotypeExtensions.HubSendMessageSettings.TargetClientsOptionsEnum.User:
                            clientTarget = "User(userId)";
                            method.AddParameter("string", "userId");
                            break;
                        case SendMessageModelStereotypeExtensions.HubSendMessageSettings.TargetClientsOptionsEnum.Users:
                            clientTarget = "Users(userIds)";
                            method.AddParameter(UseType("System.Collections.Generic.IReadOnlyList<string>"), "userIds");
                            break;
                        case SendMessageModelStereotypeExtensions.HubSendMessageSettings.TargetClientsOptionsEnum.Group:
                            clientTarget = "Group(groupName)";
                            method.AddParameter("string", "groupName");
                            break;
                        case SendMessageModelStereotypeExtensions.HubSendMessageSettings.TargetClientsOptionsEnum.Groups:
                            clientTarget = "Groups(groupNames)";
                            method.AddParameter(UseType("System.Collections.Generic.IReadOnlyList<string>"), "groupNames");
                            break;
                        case SendMessageModelStereotypeExtensions.HubSendMessageSettings.TargetClientsOptionsEnum.Client:
                            clientTarget = "Client(connectionId)";
                            method.AddParameter("string", "connectionId");
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                    var messageType = GetTypeInfo(sendMessage.InternalElement.TypeReference).Name.ToPascalCase();
                    method.AddStatement($@"await _hub.Clients.{clientTarget}.SendAsync(""{messageType}"", model);");
                });
            }
            @class.AddMethod(UseType("System.Threading.Tasks.Task"), "AddToGroupAsync", method =>
            {
                method.Async();
                method.AddParameter("string", "connectionId");
                method.AddParameter("string", "groupName");
                method.AddStatement("await _hub.Groups.AddToGroupAsync(connectionId, groupName);");
            });
            @class.AddMethod(UseType("System.Threading.Tasks.Task"), "RemoveFromGroupAsync", method =>
            {
                method.Async();
                method.AddParameter("string", "connectionId");
                method.AddParameter("string", "groupName");
                method.AddStatement("await _hub.Groups.RemoveFromGroupAsync(connectionId, groupName);");
            });
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