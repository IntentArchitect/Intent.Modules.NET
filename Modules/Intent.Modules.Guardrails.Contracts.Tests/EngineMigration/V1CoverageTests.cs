using System.Collections.Generic;
using System.Linq;
using Intent.IArchitect.Agent.Persistence.Model;
using Intent.IArchitect.Agent.Persistence.Model.Common;
using Intent.Modules.Common.Drift;
using Intent.Modules.Common.Drift.EngineMigration;
using Shouldly;
using Xunit;

namespace Intent.Modules.Guardrails.Contracts.Tests.EngineMigration
{
    /// <summary>
    /// The exceptions that keep the converted opt-in rows to what v1 reported: a row is a glob, where v1's convention reported
    /// only what its find matched, and the member row reports public fields, which v1 never did.
    /// </summary>
    public class V1CoverageTests
    {
        private const string Services = """
            {
              "files": ["Api/**/*.cs", "Dto/**/*.cs"],
              "own": [
                { "kind": "endpoint", "find": "class with :ControllerBase > method with [\\[,]Http(?:Get|Post)\\b", "name": "{Class}.{Method}" },
                { "kind": "DTO", "find": "class with \\bpublic class \\w+", "files": "Dto/**/*.cs" }
              ]
            }
            """;

        private static readonly Dictionary<string, string> Code = new()
        {
            ["Api/OrdersController.cs"] = """
                public class OrdersController : ControllerBase
                {
                    [HttpGet]
                    public OrderDto Get() => new OrderDto();
                }
                """,
            ["Api/NewController.cs"] = """
                public class NewController : ControllerBase
                {
                    [HttpPost]
                    public void Post() { }
                }
                """,
            ["Api/Startup.cs"] = "public class Startup { public void Configure() { } }",
            ["Api/ICache.cs"] = "public interface ICache { } public class Cache : ICache { }",
            ["Dto/OrderDto.cs"] = """
                public class OrderDto
                {
                    public int Id { get; set; }
                    public string note;
                    public LineDto line;
                }

                public class LineDto { }

                internal sealed class Helper { }

                public class RoleAttribute : System.Attribute { }
                """,
            ["Dto/Unmodelled.cs"] = "public class UnmodelledDto { }",
            ["Other/Tool.cs"] = "public class Tool { }"
        };

        private static readonly ModelledType[] Model =
        {
            new("OrdersController", "Service", new[] { "Get" }),
            new("OrderDto", "DTO", new[] { "Id", "line" }),
            new("LineDto", "DTO", new string[0])
        };

        [Fact]
        public void A_type_no_convention_found_is_excepted_with_the_conventions_named()
        {
            var rows = Exceptions(Services, Code, Model);

            var api = rows.Single(x => x.Reason == "v1's conventions over these files (endpoint) didn't report it");
            api.Rule.ShouldBe(RecipeConversion.UnmodelledType);
            api.Matches.ShouldBe(new[] { "ICache", "Cache", "Startup" }, ignoreOrder: true);

            // The endpoint row's recipe files take in the DTO folder too, where the DTO convention's row is.
            rows.Single(x => x.Reason == "v1's conventions over these files (endpoint, DTO) didn't report it").Matches.ShouldBe(new[] { "Helper" });
        }

        [Fact]
        public void A_type_a_convention_found_stays_reported_as_v1_reported_it()
        {
            var rows = Exceptions(Services, Code, Model);

            rows.SelectMany(x => x.Matches).ShouldNotContain("NewController");
            rows.SelectMany(x => x.Matches).ShouldNotContain("UnmodelledDto");
        }

        [Fact]
        public void A_modelled_type_gets_no_exception()
        {
            var rows = Exceptions(Services, Code, Model);

            rows.SelectMany(x => x.Matches).ShouldNotContain("OrdersController");
            rows.SelectMany(x => x.Matches).ShouldNotContain("OrderDto");
            rows.SelectMany(x => x.Matches).ShouldNotContain("LineDto");
        }

        [Fact]
        public void A_type_v1_counted_as_modelled_for_sharing_a_contract_file_is_excepted()
        {
            var rows = Exceptions(Services, Code, Model, contractFiles: new[] { "Dto/OrderDto.cs" });

            rows.Single(x => x.Reason == V1Coverage.ContractFileReason).Matches.ShouldBe(new[] { "RoleAttribute" });
        }

        [Fact]
        public void Without_its_contract_file_a_type_a_convention_found_stays_reported()
        {
            var rows = Exceptions(Services, Code, Model);

            rows.SelectMany(x => x.Matches).ShouldNotContain("RoleAttribute");
        }

        [Fact]
        public void A_type_outside_every_type_row_or_out_of_scope_gets_no_exception()
        {
            var rows = Exceptions(Services.Replace("\"own\": [", "\"exceptions\": [{ \"match\": \"Api/Startup.cs\", \"reason\": \"Hosting\" }], \"own\": ["), Code, Model);

            rows.SelectMany(x => x.Matches).ShouldNotContain("Tool");
            rows.SelectMany(x => x.Matches).ShouldNotContain("Startup");
        }

        [Fact]
        public void A_modelled_types_unmodelled_public_field_is_excepted_and_a_modelled_one_isnt()
        {
            var rows = Exceptions(Services, Code, Model);

            var row = rows.Single(x => x.Reason == V1Coverage.FieldReason);
            row.Rule.ShouldBe(RecipeConversion.UnmodelledMember);
            row.Matches.ShouldBe(new[] { "OrderDto.note" });
        }

        [Fact]
        public void A_service_with_no_type_of_its_name_is_its_interface_and_the_classes_implementing_it()
        {
            var model = Model.Append(new ModelledType("Cache", "Service", new string[0])).ToArray();
            var code = new Dictionary<string, string>(Code) { ["Api/ICache.cs"] = "public interface ICache { } public class RedisCache : ICache { } public class Other { }" };

            var matches = Exceptions(Services, code, model).SelectMany(x => x.Matches).ToList();
            matches.ShouldNotContain("ICache");
            matches.ShouldNotContain("RedisCache");
            matches.ShouldContain("Other");
        }

        [Fact]
        public void A_partial_type_a_convention_found_in_one_of_its_files_stays_reported()
        {
            var code = new Dictionary<string, string>(Code)
            {
                ["Api/ReportsController.cs"] = "public partial class ReportsController : ControllerBase { [HttpGet] public void Get() { } }",
                ["Api/ReportsControllerExt.cs"] = "public partial class ReportsController { private void Helper() { } }"
            };

            Exceptions(Services, code, Model).SelectMany(x => x.Matches).ShouldNotContain("ReportsController");
        }

        [Fact]
        public void A_modelled_classs_interfaces_and_its_base_classs_are_its_code()
        {
            var code = new Dictionary<string, string>(Code)
            {
                ["Api/OrdersController.cs"] = """
                    public interface IOrders : IAudited { }
                    public interface IAudited { }
                    public interface IVersioned { }
                    public abstract class ApiController : ControllerBase, IVersioned { }
                    public class OrdersController : ApiController, IOrders
                    {
                        [HttpGet]
                        public OrderDto Get() => new OrderDto();
                    }
                    """
            };

            var matches = Exceptions(Services, code, Model).SelectMany(x => x.Matches).ToList();
            matches.ShouldNotContain("IOrders");
            matches.ShouldNotContain("IAudited");
            matches.ShouldNotContain("IVersioned");
            matches.ShouldContain("ApiController");
        }

        /// <summary>
        /// A type nested in another is part of it, which the engine never reports on its own, so it gets no exception: one would
        /// match nothing and warn (ClientB's private response classes inside its handlers).
        /// </summary>
        [Fact]
        public void A_type_declared_inside_another_gets_no_exception()
        {
            var code = new Dictionary<string, string>(Code)
            {
                ["Api/OrdersController.cs"] = """
                    public class OrdersController : ControllerBase
                    {
                        [HttpGet]
                        public OrderDto Get() => new OrderDto();

                        public enum ShippingTypes { Standard, Express }
                    }
                    """
            };

            Exceptions(Services, code, Model).SelectMany(x => x.Matches).ShouldNotContain("ShippingTypes");
        }

        /// <summary>
        /// The engine locates one type per element, so a further declaration of a modelled type's name in a row's files is
        /// unmodelled to it, where v1 counted it as modelled (ClientB's second HeartbeatPingRequest, in its API project).
        /// </summary>
        [Fact]
        public void Another_declaration_of_a_modelled_types_name_is_excepted_unless_an_element_has_each()
        {
            var code = new Dictionary<string, string>(Code) { ["Api/Legacy/OrderDto.cs"] = "namespace Legacy { public class OrderDto { } }" };

            var rows = Exceptions(Services, code, Model);
            rows.Single(x => x.Reason == V1Coverage.CopyReason).Matches.ShouldBe(new[] { "OrderDto" });

            var both = Model.Append(new ModelledType("OrderDto", "DTO", new string[0])).ToArray();
            Exceptions(Services, code, both).ShouldNotContain(x => x.Reason == V1Coverage.CopyReason);
        }

        /// <summary>
        /// The engine locates one interface per name a located class implements, so an unused interface of the same name in
        /// another namespace is unmodelled to it, where v1 counted the name as modelled.
        /// </summary>
        [Fact]
        public void Another_declaration_of_a_located_classs_interfaces_name_is_excepted()
        {
            var model = Model.Append(new ModelledType("OrderSender", "Service", new string[0])).ToArray();
            var code = new Dictionary<string, string>(Code)
            {
                ["Api/Senders/OrderSender.cs"] = "namespace Api.Senders { public class OrderSender : IOrderSender { } public interface IOrderSender { } }",
                ["Api/Logic/IOrderSender.cs"] = "namespace Api.Logic { internal interface IOrderSender { } }"
            };

            Exceptions(Services, code, model).Single(x => x.Reason == V1Coverage.CopyReason).Matches.ShouldBe(new[] { "IOrderSender" });

            code.Remove("Api/Logic/IOrderSender.cs");
            Exceptions(Services, code, model).ShouldNotContain(x => x.Reason == V1Coverage.CopyReason);
        }

        /// <summary>
        /// The engine reads a generic type as another type than the one of its name without type parameters, so <c>Result&lt;T&gt;</c>
        /// beside a modelled <c>Result</c> is unmodelled to it, where v1 counted it as modelled by name.
        /// </summary>
        [Fact]
        public void A_generic_type_beside_a_modelled_type_of_its_name_is_another_declaration()
        {
            var model = Model.Append(new ModelledType("Result", "DTO", new[] { "Success" })).ToArray();
            var code = new Dictionary<string, string>(Code)
            {
                ["Dto/Result.cs"] = "public class Result { public bool Success { get; set; } }",
                ["Dto/Result`.cs"] = "public class Result<T> : Result { public T Value { get; set; } }"
            };

            Exceptions(Services, code, model).Single(x => x.Reason == V1Coverage.CopyReason).Matches.ShouldBe(new[] { "Result" });
            V1Coverage.NeverUnmodelled(new CodeFiles(code), model).ShouldNotContain("Result");
        }

        /// <summary>
        /// The engine never reports an override, nor a member every type has, as unmodelled, so v1's exceptions for an entity's
        /// identity comparison would match nothing. A member it does report keeps its exception.
        /// </summary>
        [Fact]
        public void Overrides_and_members_every_type_has_are_never_unmodelled()
        {
            var code = new Dictionary<string, string>
            {
                ["Domain/Provider.cs"] = """
                    public class Provider
                    {
                        public static Provider[] Providers => new Provider[0];
                        public override bool Equals(object obj) => obj is Provider;
                        public override int GetHashCode() => 0;
                        public void Refresh() { }
                    }
                    """
            };

            var never = V1Coverage.NeverUnmodelledMembers(new CodeFiles(code),
                new[] { "Provider.Providers", "Provider.Equals", "Provider.GetHashCode", "Provider.ToString", "Provider.Refresh" });
            never.ShouldBe(new[] { "Provider.Equals", "Provider.GetHashCode", "Provider.ToString" }, ignoreOrder: true);
        }

        /// <summary>
        /// v1's handler contract stated its modifiers only, so it never reported a handler's methods; the engine's handler
        /// conventions state them. The method that takes the modelled message is the subscription's, which the engine locates,
        /// and an override is never reported.
        /// </summary>
        [Fact]
        public void A_handlers_public_method_is_excepted_and_its_handle_method_and_overrides_arent()
        {
            var model = Model.Append(new ModelledType("PortalHandler", "Integration Event Handler", new string[0]) { Messages = new[] { "PortalMessage" } })
                .Append(new ModelledType("PortalMessage", "Message", new string[0])).ToArray();
            var code = new Dictionary<string, string>(Code)
            {
                ["Api/PortalHandler.cs"] = """
                    public class PortalMessage { }
                    public class PortalHandler
                    {
                        public Task Handle(PortalMessage message, object context) => Task.CompletedTask;
                        public static string DeriveCode(decimal rate) => "";
                        public override string ToString() => "";
                        private void Log() { }
                    }
                    """
            };

            var row = Exceptions(Services, code, model).Single(x => x.Reason == V1Coverage.HandlerMethodReason);
            row.Rule.ShouldBe(RecipeConversion.UnmodelledMember);
            row.Matches.ShouldBe(new[] { "PortalHandler.DeriveCode" });
        }

        /// <summary>
        /// A modelled type declared once is never reported as unmodelled, so v1's exception naming it (ClientB's `Container`,
        /// for a relationship its conventions found) would match nothing; one with another declaration may still be reported.
        /// </summary>
        [Fact]
        public void A_modelled_type_declared_once_is_never_unmodelled()
        {
            var never = V1Coverage.NeverUnmodelled(new CodeFiles(Code), Model);
            never.ShouldContain("OrderDto");
            never.ShouldContain("OrdersController");

            var code = new Dictionary<string, string>(Code) { ["Api/Legacy/OrderDto.cs"] = "namespace Legacy { public class OrderDto { } }" };
            V1Coverage.NeverUnmodelled(new CodeFiles(code), Model).ShouldNotContain("OrderDto");
        }

        /// <summary>
        /// A ClientB application keeps a legacy copy of its host project that its recipe never read. Under the ** In row the
        /// engine read both copies of each class as one type, and the v1-clean model had 44 errors; the copy goes out.
        /// </summary>
        [Fact]
        public void A_project_no_recipe_file_reached_and_no_contract_file_is_in_is_out()
        {
            var code = new Dictionary<string, string>
            {
                ["Host/Host.csproj"] = "<Project />",
                ["Host/Handlers/OrderHandler.cs"] = "public class OrderHandler { }",
                ["Host.Legacy/Host.Legacy.csproj"] = "<Project />",
                ["Host.Legacy/Handlers/OrderHandler.cs"] = "public class OrderHandler { }",
                ["Api/Api.csproj"] = "<Project />",
                ["Api/Controllers/OrdersController.cs"] = "public class OrdersController { }",
                ["Contracts/Contracts.csproj"] = "<Project />",
                ["Contracts/OrderDto.cs"] = "public class OrderDto { }",
                ["Database/Database.sqlproj"] = "<Project />",
                ["Database/Tables/Order.sql"] = "CREATE TABLE [Order] ([Id] INT)"
            };
            var rows = RecipeConversion.Convert(new[]
            {
                new RecipeSource("services.drift.json", """
                    {
                      "files": "Api/Controllers/**/*.cs",
                      "facts": { "intent.invokes": { "files": ["Host/Handlers/**/*.cs"], "find": "class:{Service}" } }
                    }
                    """)
            });

            var row = V1Coverage.UnreadProjects(new CodeFiles(code), rows.V1Read, new[] { "Contracts/OrderDto.cs" });

            row.Scope.ShouldBe("Out");
            row.Globs.ShouldBe(new[] { "Host.Legacy/**" });
            row.Reason.ShouldBe(V1Coverage.UnreadReason);
        }

        [Fact]
        public void A_recipe_without_files_read_every_project_so_none_is_out()
        {
            var code = new Dictionary<string, string>
            {
                ["Host/Host.csproj"] = "<Project />",
                ["Host/OrderHandler.cs"] = "public class OrderHandler { }",
                ["Tests/Tests.csproj"] = "<Project />",
                ["Tests/OrderHandlerTests.cs"] = "public class OrderHandlerTests { }"
            };
            var rows = RecipeConversion.Convert(new[] { new RecipeSource("domain.drift.json", "{}") });

            V1Coverage.UnreadProjects(new CodeFiles(code), rows.V1Read, new string[0]).ShouldBeNull();
        }

        [Fact]
        public void A_project_inside_an_unread_one_is_not_listed_again_and_one_inside_a_read_one_is()
        {
            var code = new Dictionary<string, string>
            {
                ["Legacy/Legacy.csproj"] = "<Project />",
                ["Legacy/Old.cs"] = "public class Old { }",
                ["Legacy/Plugins/Plugins.csproj"] = "<Project />",
                ["Legacy/Plugins/Plugin.cs"] = "public class Plugin { }",
                ["Api/Api.csproj"] = "<Project />",
                ["Api/Controllers/OrdersController.cs"] = "public class OrdersController { }",
                ["Api/Tools/Tools.csproj"] = "<Project />",
                ["Api/Tools/Seed.cs"] = "public class Seed { }"
            };
            var rows = RecipeConversion.Convert(new[] { new RecipeSource("services.drift.json", """{ "files": "Api/Controllers/**/*.cs" }""") });

            V1Coverage.UnreadProjects(new CodeFiles(code), rows.V1Read, new string[0]).Globs.ShouldBe(new[] { "Api/Tools/**", "Legacy/**" });
        }

        /// <summary>The engine never reads a copy in a project out of scope, so the modelled type is still declared once to it.</summary>
        [Fact]
        public void A_copy_in_a_project_out_of_scope_is_never_located()
        {
            var code = new Dictionary<string, string>(Code) { ["Legacy/OrderDto.cs"] = "namespace Legacy { public class OrderDto { } }" };
            var scope = new[] { new ScopeRowSpec(new[] { "**" }, "In", "All"), new ScopeRowSpec(new[] { "Legacy/**" }, "Out", V1Coverage.UnreadReason) };

            V1Coverage.NeverUnmodelled(new CodeFiles(code), Model).ShouldNotContain("OrderDto");
            V1Coverage.NeverUnmodelled(new CodeFiles(code), Model, scope).ShouldContain("OrderDto");
        }

        /// <summary>
        /// A migrated EF6 application keeps legacy copies of its entities in Data/Models, beside the Data/Contracts its recipe
        /// read. In scope, each entity had two types and none could be verified; the folder goes out. A copy beside files v1
        /// read goes out on its own, and a folder v1 didn't read that holds no copy stays in.
        /// </summary>
        [Fact]
        public void Code_v1_never_read_that_copies_a_modelled_type_is_out()
        {
            var code = new Dictionary<string, string>
            {
                ["Data/Data.csproj"] = "<Project />",
                ["Data/Contracts/Order.cs"] = "namespace Data.Contracts { public partial class Order { } }",
                ["Data/Contracts/Old/Order.cs"] = "namespace Data.Contracts.Old { public class Order { } }",
                ["Data/Contracts/Line.cs"] = "namespace Data.Contracts { public class Line { } }",
                ["Data/Models/Order.cs"] = "namespace Data.Models { public class Order { } }",
                ["Data/Models/Sub/Line.cs"] = "namespace Data.Models.Sub { public class Line { } }",
                ["Data/Migrations/Initial.cs"] = "namespace Data.Migrations { public class Initial { } }",
                ["Data/Context.cs"] = "namespace Data { public class Context { } }"
            };
            var rows = RecipeConversion.Convert(new[]
            {
                new RecipeSource("domain.drift.json", """{ "files": ["Data/Contracts/*.cs", "Data/Context.cs"] }""")
            });
            var model = new[] { new ModelledType("Order", "Class", new string[0]), new ModelledType("Line", "Class", new string[0]) };

            var row = V1Coverage.UnreadCopies(new CodeFiles(code), rows.V1Read, new string[0], model, rows.Scope);

            row.Scope.ShouldBe("Out");
            row.Globs.ShouldBe(new[] { "Data/Contracts/Old/**", "Data/Models/**" });
            row.Reason.ShouldBe(V1Coverage.CopiesReason);
        }

        [Fact]
        public void A_type_only_code_v1_never_read_declares_is_no_copy()
        {
            var code = new Dictionary<string, string>
            {
                ["Data/Data.csproj"] = "<Project />",
                ["Data/Contracts/Order.cs"] = "namespace Data.Contracts { public class Order { } }",
                ["Data/Models/Line.cs"] = "namespace Data.Models { public class Line { } }"
            };
            var rows = RecipeConversion.Convert(new[] { new RecipeSource("domain.drift.json", """{ "files": "Data/Contracts/*.cs" }""") });
            var model = new[] { new ModelledType("Order", "Class", new string[0]), new ModelledType("Line", "Class", new string[0]) };

            V1Coverage.UnreadCopies(new CodeFiles(code), rows.V1Read, new string[0], model, rows.Scope).ShouldBeNull();
        }

        /// <summary>
        /// A migrated application models the commands it sends to and takes from a shared library project its recipes never
        /// reached, which the Out row takes out: those files are read back in for context, so the types can be located.
        /// </summary>
        [Fact]
        public void A_file_out_of_scope_declaring_a_modelled_type_no_file_in_scope_declares_is_read_for_context()
        {
            var code = new Dictionary<string, string>
            {
                ["Host/Host.csproj"] = "<Project />",
                ["Host/Handlers/Relay.cs"] = "public class Relay { }",
                ["Shared/Shared.csproj"] = "<Project />",
                ["Shared/Commands/SubmitApplication.cs"] = "namespace Shared.Commands { public class SubmitApplication { } }",
                ["Shared/Commands/Relay.cs"] = "namespace Shared.Commands { public class Relay { } }",
                ["Shared/Commands/Unmodelled.cs"] = "namespace Shared.Commands { public class Unmodelled { } }"
            };
            var scope = new[] { new ScopeRowSpec(new[] { "**" }, "In", "All"), new ScopeRowSpec(new[] { "Shared/**" }, "Out", V1Coverage.UnreadReason) };
            var model = new[] { new ModelledType("SubmitApplication", "Integration Command", new string[0]), new ModelledType("Relay", "Integration Command", new string[0]) };

            var row = V1Coverage.UnreadModelled(new CodeFiles(code), model, scope);

            row.Scope.ShouldBe("Context");
            row.Globs.ShouldBe(new[] { "Shared/Commands/SubmitApplication.cs" });
            row.Reason.ShouldBe(V1Coverage.ModelledUnreadReason);
        }

        /// <summary>
        /// v1 compared a modelled type's members only in the contract file it generated for it, so a migrated EF6 application's
        /// T4-generated entities had none reported: their unmodelled public members are excepted, of the kinds the element
        /// states, its attributes and association ends aside. Where v1 compared them, a constructor counted only when the model
        /// stated one for the type.
        /// </summary>
        [Fact]
        public void A_modelled_types_members_v1_never_compared_are_excepted_and_a_compared_types_constructor_when_the_model_has_none()
        {
            var model = new[]
            {
                new ModelledType("Order", "Class", new[] { "Id", "Lines" }),
                new ModelledType("Invoice", "Class", new[] { "Id" }),
                new ModelledType("Receipt", "Class", new[] { "Id", "Receipt" }, HasConstructors: true),
                new ModelledType("Orders", "Service", new[] { "Get" })
            };
            var code = new Dictionary<string, string>
            {
                ["Data/Order.cs"] = """
                    public class Order
                    {
                        public Order() { }
                        public int Id { get; set; }
                        public List<Line> Lines { get; set; }
                        public string Note { get; set; }
                        public void Touch() { }
                        public override string ToString() => "";
                        private void Hidden() { }
                    }
                    """,
                ["Data/Invoice.cs"] = "public class Invoice { public Invoice() { } public int Id { get; set; } }",
                ["Data/Receipt.cs"] = "public class Receipt { public Receipt() { } public int Id { get; set; } }",
                ["Data/Orders.cs"] = "public class Orders { public Orders(object db) { } public void Get() { } }"
            };
            var recipes = new[] { new RecipeSource("domain.drift.json", "{}") };
            var rows = RecipeConversion.Convert(recipes);
            rows.OptIn.Add(new OptInRowSpec(RecipeConversion.UnmodelledMember, new[] { "**/*.cs" }, "Public members of modelled types"));

            var result = V1Coverage.Exceptions(new CodeFiles(code), recipes, rows, new[] { "Data/Invoice.cs", "Data/Receipt.cs", "Data/Orders.cs" }, model, new List<string>());

            result.Single(x => x.Reason == V1Coverage.UncomparedReason).Matches.ShouldBe(new[] { "Order.Order", "Order.Note", "Order.Touch" });
            result.Single(x => x.Reason == V1Coverage.ConstructorReason).Matches.ShouldBe(new[] { "Invoice.Invoice" });
            result.ShouldAllBe(x => x.Rule == RecipeConversion.UnmodelledMember);
        }

        /// <summary>
        /// A queue consumer makes the message it handles from the body it receives (an Azure Service Bus processor's
        /// <c>JsonConvert.DeserializeObject&lt;ApplicationPack&gt;(json)</c>): that method is the subscription's, which the engine
        /// locates, so it isn't excepted.
        /// </summary>
        [Fact]
        public void A_handlers_method_making_a_modelled_message_from_what_it_receives_is_not_excepted()
        {
            var model = Model.Append(new ModelledType("OrderConsumer", "Integration Event Handler", new string[0]) { Messages = new[] { "PlaceOrder" } })
                .Append(new ModelledType("PlaceOrder", "Integration Command", new string[0])).ToArray();
            var code = new Dictionary<string, string>(Code)
            {
                ["Api/OrderConsumer.cs"] = """
                    public class OrderConsumer
                    {
                        public Task HandleOrder(ProcessMessageEventArgs args) { var order = JsonConvert.DeserializeObject<Contracts.PlaceOrder>(args.Body); return Task.CompletedTask; }
                        public Task StartAsync(CancellationToken token) => Task.CompletedTask;
                    }
                    """
            };

            Exceptions(Services, code, model).Single(x => x.Reason == V1Coverage.HandlerMethodReason).Matches.ShouldBe(new[] { "OrderConsumer.StartAsync" });
        }

        /// <summary>
        /// Only the handler's own messages are its subscription's code, as the engine reads it, over every part of a partial
        /// handler. <c>QuoteCompletedHandler</c> subscribes to <c>QuoteCompleted</c>: its <c>Handle</c> and the implementation
        /// method in its other file take it, and its static helper deserialising the modelled DTO <c>OperationData</c> doesn't
        /// handle anything, so only the helper is excepted. <c>PackGeneratedHandler</c> handles another application's message,
        /// with no subscription link to it, so the engine locates none of its methods and reports each, the implementation
        /// method that deserialises <c>OperationData</c> included. Both were clean under v1 in a migrated NServiceBus application.
        /// </summary>
        [Fact]
        public void A_handlers_methods_are_its_subscriptions_only_for_its_own_messages_over_every_part()
        {
            var model = Model.Append(new ModelledType("QuoteCompletedHandler", "Integration Event Handler", new string[0]) { Messages = new[] { "QuoteCompleted" } })
                .Append(new ModelledType("PackGeneratedHandler", "Integration Event Handler", new string[0]))
                .Append(new ModelledType("QuoteCompleted", "Integration Command", new string[0]))
                .Append(new ModelledType("OperationData", "DTO", new string[0])).ToArray();
            var code = new Dictionary<string, string>(Code)
            {
                ["Api/Infrastructure/QuoteCompletedHandler.cs"] = """
                    public partial class QuoteCompletedHandler : IHandleMessages<QuoteCompleted>
                    {
                        public Task Handle(QuoteCompleted message, IMessageHandlerContext context) => HandleImplementationAsync(message);
                    }
                    """,
                ["Api/Handlers/QuoteCompletedHandler.cs"] = """
                    public partial class QuoteCompletedHandler
                    {
                        public Task HandleImplementationAsync(QuoteCompleted message) => Task.CompletedTask;
                        public static OperationData DeserializeOperationData(string json) => System.Text.Json.JsonSerializer.Deserialize<OperationData>(json);
                    }
                    """,
                ["Api/Infrastructure/PackGeneratedHandler.cs"] = """
                    public partial class PackGeneratedHandler : IHandleMessages<Partner.Contracts.PackGenerated>
                    {
                        public Task Handle(Partner.Contracts.PackGenerated message, IMessageHandlerContext context) => HandleImplementationAsync(message);
                    }
                    """,
                ["Api/Handlers/PackGeneratedHandler.cs"] = """
                    public partial class PackGeneratedHandler
                    {
                        public Task HandleImplementationAsync(Partner.Contracts.PackGenerated message)
                        {
                            var data = System.Text.Json.JsonSerializer.Deserialize<OperationData>(message.Value);
                            return Task.CompletedTask;
                        }
                    }
                    """
            };

            Exceptions(Services, code, model).Single(x => x.Reason == V1Coverage.HandlerMethodReason).Matches.OrderBy(x => x, System.StringComparer.Ordinal).ShouldBe(new[]
            {
                "PackGeneratedHandler.Handle", "PackGeneratedHandler.HandleImplementationAsync", "QuoteCompletedHandler.DeserializeOperationData"
            });
        }

        /// <summary>
        /// A handler's subscribed messages are the elements its subscription links reach, named from any package read, another
        /// application's included; another kind of link, or one to an element no package holds, names none.
        /// </summary>
        [Fact]
        public void A_handlers_messages_are_what_its_subscription_links_reach_in_any_package()
        {
            static AssociationPersistable Link(string type, string source, string target) => new()
            {
                Id = System.Guid.NewGuid().ToString(),
                AssociationType = type,
                SourceEnd = new AssociationEndPersistable { Id = System.Guid.NewGuid().ToString(), TypeReference = new TypeReferencePersistable { TypeId = source } },
                TargetEnd = new AssociationEndPersistable { Id = System.Guid.NewGuid().ToString(), TypeReference = new TypeReferencePersistable { TypeId = target } }
            };
            var partner = new PackageModelPersistable { Id = "partner", Name = "Partner.Messages" };
            partner.Classes.Add(new ElementPersistable { Id = "pack-generated", Name = "PackGenerated", SpecializationType = "Integration Event" });
            var services = new PackageModelPersistable { Id = "services", Name = "Services" };
            services.Classes.Add(new ElementPersistable { Id = "quote-handler", Name = "QuoteCompletedHandler", SpecializationType = "Integration Event Handler" });
            services.Classes.Add(new ElementPersistable { Id = "quote-completed", Name = "QuoteCompleted", SpecializationType = "Integration Command" });
            services.Classes.Add(new ElementPersistable { Id = "pack-handler", Name = "PackGeneratedHandler", SpecializationType = "Integration Event Handler" });
            services.Associations.Add(Link("Subscribe Integration Command", "quote-handler", "quote-completed"));
            services.Associations.Add(Link("Subscribe Integration Event", "pack-handler", "pack-generated"));
            services.Associations.Add(Link("Send Command", "pack-handler", "quote-completed"));
            services.Associations.Add(Link("Subscribe Integration Event", "quote-handler", "no-such-element"));

            var messages = DriftEngineMigration.SubscribedMessages(new[] { services }, new[] { services, partner });

            messages["quote-handler"].ShouldBe(new[] { "QuoteCompleted" });
            messages["pack-handler"].ShouldBe(new[] { "PackGenerated" });
        }

        /// <summary>
        /// A Razor or Vue file is one component of its file's name to the engine, and v1 found it with a file find. eShop10's UI
        /// conventions left out the files holding &lt;html&gt; and &lt;Router&gt;, so its App and Routes were reported after
        /// the move. A modelled component is located, so it needs no exception, nor does an _Imports.razor.
        /// </summary>
        [Fact]
        public void A_component_file_no_convention_found_is_excepted_by_its_component_name()
        {
            const string ui = """
                {
                  "files": ["src/*/Components/**/*.razor", "src/*/Components/**/*.vue"],
                  "own": [
                    { "kind": "page", "find": "file with @page\"", "files": ["src/*/Components/**/*.razor"] },
                    { "kind": "component", "find": "file with ^(?!.*(?:@page\"|<Router\\b|<html\\b)).*<\\w", "files": ["src/*/Components/**/*.razor", "src/*/Components/**/*.vue"] }
                  ]
                }
                """;
            var code = new Dictionary<string, string>
            {
                ["src/Web/Components/App.razor"] = "<!DOCTYPE html>\n<html>\n<body><Routes /></body>\n</html>",
                ["src/Web/Components/Routes.razor"] = "<Router AppAssembly=\"typeof(Program).Assembly\" />",
                ["src/Web/Components/_Imports.razor"] = "@using Web",
                ["src/Web/Components/Pages/Catalog.razor"] = "@page \"/\"\n<div>Catalog</div>",
                ["src/Web/Components/Layout/MainLayout.razor"] = "@inherits LayoutComponentBase\n@Body",
                ["src/Web/Components/Item-Card.razor"] = "@Item.Name",
                ["src/Web/Components/Cart.vue"] = "<template><div /></template>",
                ["src/Web/Components/Shell.vue"] = "<template><Router /></template>"
            };
            var recipes = new[] { new RecipeSource("ui.drift.json", ui) };
            var rows = RecipeConversion.Convert(recipes);
            var notes = new List<string>();

            var result = V1Coverage.Exceptions(new CodeFiles(code), recipes, rows, new string[0], new ModelledType[0], notes, new[] { "MainLayout" });

            notes.ShouldBeEmpty();
            result.ShouldAllBe(x => x.Rule == RecipeConversion.UnmodelledType);
            result.Single(x => x.Reason == "v1's conventions over these files (page, component) didn't report it").Matches
                .ShouldBe(new[] { "App", "Routes", "Item_Card" }, ignoreOrder: true);
            // Only the component convention covers .vue files.
            result.Single(x => x.Reason == "v1's conventions over these files (component) didn't report it").Matches.ShouldBe(new[] { "Shell" });
        }

        private static IReadOnlyList<ExceptionRowSpec> Exceptions(string services, Dictionary<string, string> code, ModelledType[] model, string[] contractFiles = null)
        {
            var recipes = new[] { new RecipeSource("services.drift.json", services) };
            var rows = RecipeConversion.Convert(recipes);
            var notes = new List<string>();
            var result = V1Coverage.Exceptions(new CodeFiles(code), recipes, rows, contractFiles ?? new string[0], model, notes);
            notes.ShouldBeEmpty();
            return result;
        }
    }
}
