using System.Collections.Generic;
using Intent.Modules.Domain.Guardrails.Contracts.Drift;
using Shouldly;
using Xunit;
using static Intent.Modules.Guardrails.Contracts.Tests.Drift;

namespace Intent.Modules.Guardrails.Contracts.Tests
{
    /// <summary>
    /// New code that should have been modelled is found by the conventions the recipe file records, one <c>own</c> entry
    /// per element type. Without a convention nothing is reported; types some template writes count as modelled.
    /// </summary>
    public class TypeConventionTests
    {
        private const string OrderDto = """
        namespace Acme.Dtos
        {
            public partial class OrderDTO
            {
                public int Id { get; set; }
            }
        }
        """;

        private const string DtoConvention = """
        { "own": [{ "kind": "DTO", "find": "class with \\bclass \\w+DTO\\b", "files": "src/Dtos/**/*.cs" }] }
        """;

        private static readonly GeneratedFile[] Generated = { new() { RelativePath = "src/Dtos/OrderDTO.cs", Text = OrderDto } };

        private static Dictionary<string, string> Code(params (string Path, string Text)[] files)
        {
            var code = new Dictionary<string, string> { ["src/Dtos/OrderDTO.cs"] = OrderDto };
            foreach (var (path, text) in files)
            {
                code[path] = text;
            }

            return code;
        }

        [Fact]
        public void A_new_type_that_follows_a_convention_is_reported_and_a_helper_beside_it_is_not()
        {
            var code = Code(
                ("src/Dtos/MillSummaryDTO.cs", "namespace Acme.Dtos\n{\n    public class MillSummaryDTO\n    {\n    }\n}\n"),
                ("src/Dtos/Mills/MillDTO.cs", "namespace Acme.Dtos.Mills\n{\n    public class MillDTO\n    {\n    }\n}\n"),
                ("src/Dtos/AutoMapperConfig.cs", "namespace Acme.Dtos\n{\n    public static class AutoMapperConfig\n    {\n    }\n\n    public class MappingProfile : Profile\n    {\n    }\n}\n"));

            Check("Services", DtoConvention, new List<ModelFact>(), code, Generated).WrongLines.ShouldBe(new[]
            {
                "DTO `MillDTO`: not modelled. Location: `src/Dtos/Mills/MillDTO.cs:3`",
                "DTO `MillSummaryDTO`: not modelled. Location: `src/Dtos/MillSummaryDTO.cs:3`"
            });
        }

        [Fact]
        public void Without_a_convention_new_types_are_not_reported()
        {
            var code = Code(
                ("src/Dtos/MillSummaryDTO.cs", "namespace Acme.Dtos\n{\n    public class MillSummaryDTO\n    {\n    }\n}\n"),
                ("src/Dtos/IBlobStorage.cs", "namespace Acme.Dtos\n{\n    public interface IBlobStorage\n    {\n    }\n}\n"));

            Check("Services", "{}", new List<ModelFact>(), code, Generated).WrongLines.ShouldBeEmpty();
        }

        [Fact]
        public void Another_part_of_a_generated_type_and_a_type_another_template_writes_follow_the_convention_and_are_modelled()
        {
            var code = Code(
                ("src/Dtos/OrderDTO.Mapping.cs", "namespace Acme.Dtos\n{\n    public partial class OrderDTO\n    {\n        public static OrderDTO From(Order order) => new();\n    }\n}\n"),
                ("src/Dtos/CustomerDTO.cs", "namespace Acme.Dtos\n{\n    public class CustomerDTO\n    {\n    }\n}\n"));

            // CustomerDTO.cs is written by another designer's or module's template in the same run.
            Check("Services", DtoConvention, new List<ModelFact>(), code, Generated, outputPaths: new[] { "src/Dtos/CustomerDTO.cs" })
                .WrongLines.ShouldBeEmpty();
        }

        [Fact]
        public void A_convention_by_base_type_finds_new_types_in_any_folder()
        {
            const string recipe = """
            { "own": [{ "kind": "Service", "find": "class with :BaseBusinessObject\\b", "files": "src/Business/**/*.cs" }] }
            """;
            const string mills = "namespace Acme.Business\n{\n    public class Mills : BaseBusinessObject\n    {\n    }\n}\n";
            var code = new Dictionary<string, string>
            {
                ["src/Business/Ssb/Mills.cs"] = mills,
                ["src/Business/Reports/MillReport.cs"] = "namespace Acme.Business.Reports\n{\n    public class MillReport : BaseBusinessObject\n    {\n    }\n}\n",
                ["src/Business/AutoMapperConfig.cs"] = "namespace Acme.Business\n{\n    public static class AutoMapperConfig\n    {\n    }\n}\n"
            };

            var report = Check("Services", recipe, new List<ModelFact>(), code, new[] { new GeneratedFile { RelativePath = "src/Business/Ssb/Mills.cs", Text = mills } });

            report.WrongLines.ShouldBe(new[] { "Service `MillReport`: not modelled. Location: `src/Business/Reports/MillReport.cs:3`" });
        }

        [Fact]
        public void Exceptions_cover_a_type_a_convention_reports()
        {
            var code = Code(
                ("src/Dtos/PagedResultDTO.cs", "namespace Acme.Dtos\n{\n    public class PagedResultDTO<T>\n    {\n    }\n}\n"),
                ("src/Dtos/Legacy/OldOrderDTO.cs", "namespace Acme.Dtos.Legacy\n{\n    public class OldOrderDTO\n    {\n    }\n}\n"));
            const string recipe = """
            {
              "own": [{ "kind": "DTO", "find": "class with \\bclass \\w+DTO\\b", "files": "src/Dtos/**/*.cs" }],
              "exceptions": [{ "match": "PagedResultDTO", "reason": "paging helper" }, "src/Dtos/Legacy/*.cs"]
            }
            """;

            var report = Check("Services", recipe, new List<ModelFact>(), code, Generated);

            report.WrongLines.ShouldBeEmpty();
            report.UnusedExceptions.ShouldBeEmpty();
        }
    }
}
