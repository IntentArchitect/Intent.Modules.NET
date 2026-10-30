using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Intent.Configuration;
using Intent.Engine;
using Intent.Exceptions;
using Intent.Metadata.Models;
using Intent.Modelers.Services.Api;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.FileBuilders;
using Intent.Modules.Services.Guardrails.Contracts.Templates;
using Intent.Templates;
using NSubstitute;
using Shouldly;
using Xunit;
using static Intent.Modules.Guardrails.Contracts.Tests.Fakes;
using ServicesAsyncConfiguration = Intent.Services.Guardrails.Contracts.Api.OperationModelStereotypeExtensions.AsyncConfiguration;

namespace Intent.Modules.Guardrails.Contracts.Tests
{
    /// <summary>
    /// Method signatures the Services contract builders write: a Service Proxy operation's modifiers and parameter modifiers,
    /// and async void.
    /// </summary>
    public class OperationShapeTests
    {
        [Fact]
        public void A_proxy_class_method_takes_the_modifiers_its_Operation_Configuration_declares()
        {
            var proxy = Proxy("BasislinkApi");
            ProxyOperation(proxy, "InsertNewNotification", OperationConfiguration(("Static", "true")));
            ProxyOperation(proxy, "GetAccessToken", OperationConfiguration(("Access Modifier", "private")));
            ProxyOperation(proxy, "Send", OperationConfiguration(("Access Modifier", "protected"), ("Virtual", "true")));
            ProxyOperation(proxy, "Ping");

            var text = Render((file, template) => ContractTypeBuilders.AddServiceProxy(file, template, proxy));

            text.ShouldContain("public static void InsertNewNotification()");
            text.ShouldContain("private void GetAccessToken()");
            text.ShouldContain("protected virtual void Send()");
            text.ShouldContain("public void Ping()");
        }

        [Fact]
        public void A_proxy_parameter_takes_its_Parameter_Modifier_on_a_class_and_an_interface()
        {
            var client = Proxy("CatalogClient");
            var tryGet = ProxyOperation(client, "TryGet");
            ProxyParameter(tryGet, "key", "string");
            ProxyParameter(tryGet, "value", "string", ParameterModifier("out"));
            var api = Proxy("ICatalogApi", isInterface: true);
            var fill = ProxyOperation(api, "Fill");
            ProxyParameter(fill, "order", "Order", ParameterModifier("ref"));
            ProxyParameter(fill, "tags", "string", ParameterModifier("in"));

            var text = Render((file, template) =>
            {
                ContractTypeBuilders.AddServiceProxy(file, template, client);
                ContractTypeBuilders.AddServiceProxy(file, template, api);
            });

            text.ShouldContain("public void TryGet(string key, out string value)");
            text.ShouldContain("void Fill(ref Order order, in string tags);");
        }

        [Fact]
        public void An_interface_proxys_operation_that_declares_modifiers_is_an_error()
        {
            var api = Proxy("IGitHubApi", isInterface: true);
            var getUser = ProxyOperation(api, "GetUser", OperationConfiguration(("Static", "true")));
            // Public with nothing ticked declares nothing, so it isn't an error.
            ProxyOperation(api, "GetRepo", OperationConfiguration(("Access Modifier", "public")));

            var exception = Should.Throw<ElementException>(() => Render((file, template) => ContractTypeBuilders.AddServiceProxy(file, template, api)));

            exception.ElementId.ShouldBe(getUser.Id);
            exception.Message.ShouldBe("Operation 'GetUser' has an Operation Configuration that declares modifiers, but its Service Proxy is an interface " +
                "(Service Proxy Configuration Type interface), whose methods take none. Remove the Operation Configuration stereotype.");
        }

        [Fact]
        public void Task_Type_void_writes_async_void_on_a_service_and_a_proxy_operation()
        {
            var service = Element(Package(), "Service", "AdvertisementController");
            service.SpecializationTypeId.Returns(ServiceModel.SpecializationTypeId);
            ServiceOperation(service, "PushToSignalR", Stereotype(ServicesAsyncConfiguration.DefinitionId,
                ("Async Mode", "async"), ("Task Type", "void"), ("Include Cancellation Token", "false")));
            var proxy = Proxy("NotificationClient");
            ProxyOperation(proxy, "OnMessage", Stereotype(OptionalModelerMetadata.ServiceProxyAsyncConfigurationStereotypeId,
                ("Async Mode", "async"), ("Task Type", "void"), ("Include Cancellation Token", "true")));

            var text = Render((file, template) =>
            {
                ContractTypeBuilders.AddService(file, template, new ServiceModel(service));
                ContractTypeBuilders.AddServiceProxy(file, template, proxy);
            });

            text.ShouldContain("public async void PushToSignalR()");
            text.ShouldContain("public async void OnMessage(CancellationToken cancellationToken = default)");
        }

        [Theory]
        [InlineData("no async keyword", null,
            "Operation 'PushToSignalR' has an Async Configuration Task Type of void with Async Mode 'no async keyword'. void writes async void, so it needs Async Mode async. " +
            "For a method that returns nothing and isn't declared async, choose Async Mode sync.")]
        [InlineData("async", "bool",
            "Operation 'PushToSignalR' has an Async Configuration Task Type of void, but returns bool. An async void method returns nothing: remove the return type, or choose Task or ValueTask.")]
        public void Task_Type_void_needs_Async_Mode_async_and_no_return_type(string asyncMode, string returnType, string message)
        {
            var service = Element(Package(), "Service", "AdvertisementController");
            service.SpecializationTypeId.Returns(ServiceModel.SpecializationTypeId);
            var operation = ServiceOperation(service, "PushToSignalR", Stereotype(ServicesAsyncConfiguration.DefinitionId,
                ("Async Mode", asyncMode), ("Task Type", "void"), ("Include Cancellation Token", "false")));
            if (returnType != null)
            {
                operation.Typed(returnType);
            }

            var exception = Should.Throw<ElementException>(() => Render((file, template) => ContractTypeBuilders.AddService(file, template, new ServiceModel(service))));

            exception.ElementId.ShouldBe(operation.Id);
            exception.Message.ShouldBe(message);
        }

        private static IStereotype OperationConfiguration(params (string Key, string Value)[] properties) =>
            Stereotype(OptionalModelerMetadata.ServiceProxyOperationConfigurationStereotypeId, properties);

        private static IStereotype ParameterModifier(string modifier) =>
            Stereotype(OptionalModelerMetadata.ServiceProxyParameterModifierStereotypeId, ("Modifier", modifier));

        private static IElement Proxy(string name, bool isInterface = false)
        {
            var stereotypes = isInterface
                ? new[] { Stereotype(OptionalModelerMetadata.ServiceProxyConfigurationStereotypeId, ("Type", "interface")) }
                : Array.Empty<IStereotype>();
            var proxy = Element(Package(), "Service Proxy", name, stereotypes);
            proxy.SpecializationTypeId.Returns(OptionalModelerMetadata.ServiceProxyTypeId);
            return proxy;
        }

        private static IElement ProxyOperation(IElement proxy, string name, params IStereotype[] stereotypes)
        {
            var operation = Element(proxy, "Operation", name, stereotypes);
            operation.SpecializationTypeId.Returns(OptionalModelerMetadata.ServiceProxyOperationTypeId);
            return operation;
        }

        private static IElement ProxyParameter(IElement operation, string name, string type, params IStereotype[] stereotypes)
        {
            var parameter = Element(operation, "Parameter", name, stereotypes).Typed(type);
            parameter.SpecializationTypeId.Returns(OptionalModelerMetadata.ServiceProxyParameterTypeId);
            return parameter;
        }

        private static IElement ServiceOperation(IElement service, string name, params IStereotype[] stereotypes)
        {
            var operation = Element(service, "Operation", name, stereotypes);
            operation.SpecializationTypeId.Returns(OperationModel.SpecializationTypeId);
            return operation;
        }

        /// <summary>
        /// The file contract builders write, built as the Software Factory builds a file builder template's file, with every
        /// type name resolved to the model's own name. Whitespace is collapsed, so assertions read one line.
        /// </summary>
        private static string Render(Action<CSharpFile, ICSharpTemplate> add)
        {
            // An unset Default Type Modifiers setting: types are public.
            var group = Substitute.For<IGroupSettings>();
            group.GetSetting(Arg.Any<string>()).Returns((ISetting)null);
            var template = Substitute.For<ICSharpFileBuilderTemplate>();
            template.ExecutionContext.Settings.GetGroup(Arg.Any<string>()).Returns(group);
            template.GetTypeName(Arg.Any<ITypeReference>(), Arg.Any<string>()).Returns(call => ((ITypeReference)call[0]).Element.Name);
            template.UseType(Arg.Any<string>()).Returns(call => ((string)call[0]).Split('.').Last());

            var file = new CSharpFile("Acme.Clients", "src", template);
            add(file, template);
            template.File.Returns(file);
            template.CanRunTemplate().Returns(true);
            var outputTarget = Substitute.For<IOutputTarget>();
            outputTarget.TemplateInstances.Returns(new List<ITemplate> { template });
            var context = Substitute.For<ISoftwareFactoryExecutionContext>();
            context.OutputTargets.Returns(new[] { outputTarget });
            FileBuilderHelper.PerformConfiguration(context, isForAfterBuild: false);
            FileBuilderHelper.PerformConfiguration(context, isForAfterBuild: true);
            return Regex.Replace(file.ToString(), @"\s+", " ");
        }
    }
}
