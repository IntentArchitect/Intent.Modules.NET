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
            AddTypeSource(TemplateFulfillingRoles.Application.Contracts.Dto);

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
                    switch (sendMessage.GetHubPublishMessageSettings().TargetClients().AsEnum())
                    {
                        case SendMessageModelStereotypeExtensions.HubPublishMessageSettings.TargetClientsOptionsEnum.All:
                            clientTarget = "All";
                            break;
                        case SendMessageModelStereotypeExtensions.HubPublishMessageSettings.TargetClientsOptionsEnum.User:
                            clientTarget = "User(userId)";
                            method.AddParameter("string", "userId");
                            break;
                        case SendMessageModelStereotypeExtensions.HubPublishMessageSettings.TargetClientsOptionsEnum.Users:
                            clientTarget = "Users(userIds)";
                            method.AddParameter(UseType("System.Collections.Generic.IReadOnlyList<string>"), "userIds");
                            break;
                        case SendMessageModelStereotypeExtensions.HubPublishMessageSettings.TargetClientsOptionsEnum.Group:
                            clientTarget = "Group(groupId)";
                            method.AddParameter("string", "groupId");
                            break;
                        case SendMessageModelStereotypeExtensions.HubPublishMessageSettings.TargetClientsOptionsEnum.Groups:
                            clientTarget = "Groups(groupIds)";
                            method.AddParameter(UseType("System.Collections.Generic.IReadOnlyList<string>"), "groupIds");
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                    var messageType = GetTypeInfo(sendMessage.InternalElement.TypeReference).Name.ToPascalCase();
                    method.AddStatement($@"await _hub.Clients.{clientTarget}.SendAsync(""{messageType}"", model);");
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