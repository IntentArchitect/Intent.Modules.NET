using System.Collections.Generic;
using Intent.Modules.Domain.Guardrails.Contracts.Drift;
using Shouldly;
using Xunit;
using static Intent.Modules.Guardrails.Contracts.Tests.Drift;

namespace Intent.Modules.Guardrails.Contracts.Tests
{
    /// <summary>A contract template's output compared with its file on disk.</summary>
    public class StructureTests
    {
        private const string Path = "src/Domain/Order.cs";

        private const string Generated = """
        using System;
        using System.Collections.Generic;

        [assembly: DefaultIntentManaged(Mode.Merge)]

        namespace Acme.Domain
        {
            public class Order : Entity
            {
                public Order(Guid id)
                {
                }

                public Guid Id { get; set; }
                public DateTime? ShippedOn { get; set; }
                public ICollection<OrderLine> Lines { get; set; }
                protected string Notes { get; set; }

                public decimal Total(bool includeTax = true)
                {
                }

                protected virtual void Recalculate()
                {
                }
            }

            public enum OrderStatus
            {
                Open = 1,
                Closed = 2
            }
        }
        """;

        private const string OnDisk = """
        using System;
        using System.Collections.Generic;
        using Intent.RoslynWeaver.Attributes;

        [assembly: DefaultIntentManaged(Mode.Merge)]

        namespace Acme.Domain
        {
            public class Order : Entity, IAggregateRoot
            {
                private readonly List<OrderLine> _lines = new();

                public Order(Guid id)
                {
                    Id = id;
                }

                protected Order()
                {
                }

                public System.Guid Id { get; set; }
                public Nullable<DateTime> ShippedOn { get; private set; }
                public ICollection<OrderLine> Lines { get; set; }
                protected string Notes { get; set; }

                public decimal Total(bool includeTax = true)
                {
                    return 0;
                }

                protected virtual void Recalculate()
                {
                    // Hand-written body.
                }
            }

            public enum OrderStatus
            {
                Open = 1,
                Closed = 2
            }
        }
        """;

        private static DriftReport Compare(string onDisk, string generated = Generated)
        {
            return Check("Domain", "{}", new List<ModelFact>(),
                new Dictionary<string, string> { [Path] = onDisk },
                new[] { new GeneratedFile { RelativePath = Path, Text = generated } });
        }

        [Fact]
        public void Code_that_matches_the_generated_types_has_no_drift()
        {
            // Spelling differences that mean the same type, private fields, bodies, accessors and extra base types aren't drift.
            Compare(OnDisk).WrongLines.ShouldBeEmpty();
        }

        [Fact]
        public void A_changed_property_type_contradicts_the_model()
        {
            var report = Compare(OnDisk.Replace("public Nullable<DateTime> ShippedOn", "public DateTime ShippedOn"));

            report.Wrong(DriftSection.Contradicted).ShouldBe(new[]
            {
                "`Order.ShippedOn` type: model `DateTime?`, code `DateTime`. Location: `src/Domain/Order.cs:23`"
            });
        }

        [Fact]
        public void A_changed_method_signature_contradicts_the_model()
        {
            var report = Compare(OnDisk.Replace("public decimal Total(bool includeTax = true)", "public decimal Total(bool includeTax = false, string currency = null)"));

            report.Wrong(DriftSection.Contradicted).ShouldBe(new[]
            {
                "`Order.Total(bool)` parameters: model `(bool includeTax=true)`, code `(bool includeTax=false,string currency=null)`. Location: `src/Domain/Order.cs:27`"
            });
        }

        [Fact]
        public void Changed_modifiers_contradict_the_model()
        {
            var report = Compare(OnDisk
                .Replace("protected virtual void Recalculate()", "public virtual void Recalculate()")
                .Replace("public class Order : Entity, IAggregateRoot", "public sealed class Order : Entity, IAggregateRoot"));

            report.Wrong(DriftSection.Contradicted).ShouldBe(new[]
            {
                "`Order.Recalculate()` modifiers: model `protected virtual`, code `public virtual`. Location: `src/Domain/Order.cs:32`",
                "`Order` modifiers: model `public`, code `public sealed`. Location: `src/Domain/Order.cs:9`"
            });
        }

        [Fact]
        public void A_modelled_member_or_type_missing_from_the_code_is_reported()
        {
            var report = Compare(OnDisk
                .Replace("        protected string Notes { get; set; }\n", string.Empty)
                .Replace("        protected string Notes { get; set; }\r\n", string.Empty)
                .Replace("enum OrderStatus", "enum OrderState"));

            report.Wrong(DriftSection.Missing).ShouldBe(new[]
            {
                "`Order.Notes` (property): modelled, not in the code. Expected in `src/Domain/Order.cs`",
                "`OrderStatus` (enum): modelled, not in `src/Domain/Order.cs`"
            });
        }

        [Fact]
        public void A_file_that_does_not_exist_yet_is_reported_as_missing()
        {
            var report = Check("Domain", "{}", new List<ModelFact>(), new Dictionary<string, string>(),
                new[] { new GeneratedFile { RelativePath = Path, Text = Generated } });

            report.Wrong(DriftSection.Missing).ShouldBe(new[]
            {
                "`OrderStatus` (enum): modelled, not in `src/Domain/Order.cs`",
                "`Order` (class): modelled, not in `src/Domain/Order.cs`"
            });
        }

        [Fact]
        public void Only_unmodelled_public_members_are_reported()
        {
            // The model has public and protected methods and properties, and public constructors. Only a public member it
            // doesn't describe is reported: whether a protected, internal or private one belongs in the model is a judgement
            // call, even where the model has modelled others like it. A field isn't reported: no designer models one.
            var report = Compare(OnDisk
                .Replace("protected string Notes { get; set; }", """
                protected string Notes { get; set; }
                        public string Reference { get; set; }
                        protected string Audit { get; set; }
                        internal string Internal { get; set; }
                        private string Secret { get; set; }
                        public void Ship() { }
                        protected void Log() { }
                        private void Trace() { }
                        public int Count;
                        public Order(string reference) { }
                """)
                .Replace("Closed = 2", "Closed = 2,\n        Cancelled = 3"));

            report.Wrong(DriftSection.Undescribed).ShouldBe(new[]
            {
                "`Order.Order(string)` (public constructor): not modelled. Location: `src/Domain/Order.cs:34`",
                "`Order.Reference` (public property): not modelled. Location: `src/Domain/Order.cs:26`",
                "`Order.Ship()` (public method): not modelled. Location: `src/Domain/Order.cs:30`",
                "`OrderStatus.Cancelled` (public literal): not modelled. Location: `src/Domain/Order.cs:51`"
            });
        }

        [Fact]
        public void A_changed_kind_or_enum_value_contradicts_the_model()
        {
            var report = Compare(OnDisk.Replace("public class Order : Entity, IAggregateRoot", "public record Order : Entity, IAggregateRoot").Replace("Closed = 2", "Closed = 3"));

            report.Wrong(DriftSection.Contradicted).ShouldBe(new[]
            {
                "`OrderStatus.Closed` value: model `2`, code `3`. Location: `src/Domain/Order.cs:41`",
                "`Order` kind: model `class`, code `record`. Location: `src/Domain/Order.cs:9`"
            });
        }

        [Fact]
        public void A_modelled_base_type_missing_from_the_code_contradicts_the_model()
        {
            var report = Compare(OnDisk.Replace("public class Order : Entity, IAggregateRoot", "public class Order : IAggregateRoot"));

            report.Wrong(DriftSection.Contradicted).ShouldBe(new[]
            {
                "`Order` base types: model `Entity`, code `IAggregateRoot`. Location: `src/Domain/Order.cs:9`"
            });
        }
    }
}
