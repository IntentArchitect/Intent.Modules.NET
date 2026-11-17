using System.Collections.Generic;
using System.Linq;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.Drift;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Shouldly;
using Xunit;
using static Intent.Modules.Guardrails.Contracts.Tests.Drift;

namespace Intent.Modules.Guardrails.Contracts.Tests
{
    /// <summary>
    /// An empty primary constructor (<c>record OrderShipped()</c>) puts no parameters on the type, which is all the C# builder
    /// can write for it, but it is a declared parameterless constructor: compared as one where the type's other constructors
    /// make it matter.
    /// </summary>
    public class EmptyPrimaryConstructorTests
    {
        private const string Path = "src/Ordering.cs";

        private static readonly Modellable DomainEvent = new() { Kinds = new[] { "class", "record" }, TypeParameters = true, ParameterList = true, Properties = true };
        private static readonly Modellable DomainClass = new()
        {
            Kinds = new[] { "class", "record" }, TypeParameters = true, ParameterList = true, Properties = true, Methods = true, Constructors = true
        };

        private const string GeneratedOrder = """
        namespace Ordering.Domain
        {
            public class Order
            {
                public Order(Guid id)
                {
                }
            }
        }
        """;

        private const string OrderWithEmptyPrimaryConstructor = """
        namespace Ordering.Domain
        {
            public class Order()
            {
                public Order(Guid id) : this()
                {
                }
            }
        }
        """;

        private static DriftReport Compare(string generated, string onDisk, string type, Modellable modellable, bool emptyPrimaryConstructor = false, string recipe = "{}")
        {
            return Check("Domain", recipe, new List<ModelFact>(),
                new Dictionary<string, string> { [Path] = onDisk },
                new[]
                {
                    new GeneratedFile
                    {
                        RelativePath = Path,
                        Text = generated,
                        ModellableTypes = new Dictionary<string, Modellable> { [type] = modellable },
                        EmptyPrimaryConstructors = emptyPrimaryConstructor ? new[] { type } : null
                    }
                });
        }

        [Fact]
        public void A_record_with_empty_parentheses_is_a_record_with_no_parameters()
        {
            const string generated = "namespace Ordering.Domain\n{\n    public record OrderShipped : DomainEvent;\n}\n";
            const string onDisk = "namespace Ordering.Domain\n{\n    public record OrderShipped() : DomainEvent;\n}\n";

            Compare(generated, onDisk, "OrderShipped", DomainEvent).WrongLines.ShouldBeEmpty();
            Compare(generated, onDisk, "OrderShipped", Modellable.Everything).WrongLines.ShouldBeEmpty();
        }

        [Fact]
        public void Real_parameters_on_the_type_are_still_compared()
        {
            const string generated = "namespace Ordering.Domain\n{\n    public record OrderShipped : DomainEvent;\n}\n";

            Compare(generated, "namespace Ordering.Domain\n{\n    public record OrderShipped(Guid OrderId) : DomainEvent;\n}\n", "OrderShipped", DomainEvent).WrongLines.ShouldBe(new[]
            {
                "`OrderShipped` parameters: model (none), code `(Guid OrderId)`. Location: `src/Ordering.cs:3`"
            });
        }

        [Fact]
        public void Beside_other_constructors_it_is_a_public_parameterless_constructor()
        {
            Compare(GeneratedOrder, OrderWithEmptyPrimaryConstructor, "Order", DomainClass).WrongLines.ShouldBe(new[]
            {
                "`Order.Order()` (public constructor): not modelled. Location: `src/Ordering.cs:3`"
            });
        }

        [Fact]
        public void A_modelled_parameterless_constructor_is_the_empty_primary_constructor()
        {
            const string generated = """
            namespace Ordering.Domain
            {
                public class Order
                {
                    public Order()
                    {
                    }

                    public Order(Guid id)
                    {
                    }
                }
            }
            """;

            // Matched, not left to the Software Factory as a constructor the code lacks.
            var report = Compare(generated, OrderWithEmptyPrimaryConstructor, "Order", DomainClass);

            report.WrongLines.ShouldBeEmpty();
            report.PendingCount.ShouldBe(0);
            Compare(generated, OrderWithEmptyPrimaryConstructor.Replace("public class Order()", "public class Order"), "Order", DomainClass).PendingCount.ShouldBe(1);
        }

        [Fact]
        public void The_models_empty_primary_constructor_describes_the_codes_parameterless_constructor()
        {
            // The model's Primary Constructor has no parameters, so the C# builder writes nothing for it on the type.
            const string ordinary = """
            namespace Ordering.Domain
            {
                public class Order
                {
                    public Order()
                    {
                    }

                    public Order(Guid id)
                    {
                    }
                }
            }
            """;

            Compare(GeneratedOrder, OrderWithEmptyPrimaryConstructor, "Order", DomainClass, emptyPrimaryConstructor: true).WrongLines.ShouldBeEmpty();
            Compare(GeneratedOrder, ordinary, "Order", DomainClass, emptyPrimaryConstructor: true).WrongLines.ShouldBeEmpty();
        }

        [Fact]
        public void On_an_abstract_type_it_is_protected()
        {
            // A protected constructor left out of the model is never drift.
            Compare(GeneratedOrder.Replace("public class", "public abstract class"), OrderWithEmptyPrimaryConstructor.Replace("public class", "public abstract class"),
                "Order", DomainClass).WrongLines.ShouldBeEmpty();
        }

        [Theory]
        [InlineData("Order")]
        [InlineData("Order.Order")]
        public void An_exception_for_the_type_or_its_constructor_covers_it(string exception)
        {
            var report = Compare(GeneratedOrder, OrderWithEmptyPrimaryConstructor, "Order", DomainClass, recipe: $$"""{ "exceptions": ["{{exception}}"] }""");

            report.WrongLines.ShouldBeEmpty();
            report.UnusedExceptions.ShouldBeEmpty();
        }

        [Fact]
        public void Adding_or_removing_the_parentheses_does_not_change_the_declaration()
        {
            // The baseline reads it: a change here would count as the code changing since it last agreed with the model.
            static string Declaration(string code) => StructureComparer.CodeFingerprints(Type(code))["OrderShipped"];

            Declaration("public record OrderShipped() : DomainEvent;").ShouldBe(Declaration("public record OrderShipped : DomainEvent;"));
            Declaration("public record OrderShipped(Guid OrderId) : DomainEvent;").ShouldNotBe(Declaration("public record OrderShipped : DomainEvent;"));
        }

        [Fact]
        public void The_drift_check_reads_an_empty_primary_constructor_from_the_builder()
        {
            var file = new CSharpFile("Acme", "src").AddClass("Order").AddRecord("Money").AddClass("Customer");
            file.TypeDeclarations[0].AddPrimaryConstructor();
            file.TypeDeclarations[1].AddPrimaryConstructor(ctor => ctor.AddParameter("decimal", "Amount"));
            file.TypeDeclarations[2].AddConstructor();

            DriftCheckRunner.EmptyPrimaryConstructors(file).ShouldBe(new[] { "Order" });
        }

        private static BaseTypeDeclarationSyntax Type(string code) =>
            CSharpSyntaxTree.ParseText(code, CSharpParseOptions.Default.WithLanguageVersion(LanguageVersion.Preview)).GetRoot().DescendantNodes().OfType<BaseTypeDeclarationSyntax>().First();
    }
}
