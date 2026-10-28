using System;
using System.Collections.Generic;
using Intent.Configuration;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modelers.Domain.Repositories.Api;
using Intent.Modelers.Domain.ValueObjects.Api;
using Intent.Modelers.Services.Api;
using Intent.Modelers.Services.CQRS.Api;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.FileBuilders;
using Intent.Modules.Domain.Guardrails.Contracts.Drift;
using Intent.Templates;
using NSubstitute;
using Shouldly;
using Xunit;
using static Intent.Modules.Guardrails.Contracts.Tests.Drift;
using static Intent.Modules.Guardrails.Contracts.Tests.Fakes;
using DomainBuilders = Intent.Modules.Domain.Guardrails.Contracts.Templates.ContractTypeBuilders;
using DomainReportTemplate = Intent.Modules.Domain.Guardrails.Contracts.Templates.DriftReport.DriftReportTemplate;
using ServicesBuilders = Intent.Modules.Services.Guardrails.Contracts.Templates.ContractTypeBuilders;
using ServicesDrift = Intent.Modules.Services.Guardrails.Contracts.Drift;
using ServicesReportTemplate = Intent.Modules.Services.Guardrails.Contracts.Templates.DriftReport.DriftReportTemplate;

namespace Intent.Modules.Guardrails.Contracts.Tests
{
    /// <summary>
    /// The structure comparison reads only what the element's designer can state: the drift check keeps what is modelled
    /// in step with the code, so something the model has no way to express is never a row (user decision, 2026-10-23).
    /// </summary>
    public class ModellableTests
    {
        private const string Path = "src/Ordering.cs";

        private static readonly Modellable Service = new() { Kinds = new[] { "class", "record" }, Methods = true };
        private static readonly Modellable Handler = new() { Kinds = new[] { "class" } };
        private static readonly Modellable RecordDto = new() { Kinds = new[] { "class", "record" }, TypeParameters = true, ParameterList = true, Properties = true };
        private static readonly Modellable ClassDto = new() { Kinds = new[] { "class", "record" }, TypeParameters = true, Properties = true };
        private static readonly Modellable Proxy = new() { Kinds = new[] { "class", "interface" }, Methods = true };

        private static DriftReport Compare(string generated, string onDisk, string type = null, Modellable modellable = null)
        {
            return Check("Services", "{}", new List<ModelFact>(),
                new Dictionary<string, string> { [Path] = onDisk },
                new[]
                {
                    new GeneratedFile
                    {
                        RelativePath = Path,
                        Text = generated,
                        ModellableTypes = type == null ? null : new Dictionary<string, Modellable> { [type] = modellable }
                    }
                });
        }

        private const string GeneratedQueries = """
        namespace Ordering.API
        {
            public class OrderQueries
            {
                public Task<Order> GetOrderAsync(int id)
                {
                }
            }
        }
        """;

        private const string Queries = """
        namespace Ordering.API
        {
            public class OrderQueries(OrderingContext context) : IOrderQueries
            {
                public bool IsCached { get; set; }

                public Task<Order> GetOrderAsync(int id) => context.Orders.FindAsync(id).AsTask();

                public Task<IEnumerable<CardType>> GetCardTypesAsync() => Task.FromResult(Enumerable.Empty<CardType>());
            }
        }
        """;

        [Fact]
        public void A_Services_primary_constructor_and_properties_are_not_compared()
        {
            // Its primary constructor holds its injected dependencies, and a Service has operations, not properties.
            Compare(GeneratedQueries, Queries, "OrderQueries", Service).WrongLines.ShouldBe(new[]
            {
                "`OrderQueries.GetCardTypesAsync()` (public method): not modelled. Location: `src/Ordering.cs:9`"
            });
        }

        [Fact]
        public void A_type_whose_template_says_nothing_is_compared_on_everything()
        {
            Compare(GeneratedQueries, Queries).WrongLines.ShouldBe(new[]
            {
                "`OrderQueries` parameters: model (none), code `(OrderingContext context)`. Location: `src/Ordering.cs:3`",
                "`OrderQueries.GetCardTypesAsync()` (public method): not modelled. Location: `src/Ordering.cs:9`",
                "`OrderQueries.IsCached` (public property): not modelled. Location: `src/Ordering.cs:5`"
            }, ignoreOrder: true);
        }

        [Fact]
        public void A_handler_is_compared_on_its_modifiers_only()
        {
            const string generated = """
            namespace Ordering.API
            {
                public class OrderStartedIntegrationEventHandler
                {
                }
            }
            """;
            const string onDisk = """
            namespace Ordering.API
            {
                public record OrderStartedIntegrationEventHandler(IMediator mediator) : IIntegrationEventHandler<OrderStartedIntegrationEvent>
                {
                    private readonly int _attempts;

                    public Task Handle(OrderStartedIntegrationEvent @event) => mediator.Send(new SetAwaitingValidationOrderStatusCommand(@event.OrderId));
                }
            }
            """;

            // A handler is always a class and its members aren't modelled, so neither the record nor Handle is a row.
            Compare(generated, onDisk, "OrderStartedIntegrationEventHandler", Handler).WrongLines.ShouldBeEmpty();
            Compare(generated, onDisk.Replace("public record", "internal record"), "OrderStartedIntegrationEventHandler", Handler).WrongLines.ShouldBe(new[]
            {
                "`OrderStartedIntegrationEventHandler` modifiers: model `public`, code `internal`. Location: `src/Ordering.cs:3`"
            });
        }

        [Fact]
        public void A_DTOs_methods_are_not_compared_but_its_properties_are()
        {
            const string generated = """
            namespace Ordering.API
            {
                public record OrderDraftDTO
                {
                    public decimal Total { get; init; }
                }
            }
            """;
            const string onDisk = """
            namespace Ordering.API
            {
                public record OrderDraftDTO
                {
                    public decimal Total { get; init; }
                    public string Currency { get; init; }

                    public static OrderDraftDTO FromOrder(Order order) => new() { Total = order.Total };
                }
            }
            """;

            Compare(generated, onDisk, "OrderDraftDTO", RecordDto).WrongLines.ShouldBe(new[]
            {
                "`OrderDraftDTO.Currency` (public property): not modelled. Location: `src/Ordering.cs:6`"
            });
        }

        [Fact]
        public void A_record_the_model_could_declare_positional_is_compared_on_its_parameters()
        {
            const string generated = """
            namespace Ordering.API
            {
                public record OrderDraftDTO
                {
                    public decimal Total { get; init; }
                }
            }
            """;

            Compare(generated, "namespace Ordering.API\n{\n    public record OrderDraftDTO(decimal Total);\n}\n", "OrderDraftDTO", RecordDto).WrongLines.ShouldBe(new[]
            {
                "`OrderDraftDTO` parameters: model (none), code `(decimal Total)`. Location: `src/Ordering.cs:3`"
            });
        }

        [Fact]
        public void A_class_DTOs_primary_constructor_is_not_compared()
        {
            // Only a positional record puts parameters on a DTO, so a class's primary constructor is nothing it can state.
            const string generated = """
            namespace Ordering.API
            {
                public class BasketItem
                {
                    public string Id { get; set; }
                }
            }
            """;
            const string onDisk = """
            namespace Ordering.API
            {
                public class BasketItem(string id)
                {
                    public string Id { get; set; } = id;
                }
            }
            """;

            Compare(generated, onDisk, "BasketItem", ClassDto).WrongLines.ShouldBeEmpty();
        }

        [Fact]
        public void Generic_parameters_are_compared_only_where_the_model_can_state_them()
        {
            const string generated = "namespace Ordering.API\n{\n    public class OrderQueries\n    {\n    }\n}\n";
            const string onDisk = "namespace Ordering.API\n{\n    public class OrderQueries<T>\n    {\n    }\n}\n";

            Compare(generated, onDisk, "OrderQueries", Service).WrongLines.ShouldBeEmpty();
            Compare(generated, onDisk, "OrderQueries", ClassDto).WrongLines.ShouldBe(new[]
            {
                "`OrderQueries` type parameters: model (none), code `<T>`. Location: `src/Ordering.cs:3`"
            });
        }

        [Fact]
        public void A_kind_is_compared_only_when_the_model_can_declare_the_type_as_it()
        {
            const string generated = "namespace Ordering.API\n{\n    public class CatalogService\n    {\n    }\n}\n";

            Compare(generated, "namespace Ordering.API\n{\n    public interface CatalogService\n    {\n    }\n}\n", "CatalogService", Proxy).WrongLines.ShouldBe(new[]
            {
                "`CatalogService` kind: model `class`, code `interface`. Location: `src/Ordering.cs:3`"
            });
            Compare(generated, "namespace Ordering.API\n{\n    public record CatalogService\n    {\n    }\n}\n", "CatalogService", Proxy).WrongLines.ShouldBeEmpty();
        }

        [Fact]
        public void No_designer_models_a_field_an_event_or_an_indexer()
        {
            const string generated = "namespace Ordering.API\n{\n    public class Order\n    {\n    }\n}\n";
            const string onDisk = """
            namespace Ordering.API
            {
                public class Order
                {
                    public int Count;
                    public event EventHandler Changed;
                    public string this[int index] => index.ToString();
                }
            }
            """;

            Compare(generated, onDisk).WrongLines.ShouldBeEmpty();
        }

        [Fact]
        public void The_Services_builders_record_what_each_element_can_state()
        {
            var package = Package();
            var service = Element(package, "Service", "OrderQueries");
            service.SpecializationTypeId.Returns(ServiceModel.SpecializationTypeId);
            var command = Element(package, "Command", "CancelOrderCommand");
            command.SpecializationTypeId.Returns(CommandModel.SpecializationTypeId);
            var handler = Element(package, "Integration Event Handler", "OrderStartedIntegrationEventHandler");
            var proxy = Element(package, "Service Proxy", "CatalogService");

            var file = Build("Ordering.API", (contracts, template) =>
            {
                ServicesBuilders.AddService(contracts, template, new ServiceModel(service));
                ServicesBuilders.AddCommand(contracts, template, new CommandModel(command));
                ServicesBuilders.AddIntegrationEventHandler(contracts, template, handler);
                ServicesBuilders.AddServiceProxy(contracts, template, proxy);
            });
            const string onDisk = """
            namespace Ordering.API
            {
                public class OrderQueries(OrderingContext context) : IOrderQueries
                {
                    public bool IsCached { get; set; }

                    public Task<Order> GetOrderAsync(int id) => context.Orders.FindAsync(id).AsTask();
                }

                public class CancelOrderCommand
                {
                    public int OrderNumber { get; set; }

                    public override string ToString() => $"{OrderNumber}";
                }

                public class OrderStartedIntegrationEventHandler(IMediator mediator, ILogger<OrderStartedIntegrationEventHandler> logger) : IIntegrationEventHandler<OrderStartedIntegrationEvent>
                {
                    private readonly int _attempts;

                    public Task Handle(OrderStartedIntegrationEvent @event) => mediator.Send(new SetAwaitingValidationOrderStatusCommand(@event.OrderId));
                }

                public class CatalogService(HttpClient httpClient)
                {
                    public string BaseUrl { get; } = "api/catalog";

                    public Task<CatalogItem> GetCatalogItem(int id) => httpClient.GetFromJsonAsync<CatalogItem>($"{BaseUrl}/items/{id}");
                }
            }
            """;

            var report = ServicesDrift.DriftCheck.Run(new ServicesDrift.DriftCheckInput
            {
                Designer = "Services",
                RecipePath = "guardrails/services.drift.json",
                RecipeJson = "{}",
                Code = new ServicesDrift.CodeFiles(new Dictionary<string, string> { [Path] = onDisk }),
                Facts = Array.Empty<ServicesDrift.ModelFact>(),
                Generated = new[] { new ServicesDrift.GeneratedFile { RelativePath = Path, Text = file.ToString(), ModellableTypes = ServicesReportTemplate.ModellableTypes(file) } }
            });

            // No row for a primary constructor, a Service's or proxy's property, a command's method or anything on a handler.
            report.WrongLines.ShouldBe(new[]
            {
                "`CancelOrderCommand.OrderNumber` (public property): not modelled. Location: `src/Ordering.cs:12`",
                "`CatalogService.GetCatalogItem(int)` (public method): not modelled. Location: `src/Ordering.cs:28`",
                "`OrderQueries.GetOrderAsync(int)` (public method): not modelled. Location: `src/Ordering.cs:7`"
            }, ignoreOrder: true);
        }

        [Fact]
        public void The_Domain_builders_record_what_each_element_can_state()
        {
            var package = Package();
            var repository = Element(package, "Repository", "OrderRepository");
            repository.SpecializationTypeId.Returns(RepositoryModel.SpecializationTypeId);
            var money = Element(package, "Value Object", "Money");
            money.SpecializationTypeId.Returns(ValueObjectModel.SpecializationTypeId);

            var file = Build("Ordering.Domain", (contracts, template) =>
            {
                DomainBuilders.AddRepository(contracts, template, new RepositoryModel(repository));
                DomainBuilders.AddValueObject(contracts, template, new ValueObjectModel(money));
            });
            const string onDisk = """
            namespace Ordering.Domain
            {
                public class OrderRepository(OrderingContext context) : IOrderRepository
                {
                    public IUnitOfWork UnitOfWork => context;

                    public Order Add(Order order) => context.Orders.Add(order).Entity;
                }

                public class Money
                {
                    public decimal Amount { get; }

                    public override string ToString() => $"{Amount}";
                }
            }
            """;

            var report = Check("Domain", "{}", new List<ModelFact>(), new Dictionary<string, string> { [Path] = onDisk },
                new[] { new GeneratedFile { RelativePath = Path, Text = file.ToString(), ModellableTypes = DomainReportTemplate.ModellableTypes(file) } });

            // No row for the repository's primary constructor or property, or the value object's method.
            report.WrongLines.ShouldBe(new[]
            {
                "`Money.Amount` (public property): not modelled. Location: `src/Ordering.cs:12`",
                "`OrderRepository.Add(Order)` (public method): not modelled. Location: `src/Ordering.cs:7`"
            }, ignoreOrder: true);
        }

        [Fact]
        public void The_drift_template_reads_what_the_builder_recorded_on_each_type()
        {
            var file = new CSharpFile("Acme", "src").AddClass("Order").AddInterface("IOrderRepository").AddEnum("OrderStatus");
            var order = new Modellable();
            var repository = new Modellable();
            var status = new Modellable();
            file.Classes[0].AddMetadata(Modellable.MetadataKey, order);
            file.Interfaces[0].AddMetadata(Modellable.MetadataKey, repository);
            file.Enums[0].AddMetadata(Modellable.MetadataKey, status);

            var read = DomainReportTemplate.ModellableTypes(file);

            read.Count.ShouldBe(3);
            read["Order"].ShouldBeSameAs(order);
            read["IOrderRepository"].ShouldBeSameAs(repository);
            read["OrderStatus"].ShouldBeSameAs(status);
        }

        /// <summary>A file built by contract builders, as the Software Factory builds a file builder template's file.</summary>
        private static CSharpFile Build(string @namespace, Action<CSharpFile, ICSharpTemplate> add)
        {
            // An unset Default Type Modifiers setting: types are public.
            var group = Substitute.For<IGroupSettings>();
            group.GetSetting(Arg.Any<string>()).Returns((ISetting)null);
            var template = Substitute.For<ICSharpTemplate>();
            template.ExecutionContext.Settings.GetGroup(Arg.Any<string>()).Returns(group);

            var file = new CSharpFile(@namespace, "src");
            add(file, template);

            var fileTemplate = Substitute.For<IFileBuilderTemplate>();
            fileTemplate.File.Returns(file);
            fileTemplate.CanRunTemplate().Returns(true);
            var outputTarget = Substitute.For<IOutputTarget>();
            outputTarget.TemplateInstances.Returns(new List<ITemplate> { fileTemplate });
            var context = Substitute.For<ISoftwareFactoryExecutionContext>();
            context.OutputTargets.Returns(new[] { outputTarget });
            FileBuilderHelper.PerformConfiguration(context, isForAfterBuild: false);
            FileBuilderHelper.PerformConfiguration(context, isForAfterBuild: true);
            return file;
        }
    }
}
